using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebApp.WebScope;
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
                    measurementLog?.FinalPower ?? 0f
                )
                + " / " + string.Format
                (
                    culture,
                    "{0:F2} {1}",
                    measurementLog?.FinalCost ?? 0f,
                    measurementLog?.Currency ?? ViewModel.Instance.Settings?.Currency ?? string.Empty
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

            if (measurementLog?.Measurements?.Count > 0)
            {
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

            if (measurementLog != null)
            {
                var deleteUri = WebEx.ComponentHub.SitemapManager.GetUri<WWW.Api._1_.History.Delete>(renderContext.Request.ApplicationContext);
                var confirm = new ControlModal("delete-measurement-" + id)
                {
                    Header = _ => "vila:vila.details.delete",
                    CloseLabel = _ => "webexpress.webui:form.cancel.label"
                };
                confirm.Add(new ControlHtml
                {
                    Html = context => $"<form method='post' action='{System.Net.WebUtility.HtmlEncode(deleteUri.ToString())}'>" +
                        $"<input type='hidden' name='id' value='{System.Net.WebUtility.HtmlEncode(id)}'>" +
                        $"<p>{I18N.Translate(context, "vila:vila.delete.description")}</p>" +
                        $"<p><code>{System.Net.WebUtility.HtmlEncode(id)}</code></p>" +
                        $"<label for='confirm-{id}'>{I18N.Translate(context, "vila:vila.details.confirm.id")}</label>" +
                        $"<input id='confirm-{id}' class='form-control mb-3' name='confirmation' type='text' required " +
                        $"pattern='{System.Net.WebUtility.HtmlEncode(id)}' autocomplete='off' spellcheck='false'>" +
                        $"<button class='btn btn-danger' type='submit'>{I18N.Translate(context, "vila:vila.details.delete")}</button></form>"
                });
                visualTree.Content.MainPanel.AddPrimary(confirm);
                var editorId = "measurement-description-editor-" + measurementLog.ID;
                var section = new ControlSection { Header = _ => "vila:vila.description.title" };
                var description = new ControlPanel { Classes = new[] { "vila-description" } };
                description.Add(new ControlContent
                {
                    Content = _ => measurementLog.Description,
                    Placeholder = _ => "vila:vila.description.empty"
                });
                visualTree.Content.MainPanel.Headline.AddSecondary(new ControlButton
                {
                    Text = _ => "vila:vila.description.edit",
                    Icon = _ => new IconPenToSquare(),
                    PrimaryAction = _ => new ActionModal(editorId)
                });
                description.Add(new ViLa.WebControl.ControlDescriptionEditor(editorId)
                {
                    ItemId = _ => measurementLog.ID,
                    Draft = _ => false,
                    Title = { Name = _ => "Title", Disabled = _ => true },
                    Body = { Name = _ => "Body" },
                    ServiceFactory = context => DataServiceDescriptor.FormData(WebEx.ComponentHub.SitemapManager
                        .GetUri<WWW.Api._1_.History.Description>(context.Request.ApplicationContext).ToString())
                });
                section.Add(description);
                visualTree.Content.MainPanel.AddPrimary(section);
            }
        }
    }
}
