using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebSection;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Quickfilter sub-fragment for the history view. Binds the REST quickfilter
    /// control to the History.Quickfilter endpoint and exposes its
    /// content id for binding source references in the table.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    [Application<Application>]
    [Scope<FragmentHistoryView>]
    [Cache]
    public sealed class FragmentHistoryQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Content ID used by the binding source of the table control.
        /// </summary>
        public static readonly string ContentId = "id_history_quickfilter";

        /// <summary>
        /// Gets the quickfilter control.
        /// </summary>
        public ControlDataQuickfilter Quickfilter { get; } = new ControlDataQuickfilter(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(ViLa.App.RestUriHelper.GetUri<WWW.Api._1_.History.Quickfilter>()?.ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public FragmentHistoryQuickfilterFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Quickfilter);
        }
    }
}
