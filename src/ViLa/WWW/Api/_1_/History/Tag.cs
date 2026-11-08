using System;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// REST endpoint for updating and retrieving the tag of a history measurement log.
    /// Supports SmartEdit inline editing.
    /// </summary>
    [Application<Application>]
    public sealed class Tag : IRestApi
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tag()
        {
        }

        /// <summary>
        /// Updates the tag for a measurement log.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>HTTP response.</returns>
        [Method(RequestMethod.PUT)]
        [Method(RequestMethod.POST)]
        public IResponse Update(IRequest request)
        {
            var id = request?.GetParameter("id")?.Value;
            if (string.IsNullOrWhiteSpace(id) && request?.Parameters != null)
            {
                var idParam = request.Parameters.OfType<WebExpress.WebCore.WebParameter.Parameter>().FirstOrDefault(p => p.Key.EndsWith("_objectId_"));
                id = idParam?.Value;
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return new ResponseBadRequest();
            }

            var tagParam = request.GetParameter("tag") ?? request.GetParameter("Tag");
            var tagValue = tagParam?.Value;

            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id);
            if (measurementLog == null)
            {
                return new ResponseNotFound();
            }

            measurementLog.Tag = string.IsNullOrWhiteSpace(tagValue) ? string.Empty : tagValue.Trim();
            ViewModel.Instance.UpdateMeasurementLog(measurementLog);

            var response = new ResponseOK();
            response.Header.ContentType = "application/json; charset=utf-8";
            return response;
        }

        /// <summary>
        /// Returns the tag for a measurement log.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>HTTP response.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(IRequest request)
        {
            var id = request?.GetParameter("id")?.Value;
            if (string.IsNullOrWhiteSpace(id))
            {
                return new ResponseBadRequest();
            }

            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id);
            if (measurementLog == null)
            {
                return new ResponseNotFound();
            }

            var response = new ResponseOK
            {
                Content = System.Text.Json.JsonSerializer.Serialize(new
                {
                    id = measurementLog.ID,
                    tag = measurementLog.Tag ?? string.Empty
                })
            };
            response.Header.ContentType = "application/json; charset=utf-8";
            return response;
        }
    }
}
