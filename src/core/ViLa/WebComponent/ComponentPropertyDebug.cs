using ViLa.Model;
using WebExpress.WebAttribute;
using WebExpress.Html;
using WebExpress.UI.WebAttribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebApp.WebControl;
using WebExpress.WebPage;

namespace ViLa.WebComponent
{
    [Section(Section.HeadlineSecondary)]
    [Application("ViLa")]
    [Context("log")]
    public sealed class ComponentPropertyDebug : ComponentControlButtonLink
    {
        /// <summary>
        /// Liefert den modalen Dialog zur Bestätigung der Löschaktion
        /// </summary>
        private ControlModalFormConfirm ModalDlg = new ControlModalFormConfirm("debug")
        {
            Header = "vila:vila.debug.label",
            Content = new ControlFormularItemStaticText() { Text = "vila:vila.debug.description" }
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentPropertyDebug()
            : base("debug_btn")
        {
           
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IComponentContext context, IPage page)
        {
            base.Initialization(context, page);

            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Warning);
            Icon = new PropertyIcon(TypeIcon.Bug);
            TextColor = new PropertyColorText(TypeColorText.Light);

            ModalDlg.ButtonColor = new PropertyColorButton(TypeColorButton.Warning);
            ModalDlg.Confirm += OnConfirm; 

            Modal = ModalDlg;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Löschaktion bestätigt wurde
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnConfirm(object sender, FormularEventArgs e)
        {
            ViewModel.Instance.Settings.DebugMode = !ViewModel.Instance.Settings.DebugMode;
            ViewModel.Instance.SaveSettings();
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = ViewModel.Instance.Settings.DebugMode ? "vila:vila.log.off" : "vila:vila.log.on";

            ModalDlg.RedirectUri = context.Uri;

            return base.Render(context);
        }
    }
}
