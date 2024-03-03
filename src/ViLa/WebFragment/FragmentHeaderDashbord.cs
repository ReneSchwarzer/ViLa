using ViLa.WebPage;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace ViLa.WebFragment
{
    [Section(Section.AppNavigationPreferences)]
    [Module<Module>]
    [Cache]
    public sealed class FragmentHeaderDashbord : FragmentControlNavigationItemLink
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentHeaderDashbord()
            : base()
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

            TextColor = new PropertyColorText(TypeColorText.Light);
            Text = "vila:vila.dashboard.label";
            Icon = new PropertyIcon(TypeIcon.TachometerAlt);
            Uri = ComponentManager.SitemapManager.GetUri<PageDashboard>();
            Active = page is IPageDashbord ? TypeActive.Active : TypeActive.None;
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            return base.Render(context);
        }
    }
}
