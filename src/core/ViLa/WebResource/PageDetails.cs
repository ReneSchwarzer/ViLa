using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebResource;

namespace ViLa.WebResource
{
    [ID("Details")]
    [Title("vila.details.label")]
    [SegmentGuid("id", "vila.details.id", SegmentGuidAttribute.Format.Simple)]
    [Path("/")]
    [Path("/History")]
    [Module("ViLa")]
    [Context("general")]
    [Context("details")]
    public sealed class PageDetails : PageTemplateWebApp
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
        public override void Initialization()
        {
            base.Initialization();

            Favicons.Add(new Favicon(Uri.Root.Append("/assets/img/Favicon.png").ToString(), TypeFavicon.PNG));
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            var id = GetParamValue("id");
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id);
            var chartLabels = measurementLog?.Measurements.Select(x => ((int)(x.MeasurementTimePoint - measurementLog.From).TotalMinutes).ToString()).ToArray();
            var chartData = measurementLog?.Measurements.Select(x => x.Power * 60).ToArray();

            Content.Primary.Add(new ControlText()
            {
                Text = id + ".xml",
                TextColor = new PropertyColorText(TypeColorText.Muted)
            });

            Content.Primary.Add(new ControlText()
            {
                Text = string.Format("{0:F2} kWh", measurementLog?.FinalPower) + " / " + string.Format("{0:F2} {1}", measurementLog?.FinalCost, ViewModel.Instance.Settings.Currency),
                Format = TypeFormatText.H1,
                TextColor = new PropertyColorText(TypeColorText.Primary)
            });

            Content.Primary.Add(new ControlText()
            {
                Text = $"{ measurementLog?.FinalFrom.ToString(Culture.DateTimeFormat.LongDatePattern) } - { measurementLog?.FinalTill.ToString(Culture.DateTimeFormat.LongDatePattern) } {this.I18N("vila.charging.time")} ",
                Format = TypeFormatText.Paragraph,
                TextColor = new PropertyColorText(TypeColorText.Dark)
            });

            Content.Primary.Add(new ControlChart("chart")
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None),
                Labels = chartLabels,
                Data = new List<ControlChartDataset> { new ControlChartDataset() { Data = chartData, Title = this.I18N("vila.history.label") } },
                Title = "",
                TitleX = this.I18N("vila.charging.title.x"),
                TitleY = this.I18N("vila.charging.title.y"),
                Styles = new List<string>() { "max-width: 80%;" },
                Minimum = 0
            });
        }
    }
}
