using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Pagination sub-fragment for the history view. Hosts the pagination
    /// control bound to the table fragment via its content id.
    /// </summary>
    [Section<SectionViewFooterPrimary>]
    [Application<Application>]
    [Scope<FragmentHistoryView>]
    [Cache]
    public sealed class FragmentHistoryPaginationFragment : FragmentControlViewFooter
    {
        /// <summary>
        /// Content ID used by the binding source of the table control.
        /// </summary>
        public static readonly string ContentId = "id_history_paging";

        /// <summary>
        /// Gets the pagination control.
        /// </summary>
        public ControlPagination Pagination { get; } = new ControlPagination(ContentId)
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public FragmentHistoryPaginationFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Pagination);
        }
    }
}
