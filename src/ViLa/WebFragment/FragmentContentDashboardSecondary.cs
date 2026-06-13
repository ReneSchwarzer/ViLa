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

            AddYearSection(renderContext, DateTime.Now.Year);
            AddYearSection(renderContext, DateTime.Now.Year - 1);
            AddYearSection(renderContext, DateTime.Now.Year - 2);

            return base.Render(renderContext, visualTree);
        }

        private void AddYearSection(IRenderControlContext renderContext, int year)
        {
            if (!ViewModel.Instance.GetHistoryMeasurementLogs().Any(x => x.From.Year == year))
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

            var offset = -ViewModel.Instance.Settings.BillingDayOffset;
            var culture = CultureInfo.CurrentCulture;

            for (var month = 12; month > 1; month--)
            {
                var from = new DateTime(year, month, 1).AddDays(offset);
                var till = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59).AddDays(offset);
                var logs = ViewModel.Instance.GetHistoryMeasurementLogs(from, till).ToList();

                if (!logs.Any())
                {
                    continue;
                }

                Add
                (
                    new ControlText()
                    {
                        Text = _ =>
                        {
                            var monthName = culture.DateTimeFormat.GetMonthName(month);
                            return string.Format
                            (
                                culture,
                                "{0} - {1:F2} {2} / {3:F2} kWh",
                                monthName,
                                logs.Sum(x => x.FinalCost),
                                ViewModel.Instance.Settings.Currency,
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
                    .SelectMany(x => x.Tag.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .Distinct())
                {
                    var taggedLogs = logs
                        .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                        .Where(x => x.Tag.Contains(tag))
                        .ToList();

                    var panel = new ControlPanel()
                    {
                        Fluid = _ => TypePanelContainer.Fluid
                    };

                    panel.Add
                    (
                        new ControlTag(tag)
                        {
                            BackgroundColor = _ => new PropertyColorBackground(ViewModel.Instance.GetColor(tag))
                        }
                    );

                    panel.Add
                    (
                        new ControlText()
                        {
                            Text = _ => string.Format
                            (
                                culture,
                                "{0:F2} {1} / {2:F2} kWh",
                                taggedLogs.Sum(x => x.FinalCost),
                                ViewModel.Instance.Settings.Currency,
                                taggedLogs.Sum(x => x.FinalPower)
                            ),
                            Format = _ => TypeFormatText.Span,
                            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                        }
                    );

                    Add(panel);
                }

                var grid = new ControlPanelGrid()
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
