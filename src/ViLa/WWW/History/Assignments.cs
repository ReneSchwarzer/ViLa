using System;
using System.Globalization;
using System.Linq;
using System.Net;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebIndex.Wql;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebNotification;

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
        /// Builds the vehicle board and saves a card move from its expected source column.
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
        /// <param name="renderContext">The request providing the search and posted assignment.</param>
        /// <param name="embedded">Whether the board is part of the history view.</param>
        /// <returns>The board and its search and persistence controls.</returns>
        internal static ControlPanel BuildBoard(IRenderContext renderContext, bool embedded)
        {
            var request = renderContext.Request;
            var model = ViewModel.Instance;
            var vehicles = model.Vehicles.GetAll();
            var query = request.GetParameter("q")?.Value?.Trim() ?? string.Empty;
            var search = request.GetParameter("search")?.Value?.Trim() ?? string.Empty;
            var page = int.TryParse(request.GetParameter("page")?.Value, out var requested) ? Math.Max(1, requested) : 1;
            var uri = WebEx.ComponentHub.SitemapManager.GetUri<Assignments>(request.ApplicationContext);
            var panel = new ControlPanel { Classes = new[] { embedded ? "vila-history-assignment" : "vila-vehicle-page" } };
            if (embedded)
            {
                uri = WebEx.ComponentHub.SitemapManager.GetUri<Index>(request.ApplicationContext);
            }
            if (request.Method == RequestMethod.POST)
            {
                var target = request.GetParameter("target")?.Value;
                if (model.TryMoveMeasurement(request.GetParameter("measurement")?.Value,
                    request.GetParameter("source")?.Value, target))
                {
                    WebEx.ComponentHub.GetComponentManager<NotificationManager>()?.AddNotification(
                        request.ApplicationContext, request, "vila:vila.assign.saved", 5000, type: TypeNotification.Success);
                    throw new RedirectException(new UriEndpoint(uri + (query.Length > 0 ? "?q=" + Uri.EscapeDataString(query) : "?search=" + Uri.EscapeDataString(search))
                        + (embedded && page > 1 ? "&page=" + page : string.Empty)));
                }
                // inactive cars are shown as columns but cannot receive measurements; say so instead of
                // reporting a concurrent change
                var inactive = vehicles.Any(vehicle => vehicle.Id == target && !vehicle.Active);
                panel.Add(new ControlAlert { Text = _ => inactive ? "vila:vila.assign.inactive" : "vila:vila.assign.stale" });
            }

            var culture = request.Culture ?? CultureInfo.CurrentCulture;
            var board = new ControlKanban("vehicle-assignment-board");
            board.Add(new VehicleBoardColumn("none", "vila:vila.vehicles.badge.unassigned", "#6c757d"));
            foreach (var vehicle in vehicles)
            {
                var title = vehicle.Active ? vehicle.Name : $"{vehicle.Name} ({I18N.Translate(renderContext, "vila:vila.vehicles.inactive")})";
                board.Add(new VehicleBoardColumn(vehicle.Id, title, vehicle.Color));
            }
            var logs = model.GetHistoryMeasurementLogs().OrderByDescending(log => log.From).ThenBy(log => log.ID).ToArray();
            foreach (var missing in logs.Where(log => !string.IsNullOrEmpty(log.VehicleId))
                .GroupBy(log => log.VehicleId).Where(group => !vehicles.Any(vehicle => vehicle.Id == group.Key)))
            {
                board.Add(new VehicleBoardColumn(missing.Key, missing.First().VehicleName ?? missing.Key, Vehicle.DefaultColor(missing.Key)));
            }
            var filtered = HistorySearch.Apply(logs.Select(log => new IndexMeasurementLog { Log = log }).AsQueryable(), query, search, culture);
            if (embedded)
            {
                var size = ControlHistoryViewItem.PageSize;
                page = Math.Clamp(page, 1, Math.Max(1, (filtered.Count() + size - 1) / size));
                filtered = filtered.Skip((page - 1) * size).Take(size);
            }
            foreach (var item in filtered)
            {
                var log = item.Log;
                var title = $"{log.From.ToString("g", culture)} \u00B7 {log.FinalPower.ToString("N2", culture)} kWh";
                var duration = $"{(log.Till - log.From).TotalMinutes.ToString("0", culture)} min";
                var card = new ControlKanbanCard(log.ID)
                {
                    Title = _ => title,
                    ColumnId = _ => string.IsNullOrEmpty(log.VehicleId) ? "none" : log.VehicleId
                };
                card.Add(new ControlKanbanCardChip { Label = _ => duration });
                card.Add(new ControlKanbanCardChip
                {
                    Label = _ => I18N.Translate(renderContext, "vila:vila.charging.average",
                        log.AveragePowerKw?.ToString("N2", culture) ?? "\u2013")
                });
                foreach (var tag in BillingPeriod.GetTags(log))
                {
                    card.Add(new ControlKanbanCardChip
                    {
                        Label = _ => tag,
                        Color = _ => new MeasurementTagColor(model.GetColor(tag))
                    });
                }
                board.Add(card);
            }

            panel.Add(new ControlText { Text = _ => "vila:vila.assign.help" });
            if (!embedded)
            {
                panel.Add(new ControlWqlSearch("assignment-search", WebEx.ComponentHub.SitemapManager
                    .GetUri<WWW.Api._1_.History.Wql>(request.ApplicationContext)?.ToString()));
            }
            var host = new ControlPanel { Classes = new[] { "vila-assignment-board" } };
            host.Add(new ControlHtml
            {
                Html = _ => $"<form method='post' action='{WebUtility.HtmlEncode(uri.ToString())}' hidden>" +
                    $"<input name='page' value='{page}'>" +
                    $"<input name='search' value='{WebUtility.HtmlEncode(search)}'>" +
                    $"<input name='q' value='{WebUtility.HtmlEncode(query)}'><input name='source'>" +
                    "<input name='target'><input name='measurement'></form>"
            });
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

        /// <summary>Supplies the measurement label color to the board chip stylesheet.</summary>
        private sealed class MeasurementTagColor : PropertyColorBackgroundBadge
        {
            /// <summary>Initializes the chip with the color used in the history table.</summary>
            /// <param name="color">The automatically derived label color.</param>
            public MeasurementTagColor(string color) : base(color)
            {
            }

            /// <summary>Exposes the shared label color to the rendered chip.</summary>
            /// <returns>The CSS custom property for the label.</returns>
            public override string ToStyle() => "--vila-tag-color: " + UserColor + ";";
        }
    }
}
