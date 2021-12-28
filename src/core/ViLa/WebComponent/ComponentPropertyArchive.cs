using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.Attribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebApp.WebControl;
using WebExpress.WebPage;

namespace ViLa.WebComponent
{
    [Section(Section.PropertyPrimary)]
    [Application("ViLa")]
    [Context("details")]
    public sealed class ComponentPropertyArchive : ControlButtonLink, IComponent
    {
        /// <summary>
        /// Liefert den modalen Dialog zur Bestätigung der Löschaktion
        /// </summary>
        private ControlModalFormConfirm ModalDlg = new ControlModalFormConfirm("archive_btn")
        {
            Header = "vila:vila.archive.label",
            Content = new ControlFormularItemStaticText() { Text = "vila:vila.archive.description" }
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentPropertyArchive()
            : base("archive_btn")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public void Initialization(IComponentContext context)
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
            Icon = new PropertyIcon(TypeIcon.Clock);
            TextColor = new PropertyColorText(TypeColorText.Light);

            Modal = ModalDlg;
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
