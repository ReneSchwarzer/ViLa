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
    [Section(Section.AppNavigationPrimary)]
    [Application("ViLa")]
    public sealed class ControlHeaderHistory : ControlNavigationItemLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlHeaderHistory()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            TextColor = new PropertyColorText(TypeColorText.Light);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = context.I18N("vila.history.label");
            Uri = context.Page.Uri.Root.Append("history");
            Active = context.Page is IPageHistory ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.ChartBar);

            return base.Render(context);
        }

    }
}
