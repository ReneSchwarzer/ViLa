using ViLa.WebResource;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.AppHelpPrimary)]
    [Application("ViLa")]
    public sealed class ControlHeaderHelp : ControlDropdownItemLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlHeaderHelp()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            TextColor = new PropertyColorText(TypeColorText.Dark);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = context.I18N("vila.help.label");
            Uri = context.Page.Uri.Root.Append("help");
            Active = context.Page is IPageHelp ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.InfoCircle);

            return base.Render(context);
        }

    }
}
