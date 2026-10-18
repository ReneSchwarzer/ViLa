using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebSection;
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
        /// Adds a twelve month consumption chart and the current billing totals.
        /// </summary>
        /// <param name="renderContext">The context providing the display culture.</param>
        private void AddSummary(IRenderControlContext renderContext)
        {
            var model = ViewModel.Instance;
            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
            var history = model.GetHistoryMeasurementLogs().ToArray();
            var current = DateTime.Now.AddDays(model.Settings.BillingDayOffset);
            var month = new DateTime(current.Year, current.Month, 1);
            var months = Enumerable.Range(0, 12).Select(index => month.AddMonths(index - 11)).ToArray();
            var logs = BillingPeriod.GetLogs(history, month, model.Settings.BillingDayOffset);
            var summary = new ControlCard()
            {
                Header = _ => "vila:vila.dashboard.summary",
                Classes = new[] { "vila-summary" }
            };
            summary.Add(new ControlCardCounter()
            {
                Value = _ => logs.Length,
                Text = _ => "vila:vila.dashboard.sessions"
            });
            summary.Add(new ControlText()
            {
                Text = _ => string.Format(culture, "{0:Y}: {1:F2} kWh", month, logs.Sum(x => x.FinalPower)),
                Format = _ => TypeFormatText.H3
            });
            summary.Add(new ControlText()
            {
                Text = _ => BillingPeriod.FormatCosts(logs, culture, model.Settings.Currency)
            });
            summary.Add(new ControlChart("vila-monthly-chart")
            {
                Type = _ => TypeChart.Bar,
                Labels = _ => months.Select(x => x.ToString("MMM yy", culture)),
                Data = _ => new[] { new ControlChartDataset()
                {
                    Title = "kWh",
                    Data = new ControlChartDatasetPointCollection(months.Select(x =>
                        BillingPeriod.GetLogs(history, x, model.Settings.BillingDayOffset).Sum(log => log.FinalPower)).ToArray())
                } },
                TitleY = _ => "kWh",
                Responsive = _ => true,
                Height = _ => 220,
                MaintainAspectRatio = _ => false,
                Minimum = _ => 0
            });
            if (history.Length == 0)
            {
                summary.Add(new ControlText() { Text = _ => "vila:vila.dashboard.empty" });
            }
            Add(summary);
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

                foreach (var tag in logs
                    .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                    .SelectMany(BillingPeriod.GetTags)
                    .Distinct())
                {
                    var taggedLogs = logs
                        .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                        .Where(x => BillingPeriod.GetTags(x).Contains(tag))
                        .ToList();

                    var panel = new ControlPanel()
                    {
                        Fluid = _ => TypePanelContainer.Fluid
                    };

                    panel.Add
                    (
                        new ControlTag()
                        {
                            Value = _ => tag,
                            Color = _ => new PropertyColorTag(ViewModel.Instance.GetColor(tag))
                        }
                    );

                    panel.Add
                    (
                        new ControlText()
                        {
                            Text = _ => string.Format
                            (
                                culture,
                                "{0} / {1:F2} kWh",
                                BillingPeriod.FormatCosts(taggedLogs, culture, ViewModel.Instance.Settings.Currency),
                                taggedLogs.Sum(x => x.FinalPower)
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
