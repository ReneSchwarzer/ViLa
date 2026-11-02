using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>
    /// Builds a history presentation from the measurements available in the current request.
    /// </summary>
    internal sealed class ControlHistoryViewItem : ControlViewItem
    {
        internal const int PageSize = 50;

        private readonly string _name;
        private readonly Func<IRenderControlContext, IControl> _build;

        /// <summary>
        /// Initializes a named history view.
        /// </summary>
        /// <param name="name">The localized presentation key.</param>
        /// <param name="icon">The switcher icon.</param>
        /// <param name="build">The factory for request specific content.</param>
        public ControlHistoryViewItem(string name, IIcon icon, Func<IRenderControlContext, IControl> build)
            : base("history-" + name)
        {
            Title = _ => "vila:vila.history.view." + name;
            Icon = _ => icon;
            _name = name;
            _build = build;
        }

        /// <summary>
        /// Renders a fresh presentation without retaining request data in the fragment.
        /// A request naming a single view (the "view" parameter, sent by the search refresh)
        /// builds only that view and leaves the other result panes empty.
        /// </summary>
        /// <param name="renderContext">The current rendering context.</param>
        /// <param name="visualTree">The history page tree.</param>
        /// <returns>The populated view item.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var item = new ControlViewItem(Id) { Title = Title, Icon = Icon };
            var results = new ControlPanel(Id + "-results");
            var requested = renderContext.Request.GetParameter("view")?.Value;
            if (string.IsNullOrEmpty(requested) || requested == _name)
            {
                results.Add(_build(renderContext));
            }
            item.Add(results);
            return item.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Builds a page of charging cards or a selectable list with measurement details.
        /// </summary>
        /// <param name="context">The request supplying routing and culture.</param>
        /// <param name="cards">Whether to render the charging cards.</param>
        /// <returns>The paginated measurement presentation.</returns>
        internal static IControl BuildMeasurements(IRenderControlContext context, bool cards)
        {
            var request = context.Request;
            var culture = request.Culture ?? CultureInfo.CurrentCulture;
            var wql = request.GetParameter("q")?.Value;
            var text = request.GetParameter("search")?.Value;
            var source = ViewModel.Instance.GetHistoryMeasurementLogs().OrderByDescending(log => log.From).ThenBy(log => log.ID)
                .Select(log => new IndexMeasurementLog { Log = log }).AsQueryable();
            var logs = HistorySearch.Apply(source, wql, text, culture).Select(item => item.Log).ToArray();
            if (logs.Length == 0)
            {
                return new ControlEmptyState { Message = _ => "vila:vila.history.empty" };
            }
            var pageCount = (logs.Length + PageSize - 1) / PageSize;
            int.TryParse(request.GetParameter("page")?.Value, out var requested);
            var page = Math.Clamp(requested, 1, pageCount);
            var panel = new ControlPanel();
            var pageLogs = logs.Skip((page - 1) * PageSize).Take(PageSize);
            if (cards)
            {
                var grid = new ControlGrid();
                foreach (var log in pageLogs)
                {
                    grid.Add(new ControlCardMeasurementLog { MeasurementLog = log, GridColumn = _ => new PropertyGrid(TypeDevice.Medium, 4) });
                }
                panel.Add(grid);
            }
            else
            {
                const string detailId = "history-list-detail";
                var list = new ControlList("history-measurement-list") { Selectable = _ => true };
                foreach (var log in pageLogs)
                {
                    var uri = WebEx.ComponentHub.SitemapManager.GetUri<WWW.Details.Index>(request.ApplicationContext, new ParameterId(log.ID));
                    list.Add(new ControlListItem("history-list-" + log.ID)
                    {
                        Text = _ => log.From.ToString("g", culture),
                        Description = _ => log.FinalPower.ToString("N2", culture) + " kWh",
                        PrimaryAction = _ => new ActionMasterDetail(detailId, uri, log.ID)
                    });
                }
                panel.Add(new ControlMasterDetail(detailId, list)
                {
                    Detail = new ControlFrame("history-list-frame") { Selector = _ => "#wx-content-main" }
                });
            }
            return panel;
        }
    }
}
