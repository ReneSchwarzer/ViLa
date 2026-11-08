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
    /// Adds the vehicle catalog to the application settings menu.
    /// </summary>
    [Order(30)]
    [Section<SectionAppSettingsPrimary>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class FragmentHeaderVehicles : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes the localized vehicle navigation link.
        /// </summary>
        /// <param name="fragmentContext">The fragment registration context.</param>
        /// <param name="sitemapManager">The service resolving the catalog route.</param>
        public FragmentHeaderVehicles(IFragmentContext fragmentContext, ISitemapManager sitemapManager)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.vehicles.title";
            Uri = _ => sitemapManager.GetUri<WWW.Settings.Vehicles>(fragmentContext.ApplicationContext);
            Icon = _ => new IconCar();
        }
    }
}
