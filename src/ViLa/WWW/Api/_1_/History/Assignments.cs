using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;
using WebExpress.WebUI.WebControl;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// Loads the vehicle assignment board and persists card moves without navigating the page.
    /// </summary>
    [Application<Application>]
    public sealed class Assignments : IRestApi
    {
        /// <summary>Returns vehicle columns and the requested page of measurement cards.</summary>
        /// <param name="request">The request providing filters, paging and culture.</param>
        /// <returns>The DataKanban board representation.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(IRequest request)
        {
            var model = ViewModel.Instance;
            var vehicles = model.Vehicles.GetAll();
            var culture = request.Culture ?? CultureInfo.CurrentCulture;
            var columns = new List<RestApiKanbanColumn>
            {
                new() { Id = "none", Label = I18N.Translate(request, "vila:vila.vehicles.badge.unassigned"), Color = "#6c757d" }
            };
            columns.AddRange(vehicles.Select(vehicle => new RestApiKanbanColumn
            {
                Id = vehicle.Id,
                Label = vehicle.Active ? vehicle.Name : $"{vehicle.Name} ({I18N.Translate(request, "vila:vila.vehicles.inactive")})",
                Color = vehicle.Color
            }));
            var logs = model.GetHistoryMeasurementLogs().OrderByDescending(log => log.From).ThenBy(log => log.ID).ToArray();
            columns.AddRange(logs.Where(log => !string.IsNullOrEmpty(log.VehicleId)).GroupBy(log => log.VehicleId)
                .Where(group => !vehicles.Any(vehicle => vehicle.Id == group.Key)).Select(group => new RestApiKanbanColumn
                {
                    Id = group.Key,
                    Label = group.First().VehicleName ?? group.Key,
                    Color = Vehicle.DefaultColor(group.Key)
                }));
            var filtered = HistorySearch.Apply(logs.Select(log => new IndexMeasurementLog { Log = log }).AsQueryable(),
                request.GetParameter("q")?.Value, request.GetParameter("search")?.Value, culture);
            if (request.GetParameter("embedded")?.Value == "true")
            {
                var size = ControlHistoryViewItem.PageSize;
                int.TryParse(request.GetParameter("page")?.Value, out var requested);
                var page = Math.Clamp(requested, 1, Math.Max(1, (filtered.Count() + size - 1) / size));
                filtered = filtered.Skip((page - 1) * size).Take(size);
            }
            var cards = filtered.AsEnumerable().Select(item =>
            {
                var log = item.Log;
                var chips = new List<RestApiKanbanCardChip>
                {
                    new() { Label = $"{(log.Till - log.From).TotalMinutes.ToString("0", culture)} min" },
                    new() { Label = I18N.Translate(request, "vila:vila.charging.average", log.AveragePowerKw?.ToString("N2", culture) ?? "\u2013") }
                };
                chips.AddRange(BillingPeriod.GetTags(log).Select(tag => new RestApiKanbanCardChip
                {
                    Label = tag,
                    Color = new PropertyColorBackgroundBadge(model.GetColor(tag))
                }));
                return new RestApiKanbanCard
                {
                    Id = log.ID,
                    Label = $"{log.From.ToString("g", culture)} \u00B7 {log.FinalPower.ToString("N2", culture)} kWh",
                    ColumnId = string.IsNullOrEmpty(log.VehicleId) ? "none" : log.VehicleId,
                    Footer = chips
                };
            }).ToArray();
            return new RestApiKanbanResult { Columns = columns, Cards = cards, Swimlanes = [] }.ToResponse();
        }

        /// <summary>Persists a move only while the measurement still belongs to the expected source.</summary>
        /// <param name="request">The JSON request containing the card, source and destination.</param>
        /// <returns>Success or a localized refusal that causes DataKanban to restore the persisted board.</returns>
        [Method(RequestMethod.PUT)]
        public IResponse Update(IRequest request)
        {
            try
            {
                using var document = JsonDocument.Parse((request as Request)?.Content ?? []);
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object
                    || !ReadString(root, "cardId", out var id)
                    || !ReadString(root, "source", out var source)
                    || !ReadString(root, "columnId", out var target)
                    || (root.TryGetProperty("action", out var action)
                        && (action.ValueKind != JsonValueKind.String || action.GetString() != "move")))
                {
                    return new ResponseBadRequest();
                }
                var model = ViewModel.Instance;
                var log = model.GetHistoryMeasurementLog(id);
                if (log == null)
                {
                    return new ResponseNotFound();
                }
                lock (log)
                {
                    if (source == target && (string.IsNullOrEmpty(log.VehicleId) ? "none" : log.VehicleId) == source)
                    {
                        return Json(new ResponseOK(), new { success = true });
                    }
                    if (!model.TryMoveMeasurement(id, source, target))
                    {
                        var key = model.Vehicles.GetAll().Any(vehicle => vehicle.Id == target && !vehicle.Active)
                            ? "vila:vila.assign.inactive" : "vila:vila.assign.stale";
                        return Json(new ResponseConflict(), new { message = I18N.Translate(request, key) });
                    }
                }
                return Json(new ResponseOK(), new { success = true });
            }
            catch (JsonException)
            {
                return new ResponseBadRequest();
            }
        }

        /// <summary>Reads a required nonempty string from a move payload.</summary>
        /// <param name="root">The payload object.</param>
        /// <param name="name">The required property name.</param>
        /// <param name="value">The parsed string.</param>
        /// <returns>Whether the property contains a usable string.</returns>
        private static bool ReadString(JsonElement root, string name, out string value)
        {
            value = root.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String ? property.GetString() : null;
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>Serializes an API result with its response status.</summary>
        /// <param name="response">The response carrying the HTTP status.</param>
        /// <param name="value">The JSON result.</param>
        /// <returns>The response with a JSON content type.</returns>
        private static IResponse Json(Response response, object value)
        {
            response.Content = JsonSerializer.SerializeToUtf8Bytes(value);
            return response.AddHeaderContentType("application/json");
        }
    }
}
