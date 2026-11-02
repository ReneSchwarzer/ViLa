using ViLa.Model;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a fragment control panel for tag content within the details index view.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Details.Index>]
    public sealed class FragmentContentTag : FragmentControlPanel
    {
        /// <summary>
        /// Initializes the tag control for the selected measurement.
        /// </summary>
        /// <param name="fragmentContext">The fragment registration context.</param>
        public FragmentContentTag(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(new ControlDataTag("measurement-tags")
            {
                Value = context => ViewModel.Instance.GetHistoryMeasurementLog(context.Request.GetParameter<ParameterId>()?.Value) is { } log
                    ? BillingPeriod.GetTags(log) : [],
                ServiceFactory = context => DataServiceDescriptor.QueryData(WebEx.ComponentHub.SitemapManager
                    .GetUri<WWW.Api._1_.History.Labels._id_.Index>(context.Request.ApplicationContext,
                        new ParameterId(context.Request.GetParameter<ParameterId>().Value)).ToString())
            });
        }
    }
}
