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
    [Section(Section.AppSettingsPrimary)]
    [Application("ViLa")]
    public sealed class ComponentHeaderSettings : ControlDropdownItemLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentHeaderSettings()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public void Initialization(IComponentContext context)
        {
            TextColor = new PropertyColorText(TypeColorText.Dark);
            Text = "vila:vila.setting.label";
            Icon = new PropertyIcon(TypeIcon.Cog);
            Uri = new UriResource(context.Module.ContextPath, "settings");
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Active = context.Page is IPageSettings ? TypeActive.Active : TypeActive.None;

            return base.Render(context);
        }

    }
}
