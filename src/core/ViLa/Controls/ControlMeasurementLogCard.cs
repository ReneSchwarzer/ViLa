using ViLa.Model;
using WebExpress.Html;
using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace ViLa.Controls
{
    public class ControlMeasurementLogCard : ControlCardCounter
    {
        /// <summary>
        /// Liefert oder setzt das Messprotokoll
        /// </summary>
        public MeasurementLog MeasurementLog { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="page">Die zugehörige Seite</param>
        /// <param name="id">Die ID</param>
        public ControlMeasurementLogCard(IPage page, string id = null)
            : base(page, id)
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode ToHtml()
        {
            Text = MeasurementLog?.From.ToString("dd.MM.yyyy") +
            new ControlText(Page)
            {
                Text = MeasurementLog?.From.ToString("HH:mm:ss") + " - " + MeasurementLog?.Till.ToString("HH:mm:ss") + " Uhr",
                Format = TypesTextFormat.Small
            }.ToHtml() +
            new HtmlElementBr() +
            new ControlLink(Page)
            {
                Text = "Details",
                Uri = Page.Uri.Root.Append(MeasurementLog.ID)
            }.ToHtml();
            Value = string.Format("{0:F2} kWh", MeasurementLog?.Power) + " / " + string.Format("{0:F2} €", MeasurementLog?.Cost);
            Icon = Icon.TachometerAlt;
            Color = TypesTextColor.White;
            Layout = TypesLayoutCard.Default;
            Progress = (int)MeasurementLog?.Power;

            return base.ToHtml();
        }
    }
}
