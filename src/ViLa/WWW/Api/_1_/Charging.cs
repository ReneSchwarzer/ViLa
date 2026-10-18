using System.Globalization;
using System.Text.Json;
using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_
{
    /// <summary>
    /// Returns live charging metrics without response caching.
    /// </summary>
    [Application<Application>]
    public sealed class Charging : IRestApi
    {
        /// <summary>
        /// Creates the current localized charging response.
        /// </summary>
        /// <param name="request">The request supplying the preferred culture.</param>
        /// <returns>The JSON snapshot with caching disabled.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Process(IRequest request)
        {
            var response = new ResponseOK()
            {
                Content = JsonSerializer.Serialize(ChargingSnapshot.Create(ViewModel.Instance,
                    request?.Culture ?? CultureInfo.CurrentCulture))
            };
            response.Header.ContentType = "application/json; charset=utf-8";
            response.Header.CacheControl = "no-store";
            return response;
        }
    }
}
