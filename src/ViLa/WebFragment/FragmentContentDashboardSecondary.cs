using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents the secondary dashboard content with yearly and monthly measurement summaries.
    /// </summary>
    [Section<SectionContentSecondary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Index>]
    public sealed class FragmentContentDashboardSecondary : FragmentControlPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentContentDashboardSecondary(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Renders the control into an HTML node.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree control.</param>
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            Clear();

            AddSummary(renderContext);
            var year = DateTime.Now.AddDays(ViewModel.Instance.Settings.BillingDayOffset).Year;
            AddYearSection(renderContext, year);
            AddYearSection(renderContext, year - 1);
            AddYearSection(renderContext, year - 2);

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Compares monthly consumption across four billing years and displays the current billing totals.
        /// </summary>
        /// <param name="renderContext">The context providing the display culture.</param>
        private void AddSummary(IRenderControlContext renderContext)
        {
            var model = ViewModel.Instance;
            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
            var history = model.GetHistoryMeasurementLogs().ToArray();
            AddActivity(history, DateTime.Today, culture);
            var current = DateTime.Now.AddDays(model.Settings.BillingDayOffset);
            var month = new DateTime(current.Year, current.Month, 1);
            var months = Enumerable.Range(1, 12).ToArray();
            var totals = history.GroupBy(log =>
            {
                var billingDate = log.Till.AddDays(model.Settings.BillingDayOffset);
                return (billingDate.Year, billingDate.Month);
            }).ToDictionary(group => group.Key, group => group.Sum(log => log.FinalPower));
            var logs = BillingPeriod.GetLogs(history, month, model.Settings.BillingDayOffset);
            var summary = new ControlSection("vila-consumption-section")
            {
                Header = _ => "vila:vila.dashboard.summary",
                Layout = _ => TypeLayoutSection.Rule
            };
            if (history.Length == 0)
            {
                summary.Add(new ControlEmptyState { Message = _ => "vila:vila.dashboard.empty" });
                Add(summary);
                return;
            }
            summary.Add(new ControlText()
            {
                Text = _ => BillingPeriod.FormatCosts(logs, culture, model.Settings.Currency)
            });
            var views = new ControlView("vila-consumption-views") { Layout = _ => TypeLayoutView.ToggleGroup };
            var yearlyView = new ControlViewItem("vila-yearly-comparison") { Title = _ => "vila:vila.dashboard.comparison.title" };
            yearlyView.Add(new ControlText { Text = _ => "vila:vila.dashboard.comparison" });
            yearlyView.Add(new ControlChart("vila-monthly-chart")
            {
                Type = _ => TypeChart.Bar,
                Labels = _ => months.Select(culture.DateTimeFormat.GetAbbreviatedMonthName),
                Data = _ => Enumerable.Range(0, 4).Select(offset =>
                {
                    var year = current.Year - offset;
                    var count = offset == 0 ? current.Month : 12;
                    return new ControlChartDataset
                    {
                        Title = year.ToString(culture),
                        Data = new ControlChartDatasetPointCollection(months.Take(count).Select(number =>
                            totals.TryGetValue((year, number), out var energy) ? energy : 0).ToArray())
                    };
                }).ToArray(),
                TitleY = _ => "kWh",
                Responsive = _ => true,
                Height = _ => 220,
                MaintainAspectRatio = _ => false,
                Minimum = _ => 0
            });
            views.Add(yearlyView);
            var vehicleView = new ControlViewItem("vila-vehicle-comparison") { Title = _ => "vila:vila.dashboard.vehicles" };
            vehicleView.Add(new ControlText { Text = _ => current.Year.ToString(culture) });
            vehicleView.Add(new ControlChart("vila-vehicle-chart")
            {
                Type = _ => TypeChart.Line,
                Labels = _ => months.Take(current.Month).Select(culture.DateTimeFormat.GetAbbreviatedMonthName),
                Data = context => history.Where(log => log.Till.AddDays(model.Settings.BillingDayOffset).Year == current.Year
                        && log.Till.AddDays(model.Settings.BillingDayOffset).Month <= current.Month)
                    .GroupBy(log => log.VehicleId ?? string.Empty).Select(group =>
                    {
                        var vehicle = model.Vehicles.GetVehicle(group.Key);
                        var color = string.IsNullOrEmpty(group.Key) ? "#6c757d" : vehicle?.Color ?? Vehicle.DefaultColor(group.Key);
                        return new ControlChartDataset
                        {
                            Title = string.IsNullOrEmpty(group.Key) ? I18N.Translate(context, "vila:vila.vehicles.badge.unassigned")
                                : vehicle?.Name ?? group.First().VehicleName ?? group.Key,
                            BorderColor = color,
                            BackgroundColor = color,
                            Data = new ControlChartDatasetPointCollection(months.Take(current.Month).Select(number =>
                                group.Where(log => log.Till.AddDays(model.Settings.BillingDayOffset).Month == number).Sum(log => log.FinalPower)).ToArray())
                        };
                    }).ToArray(),
                TitleY = _ => "kWh",
                Responsive = _ => true,
                Height = _ => 220,
                MaintainAspectRatio = _ => false,
                Minimum = _ => 0
            });
            views.Add(vehicleView);
            summary.Add(views);
            Add(summary);
        }

        /// <summary>
        /// Displays completed charging sessions per day for the last 365 calendar days.
        /// </summary>
        /// <param name="history">The completed measurements.</param>
        /// <param name="today">The inclusive end of the activity window.</param>
        /// <param name="culture">The culture used for the calendar labels.</param>
        private void AddActivity(MeasurementLog[] history, DateTime today, CultureInfo culture)
        {
            var first = today.AddDays(-364);
            var counts = history.Where(x => x.Till.Date >= first && x.Till.Date <= today)
                .GroupBy(x => x.Till.Date).ToDictionary(group => group.Key, group => group.Count());
            var firstMonth = new DateTime(first.Year, first.Month, 1);
            var months = Enumerable.Range(0, 13).Select(firstMonth.AddMonths).TakeWhile(month => month <= today).ToArray();
            var days = Enumerable.Range(1, 31).ToArray();
            var section = new ControlSection("vila-activity-section")
            {
                Header = _ => "vila:vila.dashboard.activity",
                Layout = _ => TypeLayoutSection.Rule
            };
            section.Add(new ControlText { Text = _ => "vila:vila.dashboard.activity.help" });
            var scroll = new ControlPanel { Classes = new[] { "vila-heatmap-scroll" } };
            scroll.Add(new ControlHeatMap("vila-activity")
            {
                RowLabels = _ => months.Select(month => month.ToString("MMM yyyy", culture)),
                ColumnLabels = _ => days.Select(day => day.ToString(culture)),
                Min = _ => 0,
                Max = _ => counts.Values.DefaultIfEmpty(1).Max(),
                LowColor = _ => "#e4eeed",
                HighColor = _ => "#08756b",
                Values = _ => months.Select(month => days.Select(day =>
                {
                    var date = month.AddDays(day - 1);
                    if (date.Month != month.Month || date < first || date > today)
                    {
                        return double.NaN;
                    }
                    return counts.TryGetValue(date, out var count) ? count : 0;
                }))
            });
            section.Add(scroll);
            Add(section);
        }

        /// <summary>
        /// Adds the populated billing months for a nominal calendar year.
        /// </summary>
        /// <param name="renderContext">The context providing the display culture.</param>
        /// <param name="year">The nominal billing year.</param>
        private void AddYearSection(IRenderControlContext renderContext, int year)
        {
            var history = ViewModel.Instance.GetHistoryMeasurementLogs().ToArray();
            if (!Enumerable.Range(1, 12).Any(month => BillingPeriod.GetLogs(history,
                new DateTime(year, month, 1), ViewModel.Instance.Settings.BillingDayOffset).Length > 0))
            {
                return;
            }

            Add
            (
                new ControlText()
                {
                    Text = _ => year.ToString(),
                    Format = _ => TypeFormatText.H3,
                    Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two, PropertySpacing.Space.None),
                    TextColor = _ => new PropertyColorText(TypeColorText.Primary)
                }
            );

            var offset = ViewModel.Instance.Settings.BillingDayOffset;
            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;

            for (var month = 12; month >= 1; month--)
            {
                var logs = BillingPeriod.GetLogs(history, new DateTime(year, month, 1), offset);

                if (!logs.Any())
                {
                    continue;
                }

                var monthName = culture.DateTimeFormat.GetMonthName(month);
                Add
                (
                    new ControlText()
                    {
                        Text = _ =>
                        {
                            return string.Format
                            (
                                culture,
                                "{0}: {1} / {2:F2} kWh",
                                monthName,
                                BillingPeriod.FormatCosts(logs, culture, ViewModel.Instance.Settings.Currency),
                                logs.Sum(x => x.FinalPower)
                            );
                        },
                        Format = _ => TypeFormatText.H4,
                        Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None),
                        TextColor = _ => new PropertyColorText(TypeColorText.Primary)
                    }
                );

                foreach (var group in logs.GroupBy(log => log.VehicleId ?? string.Empty))
                {
                    var panel = new ControlPanel { Classes = new[] { "vila-property-breakdown" } };
                    panel.Add(new ControlVehicleBadge(group.First()));
                    panel.Add
                    (
                        new ControlText()
                        {
                            Text = _ => string.Format
                            (
                                culture,
                                "{0} / {1:F2} kWh",
                                BillingPeriod.FormatCosts(group, culture, ViewModel.Instance.Settings.Currency),
                                group.Sum(x => x.FinalPower)
                            ),
                            Format = _ => TypeFormatText.Span,
                            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                        }
                    );

                    Add(panel);
                }

                var grid = new ControlGrid()
                {
                    Fluid = _ => TypePanelContainer.Fluid
                };

                foreach (var measurementLog in logs)
                {
                    var card = new ControlCardMeasurementLog()
                    {
                        MeasurementLog = measurementLog,
                        GridColumn = _ => new PropertyGrid(TypeDevice.Medium, 3)
                    };

                    grid.Add(card);
                }

                Add(grid);
            }
        }
    }
}
