using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ViLa.App;
using ViLa.Model;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// REST endpoint returning history rows for the rest table control.
    /// Inherits the table-protocol from RestApiTable (Columns + Rows + Pagination).
    /// </summary>
    [Application<Application>]
    [Title("vila:vila.history.label")]
    public sealed class Table : RestApiTable<IndexMeasurementLog>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Provides a query context for the in-memory data store.
        /// </summary>
        /// <returns>An in-memory query context.</returns>
        protected override IQueryContext CreateContext()
        {
            return new InMemoryQueryContext();
        }

        /// <summary>
        /// Returns the column definitions for the history table.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The columns.</returns>
        protected override IEnumerable<RestApiTableColumn> RetrieveColums(IRequest request)
        {
            yield return new RestApiTableColumn()
            {
                Id = "from",
                Name = "From",
                Label = I18N.Translate(request, "vila:vila.history.from"),
                Icon = new IconPlay().ToString(),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "till",
                Name = "Till",
                Label = I18N.Translate(request, "vila:vila.history.till"),
                Icon = new IconStop().ToString(),
                Visible = false
            };

            yield return new RestApiTableColumn()
            {
                Id = "power",
                Name = "Power",
                Label = I18N.Translate(request, "vila:vila.history.power"),
                Icon = new IconBolt().ToString(),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "cost",
                Name = "Cost",
                Label = I18N.Translate(request, "vila:vila.history.cost"),
                Icon = new IconEuroSign().ToString(),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "vehicle",
                Name = "Vehicle",
                Label = I18N.Translate(request, "vila:vila.vehicles.vehicle"),
                Icon = new IconCar().ToString(),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "tag",
                Name = "tag",
                Label = I18N.Translate(request, "vila:vila.history.tag"),
                Icon = new IconTag().ToString(),
                Template = new RestApiTableColumnTemplateTag(true, TypeColorTag.Default, I18N.Translate(request, "vila:vila.history.tag")),
                Visible = true
            };
        }

        /// <summary>
        /// Returns the rows from the in-memory measurement log list.
        /// </summary>
        /// <param name="query">The query (filters, sort, paging applied).</param>
        /// <param name="context">The query context.</param>
        /// <param name="columns">The visible columns.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The rows.</returns>
        protected override IEnumerable<RestApiTableRow> RetrieveRows(IQuery<IndexMeasurementLog> query, IQueryContext context, IEnumerable<RestApiTableColumn> columns, IRequest request)
        {
            // only the rows need the default chronological order; the total is order independent
            var logs = query.Apply(History().OrderByDescending(x => x.From).ThenBy(x => x.Log.ID)).AsEnumerable();

            var tagUri = RestUriHelper.GetUri<Tag>()?.ToString() ?? "/api/1/history/tag";

            return logs.Select(x => new RestApiTableRow
            {
                Id = x.Id.ToString(),
                Uri = request?.ApplicationContext == null ? null : WebExpress.WebCore.WebEx.ComponentHub.SitemapManager
                    .GetUri<WWW.Details.Index>(request.ApplicationContext, new WebExpress.WebCore.WebParameter.ParameterId(x.Id.ToString()))?.ToString(),
                RestApi = $"{tagUri}?id={x.Id}",
                Cells = new List<RestApiTableCell>
                {
                    new RestApiTableCell { Content = x.From.ToString("o") },
                    new RestApiTableCell { Content = x.Till.ToString("o") },
                    new RestApiTableCell { Content = (double.IsNaN(x.Power) || double.IsInfinity(x.Power) ? 0d : x.Power).ToString("0.00") },
                    new RestApiTableCell { Content = (double.IsNaN(x.Cost) || double.IsInfinity(x.Cost) ? 0d : x.Cost).ToString("0.00") },
                    CreateVehicleCell(x.Log, request),
                    new RestApiTableCell
                    {
                        Type = "vila-tags",
                        Content = BillingPeriod.GetTags(x.Log).Select(tag => new { label = tag, color = ViewModel.Instance.GetColor(tag) }).ToArray()
                    }
                }
            });
        }

        /// <summary>
        /// Counts matching measurements before the table applies its page limits.
        /// </summary>
        /// <param name="query">The filtered query.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The matching measurement count.</returns>
        protected override int RetrieveTotal(IQuery<IndexMeasurementLog> query, IQueryContext context, IRequest request)
        {
            return query.Apply(History()).Count();
        }

        /// <summary>
        /// Builds the unfiltered history source; search, WQL and quickfilters arrive through the query.
        /// </summary>
        /// <returns>The measurements available to the table query.</returns>
        private static IQueryable<IndexMeasurementLog> History()
        {
            return ViewModel.Instance.GetHistoryMeasurementLogs().Select(log => new IndexMeasurementLog { Log = log }).AsQueryable();
        }

        /// <summary>
        /// Applies the basic search text. The base class only calls this when no WQL expression is
        /// given, so the two searches never stack. The matches are resolved once here, because the
        /// query is evaluated both for the total and for the page rows.
        /// </summary>
        /// <param name="search">The basic search text.</param>
        /// <param name="query">The current query.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The filtered query.</returns>
        protected override IQuery<IndexMeasurementLog> Filter(string search, IQuery<IndexMeasurementLog> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }
            var culture = request?.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
            var vehicles = HistorySearch.VehicleNames();
            var ids = ViewModel.Instance.GetHistoryMeasurementLogs()
                .Where(log => HistorySearch.Matches(log, search, culture, vehicles))
                .Select(log => log.ID)
                .ToHashSet(StringComparer.Ordinal);
            return query.Where(x => ids.Contains(x.Log.ID));
        }

        /// <summary>
        /// Restricts the history to measurements with a selected tag.
        /// </summary>
        /// <param name="filters">The selected tag labels.</param>
        /// <param name="query">The current query.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The filtered query.</returns>
        protected override IQuery<IndexMeasurementLog> Filter(IEnumerable<string> filters, IQuery<IndexMeasurementLog> query, IRequest request)
        {
            var tags = filters.ToHashSet(StringComparer.OrdinalIgnoreCase);
            return tags.Count == 0 ? query : query.Where(x => BillingPeriod.GetTags(x.Log).Any(tags.Contains));
        }

        /// <summary>
        /// Renders a vehicle badge using the catalog color and its contrasting foreground.
        /// </summary>
        /// <param name="log">The measurement carrying the vehicle assignment.</param>
        /// <param name="request">The request providing the display language.</param>
        /// <returns>The table cell containing the encoded badge.</returns>
        private static RestApiTableCell CreateVehicleCell(MeasurementLog log, IRequest request)
        {
            var vehicle = ViewModel.Instance.Vehicles.GetVehicle(log.VehicleId);
            var assigned = !string.IsNullOrEmpty(log.VehicleId);
            var label = assigned ? vehicle?.Name ?? log.VehicleName ?? log.VehicleId
                : I18N.Translate(request, "vila:vila.vehicles.badge.unassigned");
            var color = assigned ? vehicle?.Color ?? Vehicle.DefaultColor(log.VehicleId) : "#6c757d";
            var style = new PropertyColorBackgroundBadge(color).ToStyle();
            return new RestApiTableCell
            {
                Html = true,
                Content = $"<span class='badge' style='{style}'>{WebUtility.HtmlEncode(label)}</span>"
            };
        }
    }
}
