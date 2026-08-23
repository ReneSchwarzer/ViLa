using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Table sub-fragment for the history view. Binds the REST table control
    /// to the History.Table endpoint.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Application<Application>]
    [Scope<FragmentHistoryView>]
    [Cache]
    public sealed class FragmentHistoryTableFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the table control.
        /// </summary>
        public ControlDataTable Table { get; } = new ControlDataTable()
        {
            ServiceFactory = _ => DataServiceDescriptor.TableData(ViLa.App.RestUriHelper.GetUri<WWW.Api._1_.History.Table>()?.ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public FragmentHistoryTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTable();
            Title = _ => "vila:vila.history.label";

            Add(Table);
        }
    }
}
