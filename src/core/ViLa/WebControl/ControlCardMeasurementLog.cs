using ViLa.Model;
using WebExpress.Html;
using WebExpress.UI.WebControl;

namespace ViLa.WebControl
{
    public class ControlCardMeasurementLog : ControlCardCounter
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
        public ControlCardMeasurementLog(string id = null)
            : base(id)
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = MeasurementLog?.From.ToString(context.Culture.DateTimeFormat.ShortDatePattern) +
            new ControlText()
            {
                Text = MeasurementLog?.From.ToString(context.Culture.DateTimeFormat.ShortTimePattern) + " - " + MeasurementLog?.Till.ToString(context.Culture.DateTimeFormat.ShortTimePattern) + " Uhr",
                Format = TypeFormatText.Small
            }.Render(context) +
            new HtmlElementTextSemanticsBr() +
            new ControlLink()
            {
                Text = "Details",
                Uri = context.Page.Uri.Root.Append(MeasurementLog.ID)
            }.Render(context);
            Value = $"{ string.Format("{0:F2} kWh", MeasurementLog?.Power) } / { string.Format("{0:F2} {1}", MeasurementLog?.Cost, ViewModel.Instance.Settings.Currency) }";
            Icon = new PropertyIcon(TypeIcon.TachometerAlt);
            TextColor = new PropertyColorText(TypeColorText.Default);
            BackgroundColor = new PropertyColorBackground(TypeColorBackground.Light);
            Progress = (int)MeasurementLog?.Power;

            return base.Render(context);
        }
    }
}
