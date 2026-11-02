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
    /// Adds the PDF action to the measurement detail menu.
    /// </summary>
    [Section<SectionHeadlineMorePrimary>]
    [Application<Application>]
    [Scope<WWW.Details.Index>]
    [Order(2)]
    public sealed class FragmentPropertyPdf : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes the menu item for the current measurement.
        /// </summary>
        /// <param name="fragmentContext">The fragment registration context.</param>
        public FragmentPropertyPdf(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.details.pdf";
            Icon = _ => new IconFilePdf();
            Uri = context => WebEx.ComponentHub.SitemapManager.GetUri<WWW.Api._1_.History.Export>(context.Request.ApplicationContext)
                .Add(new UriQuery("id", context.Request.GetParameter<ParameterId>()?.Value), new UriQuery("format", "pdf"));
        }
    }
}
