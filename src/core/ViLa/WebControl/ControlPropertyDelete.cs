using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;
using WebExpress.WebApp.WebControl;

namespace ViLa.WebControl
{
    [Section(Section.PropertySecondary)]
    [Application("ViLa")]
    [Context("details")]
    public sealed class ControlPropertyDelete : ControlButtonLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyDelete()
            : base("delete_btn")
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Danger);
            Icon = new PropertyIcon(TypeIcon.Trash);
            TextColor = new PropertyColorText(TypeColorText.Light);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = context.I18N("vila.delete.label");

            var modal = new ControlModalFormConfirmDelete()
            {
                Header = context.Page.I18N("vila.delete.label"),
                Content = new ControlFormularItemStaticText() { Text = context.I18N("vila.delete.description") },
                RedirectUri = context.Uri.Take(-1)
            };

            modal.Confirm += (s, e) =>
            {
                var id = context.Page.GetParamValue("id");
                ViewModel.Instance.RemoveHistoryMeasurementLog(id);

                context.Page.Redirecting(context.Uri.Take(-1));
            };

            Modal = modal;

            return base.Render(context);
        }
    }
}
