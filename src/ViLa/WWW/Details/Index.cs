using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebUI.WebControl;

namespace ViLa.WWW.Details
{
    /// <summary>
    /// Represents the details page for a single measurement log.
    /// Renders the same view as the master 1.4.7 PageDetails: id muted,
    /// power/cost headline, time range, then a chart of the measurements.
    /// </summary>
    [Title("vila:vila.details.label")]
    [SegmentGuid<ParameterId>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Processes the visual tree using the specified render context.
        /// </summary>
        /// <param name="renderContext">The render context used for processing.</param>
        /// <param name="visualTree">The visual tree of the web application to be processed.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var id = renderContext?.Request?.GetParameter<ParameterId>()?.Value;

            if (string.IsNullOrEmpty(id))
            {
                visualTree.Content.MainPanel.AddPrimary(new ControlText()
                {
                    Text = _ => I18N.Translate(renderContext, "vila:vila.details.id", id ?? string.Empty),
                    TextColor = _ => new PropertyColorText(TypeColorText.Muted)
                });
                return;
            }

            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id);
            var culture = renderContext?.Request?.Culture ?? CultureInfo.CurrentCulture;

            // Muted id text (e.g. "28d065e7-...xml")
            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => id + ".xml",
                TextColor = _ => new PropertyColorText(TypeColorText.Muted)
            });

            // Headline: "{Power:F2} kWh / {Cost:F2} {Currency}"
            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => string.Format
                (
                    culture,
                    "{0:F2} kWh",
                    measurementLog?.Power ?? 0f
                )
                + " / " + string.Format
                (
                    culture,
                    "{0:F2} {1}",
                    measurementLog?.Cost ?? 0f,
                    ViewModel.Instance.Settings?.Currency ?? string.Empty
                ),
                Format = _ => TypeFormatText.H1,
                TextColor = _ => new PropertyColorText(TypeColorText.Primary)
            });

            // Time range paragraph
            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => measurementLog is null
                    ? string.Empty
                    : string.Format
                    (
                        culture,
                        "{0:HH:mm:ss} - {1:HH:mm:ss} {2}",
                        measurementLog.From,
                        measurementLog.Till,
                        I18N.Translate(renderContext, "vila:vila.charging.time")
                    ),
                Format = _ => TypeFormatText.Paragraph,
                TextColor = _ => new PropertyColorText(TypeColorText.Dark)
            });

            if (measurementLog is null || measurementLog.Measurements is null || measurementLog.Measurements.Count == 0)
            {
                return;
            }

            // Chart of power over time, same semantics as the master 1.4.7 PageDetails.
            // X axis = minutes since From, Y axis = Power * 60 (kW, since Power is kWh per minute slot).
            var labels = measurementLog.Measurements
                .Select(x => ((int)(x.MeasurementTimePoint - measurementLog.From).TotalMinutes).ToString(culture))
                .ToArray();
            var data = measurementLog.Measurements
                .Select(x => x.Power * 60f)
                .ToArray();

            visualTree.Content.MainPanel.AddPrimary(new ControlChart("chart")
            {
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None),
                Labels = _ => labels,
                Data = _ => new[] { new ControlChartDataset()
                {
                    Data = new ControlChartDatasetPointCollection(data),
                    Title = I18N.Translate(renderContext, "vila:vila.history.label")
                } },
                Title = _ => string.Empty,
                TitleX = _ => I18N.Translate(renderContext, "vila:vila.charging.title.x"),
                TitleY = _ => I18N.Translate(renderContext, "vila:vila.charging.title.y"),
                Styles = new[] { "max-width: 80%;" },
                Minimum = _ => 0f
            });
        }
    }
}
