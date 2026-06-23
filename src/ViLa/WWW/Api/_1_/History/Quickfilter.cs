using System.Linq;
using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// REST endpoint providing the list of available tag values for the
    /// history quickfilter control.
    /// </summary>
    [Application<Application>]
    public sealed class Quickfilter : IRestApi
    {
        /// <summary>
        /// Process the GET request and return the tag list.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The HTTP response.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(IRequest request)
        {
            var response = new ResponseOK()
            {
                Content = System.Text.Json.JsonSerializer.Serialize(ViewModel.Instance.Tags)
            };
            response.Header.ContentType = "application/json; charset=utf-8";
            return response;
        }
    }
}
