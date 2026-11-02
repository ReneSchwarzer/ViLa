using System;
using ViLa.Model;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// Removes a completed measurement after confirmation.
    /// </summary>
    [Application<Application>]
    public sealed class Delete : IRestApi
    {
        /// <summary>
        /// Deletes the named measurement and returns the user to history.
        /// </summary>
        /// <param name="request">The submitted confirmation identifying the record.</param>
        /// <returns>A redirect after deletion or an error if the record could not be removed.</returns>
        [Method(RequestMethod.POST)]
        public IResponse Remove(IRequest request)
        {
            var target = request.GetParameter("id")?.Value;
            var confirmation = request.GetParameter("confirmation")?.Value;
            if (!Guid.TryParseExact(target, "D", out var id) || !string.Equals(target, confirmation, StringComparison.Ordinal))
            {
                return new ResponseBadRequest();
            }
            var model = ViewModel.Instance;
            var log = model.GetHistoryMeasurementLog(id.ToString());
            if (log == null)
            {
                return new ResponseNotFound();
            }
            lock (log)
            {
                model.RemoveHistoryMeasurementLog(log.ID);
                if (model.GetHistoryMeasurementLog(log.ID) != null)
                {
                    return new ResponseInternalServerError();
                }
            }
            return new ResponseSeeOther(WebEx.ComponentHub.SitemapManager.GetUri<WWW.History.Index>(request.ApplicationContext));
        }
    }
}
