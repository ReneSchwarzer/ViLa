using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebSitemap;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents the history content as a dynamically generated table.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.History.Index>]
    public sealed class FragmentContentHistory : FragmentControlPanel
    {
        private readonly IFragmentContext _fragmentContext;
        private readonly ISitemapManager _sitemapManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The fragment context.
        /// </param>
        /// <param name="sitemapManager">
        /// The sitemap manager.
        /// </param>
        public FragmentContentHistory(IFragmentContext fragmentContext, ISitemapManager sitemapManager)
            : base(fragmentContext)
        {
            _fragmentContext = fragmentContext;
            _sitemapManager = sitemapManager;
        }

        /// <summary>
        /// Renders the control into an HTML node.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree control.</param>
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            Clear();

            var table = new ControlTable();
            table.AddColumn("vila:vila.history.from", new IconCalendar());
            table.AddColumn("vila:vila.history.duration", new IconStopwatch());
            table.AddColumn("vila:vila.history.power", new IconTachometerAlt());
            table.AddColumn("vila:vila.history.cost", new IconEuroSign());
            table.AddColumn("vila:vila.history.tag", new IconTag());
            table.AddColumn(string.Empty);

            foreach (var measurementLog in ViewModel.Instance.GetHistoryMeasurementLogs().OrderByDescending(x => x.From))
            {
                var tag = measurementLog.Tag;
                var tagCell = new ControlTableCellPanel();
                tagCell.Add
                (
                    new ControlTag(string.IsNullOrWhiteSpace(tag) ? "-" : tag)
                    {
                        BackgroundColor = _ => new PropertyColorBackground(ViewModel.Instance.GetColor(tag))
                    }
                );

                var linkCell = new ControlTableCellPanel();
                linkCell.Add
                (
                    new ControlLink()
                    {
                        Text = _ => "vila:vila.details.label",
                        Uri = _ => _sitemapManager.GetUri<WWW.Details.Index>(_fragmentContext?.ApplicationContext, new ParameterId(measurementLog.ID))
                    }
                );

                table.AddRow
                (
                    new ControlTableCell()
                    {
                        Text = _ => measurementLog.From.ToString("yyyy-MM-dd HH:mm")
                    },
                    new ControlTableCell()
                    {
                        Text = _ => (measurementLog.Till - measurementLog.From).ToString()
                    },
                    new ControlTableCell()
                    {
                        Text = _ => string.Format("{0:0.00} kWh", measurementLog.Power)
                    },
                    new ControlTableCell()
                    {
                        Text = _ => string.Format("{0:0.00} {1}", measurementLog.Cost, ViewModel.Instance.Settings.Currency)
                    },
                    tagCell,
                    linkCell
                );
            }

            Add(table);

            return base.Render(renderContext, visualTree);
        }
    }
}
