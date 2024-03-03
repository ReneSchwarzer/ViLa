using ViLa.Model;
using ViLa.WebPage;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace ViLa.WebFragment
{
    [Section(Section.HeadlineSecondary)]
    [Module<Module>]
    [Scope<PageLog>]
    public sealed class FragmentPropertyDebug : FragmentControlButtonLink
    {
        /// <summary>
        /// Liefert den modalen Dialog zur Bestätigung der Löschaktion
        /// </summary>
        private ControlModalFormularConfirm ModalDlg { get; } = new ControlModalFormularConfirm("debug")
        {
            Header = "vila:vila.debug.label",
            Content = new ControlFormItemStaticText() { Text = "vila:vila.debug.description" }
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentPropertyDebug()
            : base("debug_btn")
        {

        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IFragmentContext context, IPage page)
        {
            base.Initialization(context, page);

            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Warning);
            Icon = new PropertyIcon(TypeIcon.Bug);
            TextColor = new PropertyColorText(TypeColorText.Light);

            ModalDlg.ButtonColor = new PropertyColorButton(TypeColorButton.Warning);
            ModalDlg.Confirm += OnConfirm;

            Modal = new PropertyModal(TypeModal.Modal, ModalDlg);
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
