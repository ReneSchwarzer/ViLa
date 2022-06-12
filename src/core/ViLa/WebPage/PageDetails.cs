using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using WebExpress.Internationalization;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebAttribute;
using WebExpress.WebResource;

namespace ViLa.WebPage
{
    [Id("Details")]
    [Title("vila:vila.details.label")]
    [SegmentGuid("id", "vila:vila.details.id", SegmentGuidAttribute.Format.Simple)]
    [Path("/")]
    [Path("/History")]
    [Module("ViLa")]
    [Context("general")]
    [Context("details")]
    public sealed class PageDetails : PageWebApp
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageDetails()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        /// <param name="context">Der Kontext zum Rendern der Seite</param>
        public override void Process(RenderContextWebApp context)
        {
            base.Process(context);

            var id = context.Request.GetParameter("id");
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);
            var chartLabels = measurementLog?.Measurements.Select(x => measurementLog?.Measurements.IndexOf(x).ToString()).ToArray();
            var chartData = measurementLog?.Measurements.Select(x => x.Power * 60).ToArray();

            context.VisualTree.Content.Primary.Add(new ControlText()
            {
                Text = id.Value + ".xml",
                TextColor = new PropertyColorText(TypeColorText.Muted)
            });

            context.VisualTree.Content.Primary.Add(new ControlText()
            {
                Text = string.Format("{0:F2} kWh", measurementLog?.FinalPower) + " / " + string.Format("{0:F2} {1}", measurementLog?.FinalCost, ViewModel.Instance.Settings.Currency),
                Format = TypeFormatText.H1,
                TextColor = new PropertyColorText(TypeColorText.Primary)
            });

            context.VisualTree.Content.Primary.Add(new ControlText()
            {
                Text = $"{measurementLog?.FinalFrom.ToString(Culture.DateTimeFormat.LongDatePattern)}",
                Format = TypeFormatText.H5,
                TextColor = new PropertyColorText(TypeColorText.Primary)
            });

            context.VisualTree.Content.Primary.Add(new ControlText()
            {
                Text = $"{measurementLog?.FinalFrom.ToString(Culture.DateTimeFormat.LongTimePattern)} - {measurementLog?.FinalTill.ToString(Culture.DateTimeFormat.LongTimePattern)} {this.I18N("vila:vila.charging.time")}",
                Format = TypeFormatText.Paragraph,
                TextColor = new PropertyColorText(TypeColorText.Dark)
            });

            context.VisualTree.Content.Primary.Add(new ControlChart("chart")
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None),
                Labels = chartLabels,
                Data = new List<ControlChartDataset> { new ControlChartDataset() { Data = chartData, Title = this.I18N("vila:vila.history.label") } },
                Title = "",
                TitleX = this.I18N("vila:vila.charging.title.x"),
                TitleY = this.I18N("vila:vila.charging.title.y"),
                Styles = new List<string>() { "max-width: 80%;" },
                Minimum = 0
            });

            context.Uri.Display = id.Value;
        }
    }
}
