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
    [Section(Section.AppNavigationPreferences)]
    [Application("ViLa")]
    public sealed class ControlHeaderDashbord : ControlNavigationItemLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlHeaderDashbord()
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
            Text = context.I18N("vila.dashboard.label");
            Uri = context.Page.Uri.Root;
            Active = context.Page is IPageDashbord ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.TachometerAlt);

            return base.Render(context);
        }

    }
}
