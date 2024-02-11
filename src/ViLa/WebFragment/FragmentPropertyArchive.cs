using ViLa.Model;
using ViLa.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebAttribute;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebFragment
{
    [Section(Section.PropertyPrimary)]
    [Module<Module>]
    [Scope<PageDetails>]
    public sealed class FragmentPropertyArchive : FragmentControlButtonLink
    {
        /// <summary>
        /// Liefert den modalen Dialog zur Bestätigung der Löschaktion
        /// </summary>
        private ControlModalFormularConfirm ModalDlg { get; } = new ControlModalFormularConfirm("archive_btn")
        {
            Header = "vila:vila.archive.label",
            Content = new ControlFormItemStaticText() { Text = "vila:vila.archive.description" }
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentPropertyArchive()
            : base("archive_btn")
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
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
            Icon = new PropertyIcon(TypeIcon.Clock);
            TextColor = new PropertyColorText(TypeColorText.Light);

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
            var id = e.Context.Request.GetParameter("id");
            ViewModel.Instance.ArchiveHistoryMeasurementLog(id.Value);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = "vila:vila.archive.label";

            ModalDlg.RedirectUri = context.Uri.Take(-1);

            return base.Render(context);
        }
    }
}
