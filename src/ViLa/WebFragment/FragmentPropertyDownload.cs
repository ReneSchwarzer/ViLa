using WebExpress.WebApp.WebSection;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Adds the XML action to the measurement detail menu.
    /// </summary>
    [Section<SectionHeadlineMorePrimary>]
    [Application<Application>]
    [Scope<WWW.Details.Index>]
    [Order(1)]
    public sealed class FragmentPropertyDownload : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes the menu item for the current measurement.
        /// </summary>
        /// <param name="fragmentContext">The fragment registration context.</param>
        public FragmentPropertyDownload(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.details.xml";
            Icon = _ => new IconFileCode();
            Uri = context => WebEx.ComponentHub.SitemapManager.GetUri<WWW.Api._1_.History.Export>(context.Request.ApplicationContext)
                .Add(new UriQuery("id", context.Request.GetParameter<ParameterId>()?.Value), new UriQuery("format", "xml"));
        }
    }
}
