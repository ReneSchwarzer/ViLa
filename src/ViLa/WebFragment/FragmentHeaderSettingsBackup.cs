using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebSitemap;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Adds backup import and export to the application settings menu.
    /// </summary>
    [Order(40)]
    [Section<SectionAppSettingsPrimary>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class FragmentHeaderSettingsBackup : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes the localized backup navigation link.
        /// </summary>
        /// <param name="fragmentContext">The fragment registration context.</param>
        /// <param name="sitemapManager">The service resolving the backup settings route.</param>
        public FragmentHeaderSettingsBackup(IFragmentContext fragmentContext, ISitemapManager sitemapManager)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.backup.menu";
            Icon = _ => new IconDatabase();
            Uri = _ => sitemapManager.GetUri<WWW.Settings.Data>(fragmentContext.ApplicationContext);
        }
    }
}
