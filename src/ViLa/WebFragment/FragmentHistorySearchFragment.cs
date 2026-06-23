using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Search sub-fragment for the history view. Hosts the WQL search control
    /// bound to the History.Wql REST endpoint.
    /// </summary>
    [Section<SectionViewHeaderPrimary>]
    [Application<Application>]
    [Scope<FragmentHistoryView>]
    [Cache]
    public sealed class FragmentHistorySearchFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Content ID used by the binding source of the table and pagination controls.
        /// </summary>
        public static readonly string ContentId = "id_history_search";

        /// <summary>
        /// Gets the search control.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch(ContentId)
        {
            RestUri = _ => ViLa.App.RestUriHelper.GetUri<WWW.Api._1_.History.Wql>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public FragmentHistorySearchFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Search);
        }
    }
}
