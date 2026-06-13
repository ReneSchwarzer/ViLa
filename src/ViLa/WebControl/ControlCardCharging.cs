using System;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a control panel for card charging operations.
    /// </summary>
    public class ControlCardCharging : ControlPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">
        /// The identifier for the control card charging, or null to use a default value.
        /// </param>
        public ControlCardCharging(string id = null)
            : base(id)
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
            static string[] createArray(int size)
            {
                var array = new string[size];
                for (var i = size * -1; i < 0; i++)
                {
                    array[size + i] = (i + 1).ToString();
                }

                return array;
            }

            var chartLabels = ViewModel.Instance.ActiveCharging
                ? [.. ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => ViewModel.Instance.CurrentMeasurementLog.Measurements.IndexOf(x).ToString())]
                : createArray(ViewModel.Instance.CurrentMeasurementLog.Measurements.Count);

            var chartData = ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => x.Power * 60).ToArray();

            var card = new ControlPanelCard(null)
            {
                BackgroundColor = _ => new PropertyColorBackground(TypeColorBackground.Light)
            };

            var durationText = new ControlText("measurementtime")
            {
                Text = ctx =>
                {
                    var culture = ctx.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
                    var val = ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? new TimeSpanConverter().Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null) : "-";
                    return string.Format(culture, WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.duration"), val);
                }
            };

            var costText = new ControlText("cost")
            {
                Text = ctx =>
                {
                    var culture = ctx.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
                    var val = ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? (object)ViewModel.Instance.CurrentMeasurementLog?.Cost : "-";
                    return string.Format(culture, WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.cost"), val, ViewModel.Instance.Settings.Currency);
                }
            };

            var powerText = new ControlText("power")
            {
                Text = ctx =>
                {
                    var culture = ctx.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
                    var val = ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? (object)ViewModel.Instance.CurrentMeasurementLog?.Power : "-";
                    return string.Format(culture, WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.consumption"), val);
                }
            };

            var infoCard = new ControlPanelCard(null, durationText, costText, powerText)
            {
                HorizontalAlignment = _ => TypeHorizontalAlignment.Default,
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
            };
            card.Add(infoCard);

            var chart = new ControlChart("chart")
            {
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None),
                Title = _ => "",
                Labels = _ => chartLabels,
                Data = ctx => new[] {
                    new ControlChartDataset()
                    {
                        Data = new ControlChartDatasetPointCollection(chartData),
                        Title = WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(ctx.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.title")
                    }
                },
                TitleX = ctx => WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(ctx.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.title.x"),
                TitleY = ctx => WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(ctx.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.title.y"),
                Styles = new[] { "max-width: 85%;" },
                Minimum = _ => 0
            };
            card.Add(chart);

            Clear();
            Add(card);

            var builder = new System.Text.StringBuilder();
            //builder.AppendLine($"var restUrl='{renderContext.ApplicationContext.ContextPath.Append("api")}';");
            builder.AppendLine($"var currency='{ViewModel.Instance.Settings.Currency}';");
            builder.AppendLine($"var vila_charging_current='{WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.current")}';");
            builder.AppendLine($"var vila_charging_begin='{WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.begin")}';");
            builder.AppendLine($"var vila_charging_stop='{WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.stop")}';");
            builder.AppendLine($"var vila_charging_duration='{WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.duration")}';");
            builder.AppendLine($"var vila_charging_cost='{WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.cost")}';");
            builder.AppendLine($"var vila_charging_consumption='{WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture, "vila:vila.charging.consumption")}';");

            visualTree.AddScript($"charging_i18n", builder.ToString());

            return base.Render(renderContext, visualTree);
        }
    }
}
