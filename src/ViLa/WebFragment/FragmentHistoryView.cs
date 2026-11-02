using ViLa.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Container fragment for the history view. Hosts sub-fragments for
    /// search, quickfilter, table, and paging.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<WWW.History.Index>]
    public sealed class FragmentHistoryView : FragmentControlView
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public FragmentHistoryView(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Classes = new[] { "vila-history-view" };
            Add(new ControlHistoryViewItem("list", new IconList(), context => ControlHistoryViewItem.BuildMeasurements(context, false)));
            Add(new ControlHistoryViewItem("cards", new IconTableCells(), context => ControlHistoryViewItem.BuildMeasurements(context, true)));
            Add(new ControlHistoryViewItem("assignment", new IconTableColumns(), context => WWW.History.Assignments.BuildBoard(context, true)));
            Layout = _ => TypeLayoutView.ToggleGroup;
        }
    }
}
