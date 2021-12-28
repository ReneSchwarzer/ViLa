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
    [Section(Section.PropertySecondary)]
    [Application("ViLa")]
    [Context("details")]
    public sealed class ComponentPropertyDelete : ControlButtonLink, IComponent
    {
        /// <summary>
        /// Liefert den modalen Dialog zur Bestätigung der Löschaktion
        /// </summary>
        private ControlModalFormConfirmDelete ModalDlg = new ControlModalFormConfirmDelete("delete_btn")
        {
            Header = "vila:vila.delete.label",
            Content = new ControlFormularItemStaticText() { Text = "vila:vila.delete.description" }
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentPropertyDelete()
            : base("delete_btn")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public void Initialization(IComponentContext context)
        {
            Text = "vila:vila.delete.label";
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Danger);
            Icon = new PropertyIcon(TypeIcon.Trash);
            TextColor = new PropertyColorText(TypeColorText.Light);

            Modal = ModalDlg;

            ModalDlg.Confirm += OnConfirm;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Löschaktion bestätigt wurde
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnConfirm(object sender, FormularEventArgs e)
        {
            var id = e.Context.Request.GetParameter("id");
            ViewModel.Instance.RemoveHistoryMeasurementLog(id.Value);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            ModalDlg.RedirectUri = context.Uri.Take(-1);

            return base.Render(context);
        }
    }
}
