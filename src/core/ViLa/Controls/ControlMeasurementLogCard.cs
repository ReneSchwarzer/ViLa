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
        public ControlMeasurementLogCard(string id = null)
            : base(id)
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
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = MeasurementLog?.From.ToString("dd.MM.yyyy") +
            new ControlText()
            {
                Text = MeasurementLog?.From.ToString("HH:mm:ss") + " - " + MeasurementLog?.Till.ToString("HH:mm:ss") + " Uhr",
                Format = TypeFormatText.Small
            }.Render(context) +
            new HtmlElementTextSemanticsBr() +
            new ControlLink()
            {
                Text = "Details",
                Uri = context.Page.Uri.Root.Append(MeasurementLog.ID)
            }.Render(context);
            Value = string.Format("{0:F2} kWh", MeasurementLog?.Power) + " / " + string.Format("{0:F2} €", MeasurementLog?.Cost);
            Icon = new PropertyIcon(TypeIcon.TachometerAlt);
            TextColor = new PropertyColorText(TypeColorText.Default);
            BackgroundColor = new PropertyColorBackground(TypeColorBackground.Light);
            Progress = (int)MeasurementLog?.Power;

            return base.Render(context);
        }
    }
}
