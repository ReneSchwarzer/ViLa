using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebSitemap;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a settings link item in the header dropdown menu.
    /// </summary>
    [Section<SectionAppSettingsPrimary>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class FragmentHeaderSettings : FragmentControlDropdownItemLink
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
        public FragmentHeaderSettings(IFragmentContext fragmentContext, ISitemapManager sitemapManager)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.setting.label";
            Uri = _ => sitemapManager.GetUri<WWW.Settings.General>(fragmentContext?.ApplicationContext);
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
