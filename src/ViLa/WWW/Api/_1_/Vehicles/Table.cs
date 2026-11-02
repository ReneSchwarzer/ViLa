using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;

namespace ViLa.WWW.Api._1_.Vehicles
{
    /// <summary>
    /// Supplies filtered and paged vehicle catalog rows.
    /// </summary>
    [Application<Application>]
    public sealed class Table : RestApiTable<Vehicle>
    {
        /// <summary>
        /// Filters the catalog by vehicle name.
        /// </summary>
        /// <param name="search">The entered name fragment.</param>
        /// <param name="query">The current vehicle query.</param>
        /// <param name="request">The requesting client.</param>
        /// <returns>The filtered query.</returns>
        protected override IQuery<Vehicle> Filter(string search, IQuery<Vehicle> query, IRequest request)
        {
            return string.IsNullOrWhiteSpace(search) ? query : query.WhereContainsIgnoreCase(x => x.Name, search);
        }

        /// <summary>
        /// Defines the vehicle columns and their localized titles.
        /// </summary>
        /// <param name="request">The request supplying the culture.</param>
        /// <returns>The catalog schema.</returns>
        protected override IEnumerable<RestApiTableColumn> RetrieveColums(IRequest request)
        {
            foreach (var (field, key) in new[] { ("Name", "name"), ("MaxPowerKw", "power"), ("Active", "status"), ("AutomaticAssignment", "automatic") })
            {
                yield return new RestApiTableColumn { Id = field, Name = field, Visible = true, Label = I18N.Translate(request, "vila:vila.vehicles." + key) };
            }
        }

        /// <summary>
        /// Counts the matching vehicles before paging.
        /// </summary>
        /// <param name="query">The filtered query.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The current request.</param>
        /// <returns>The number of matching profiles.</returns>
        protected override int RetrieveTotal(IQuery<Vehicle> query, IQueryContext context, IRequest request)
        {
            return query.Apply(ViewModel.Instance.Vehicles.GetAll().AsQueryable()).Count();
        }

        /// <summary>
        /// Returns the requested page and its edit actions.
        /// </summary>
        /// <param name="query">The query including paging.</param>
        /// <param name="context">The query context.</param>
        /// <param name="columns">The table schema.</param>
        /// <param name="request">The request supplying routing and culture.</param>
        /// <returns>The matching rows.</returns>
        protected override IEnumerable<RestApiTableRow> RetrieveRows(IQuery<Vehicle> query, IQueryContext context,
            IEnumerable<RestApiTableColumn> columns, IRequest request)
        {
            var editor = WebEx.ComponentHub.SitemapManager.GetUri<WWW.Vehicles.Editor>(request.ApplicationContext);
            foreach (var vehicle in query.Apply(ViewModel.Instance.Vehicles.GetAll().AsQueryable()))
            {
                yield return new RestApiTableRow
                {
                    Id = vehicle.Id,
                    Cells = [
                        new RestApiTableCell { Content = vehicle.Name },
                        new RestApiTableCell { Content = vehicle.MaxPowerKw.ToString("0.###", request.Culture) },
                        new RestApiTableCell { Content = I18N.Translate(request, vehicle.Active ? "vila:vila.vehicles.active" : "vila:vila.vehicles.inactive") },
                        new RestApiTableCell { Content = I18N.Translate(request, vehicle.AutomaticAssignment ? "vila:vila.vehicles.enabled" : "vila:vila.vehicles.disabled") }
                    ],
                    Options = [new RestApiOptionEdit(request) { PrimaryAction = new ActionModal("vehicle-edit", new UriEndpoint(editor + "?id=" + vehicle.Id)) }.ToJson()]
                };
            }
        }
    }
}
