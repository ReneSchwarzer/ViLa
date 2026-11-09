using System;
using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebUI.WebControl;

namespace ViLa.WWW.History
{
    /// <summary>
    /// Organizes completed measurements into vehicle assignment columns.
    /// </summary>
    [Title("vila:vila.assign.title")]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class Assignments : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Builds the vehicle assignment board.
        /// </summary>
        /// <param name="renderContext">The context providing the request and its culture.</param>
        /// <param name="visualTree">The page tree receiving the assignment controls.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.AddPrimary(BuildBoard(renderContext, false));
        }

        /// <summary>
        /// Builds the assignment board for either its standalone route or the history view.
        /// </summary>
        /// <param name="renderContext">The request providing the search and paging state.</param>
        /// <param name="embedded">Whether the board is part of the history view.</param>
        /// <returns>The board and its search and persistence controls.</returns>
        internal static ControlPanel BuildBoard(IRenderContext renderContext, bool embedded)
        {
            var request = renderContext.Request;
            var query = request.GetParameter("q")?.Value?.Trim() ?? string.Empty;
            var search = request.GetParameter("search")?.Value?.Trim() ?? string.Empty;
            var page = request.GetParameter("page")?.Value ?? "1";
            var panel = new ControlPanel { Classes = new[] { embedded ? "vila-history-assignment" : "vila-vehicle-page" } };
            var serviceUri = WebEx.ComponentHub.SitemapManager.GetUri<global::ViLa.WWW.Api._1_.History.Assignments>(request.ApplicationContext)
                + "?q=" + Uri.EscapeDataString(query) + "&search=" + Uri.EscapeDataString(search)
                + "&page=" + Uri.EscapeDataString(page) + "&embedded=" + embedded.ToString().ToLowerInvariant();
            var board = new ControlVehicleAssignmentBoard("vehicle-assignment-board")
            {
                ServiceFactory = _ => new WebExpress.WebApp.WebData.DataServiceDescriptor("data")
                {
                    BaseUri = serviceUri,
                    Method = "GET",
                    UpdateMethod = "PUT"
                }
            };

            panel.Add(new ControlText { Text = _ => "vila:vila.assign.help" });
            if (!embedded)
            {
                panel.Add(new ControlWqlSearch("assignment-search", WebEx.ComponentHub.SitemapManager
                    .GetUri<WWW.Api._1_.History.Wql>(request.ApplicationContext)?.ToString()));
            }
            var host = new ControlPanel { Classes = new[] { "vila-assignment-board" } };
            var routeId = Guid.Empty.ToString();
            var detailUri = WebEx.ComponentHub.SitemapManager.GetUri<Details.Index>(
                request.ApplicationContext, new ParameterId(routeId)).ToString().Replace(routeId, "{id}");
            host.Add(new ControlMasterDetail("vehicle-assignment-detail", board)
            {
                DetailUriTemplate = _ => detailUri,
                DetailVisible = _ => false,
                Reveal = _ => TypeMasterDetailReveal.DoubleClick,
                MasterInitialSize = _ => 62,
                Fill = _ => true,
                Detail = new ControlFrame("vehicle-measurement-frame")
                {
                    Selector = _ => "#wx-content-main"
                }
            });
            panel.Add(host);
            return panel;
        }

    }
}
