using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebSitemap;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a dashboard navigation link item in the header.
    /// </summary>
    [Section<SectionAppNavigationPreferences>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class FragmentHeaderDashbord : FragmentControlNavigationItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        /// <param name="sitemapManager">
        /// The sitemap manager used to resolve URIs of registered endpoints.
        /// </param>
        public FragmentHeaderDashbord(IFragmentContext fragmentContext, ISitemapManager sitemapManager)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.dashboard.label";
            Uri = _ => sitemapManager.GetUri<WWW.Index>(fragmentContext?.ApplicationContext);
            Icon = _ => new IconTachometerAlt();
            Active = renderContext => renderContext?.Endpoint is WWW.Index
                ? TypeActive.Active
                : TypeActive.None;
        }

        /// <summary>
        /// Renders the control into an HTML node.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree control.</param>
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
