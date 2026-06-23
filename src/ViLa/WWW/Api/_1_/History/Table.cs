using System;
using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

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
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "till",
                Name = "Till",
                Label = I18N.Translate(request, "vila:vila.history.till"),
                Visible = false
            };

            yield return new RestApiTableColumn()
            {
                Id = "power",
                Name = "Power",
                Label = I18N.Translate(request, "vila:vila.history.power"),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "cost",
                Name = "Cost",
                Label = I18N.Translate(request, "vila:vila.history.cost"),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "tag",
                Name = "Tag",
                Label = I18N.Translate(request, "vila:vila.history.tag"),
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
            var logs = ViewModel.Instance
                .GetHistoryMeasurementLogs()
                .OrderByDescending(x => x.From)
                .Select(x => new IndexMeasurementLog { Log = x })
                .ToList();

            return logs.Select(x => new RestApiTableRow
            {
                Id = x.Id.ToString(),
                Cells = new List<RestApiTableCell>
                {
                    new RestApiTableCell { Content = x.From.ToString("o") },
                    new RestApiTableCell { Content = x.Till.ToString("o") },
                    new RestApiTableCell { Content = (double.IsNaN(x.Power) || double.IsInfinity(x.Power) ? 0d : x.Power).ToString("0.00") },
                    new RestApiTableCell { Content = (double.IsNaN(x.Cost) || double.IsInfinity(x.Cost) ? 0d : x.Cost).ToString("0.00") },
                    new RestApiTableCell { Content = x.Tag ?? string.Empty }
                }
            });
        }
    }
}
