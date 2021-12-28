using ViLa.WebPage;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.Attribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.Uri;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebPage;

namespace ViLa.WebComponent
{
    [Section(Section.AppNavigationPrimary)]
    [Application("ViLa")]
    public sealed class ComponentHeaderHistory : ControlNavigationItemLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentHeaderHistory()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public void Initialization(IComponentContext context)
        {
            TextColor = new PropertyColorText(TypeColorText.Light);
            Text = "vila:vila.history.label";
            Icon = new PropertyIcon(TypeIcon.ChartBar);
            Uri = new UriResource(context.Module.ContextPath, "history");
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Active = context.Page is IPageHistory ? TypeActive.Active : TypeActive.None;

            return base.Render(context);
        }

    }
}
