using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_
{
    /// <summary>
    /// REST api endpoint returning the list of active tags as a JSON payload.
    /// </summary>
    [Application<Application>]
    public sealed class Tags : IRestApi
    {
        /// <summary>
        /// Initializes the tag lookup endpoint.
        /// </summary>
        public Tags()
        {
        }

        /// <summary>
        /// Returns the distinct tags assigned to completed sessions.
        /// </summary>
        /// <param name="request">The incoming HTTP request.</param>
        /// <returns>The JSON tag list.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Process(IRequest request)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(ViewModel.Instance.Tags);
            var response = new ResponseOK()
            {
                Content = json
            };
            response.Header.ContentType = "application/json; charset=utf-8";
            return response;
        }
    }
}
