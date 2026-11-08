using System.Text.Json;
using ViLa.Model;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// Loads and saves the rich text description of a completed measurement.
    /// </summary>
    [Application<Application>]
    public sealed class Description : IRestApi
    {
        /// <summary>
        /// Loads the description when the editor opens.
        /// </summary>
        /// <param name="request">The request identifying the measurement.</param>
        /// <returns>The editor fields or a missing record response.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(Request request)
        {
            var log = ViewModel.Instance.GetHistoryMeasurementLog(request.GetParameter("id")?.Value);
            return log == null ? new ResponseNotFound() : Document(request, log.Description);
        }

        /// <summary>
        /// Persists the edited description with the measurement.
        /// </summary>
        /// <param name="request">The request containing the identifier and editor fields.</param>
        /// <returns>The saved editor fields or a validation error.</returns>
        [Method(RequestMethod.PUT)]
        public IResponse Update(Request request)
        {
            var log = ViewModel.Instance.GetHistoryMeasurementLog(request.GetParameter("id")?.Value);
            if (log == null)
            {
                return new ResponseNotFound();
            }
            string body;
            try
            {
                using var document = JsonDocument.Parse(request.Content ?? []);
                if (document.RootElement.ValueKind != JsonValueKind.Object ||
                    !document.RootElement.TryGetProperty("Body", out var field) || field.ValueKind != JsonValueKind.String)
                {
                    return new ResponseBadRequest();
                }
                body = field.GetString();
            }
            catch (JsonException)
            {
                return new ResponseBadRequest();
            }
            lock (log)
            {
                var original = log.Description;
                try
                {
                    log.Description = body;
                    ViewModel.Instance.UpdateMeasurementLog(log);
                }
                catch
                {
                    log.Description = original;
                    throw;
                }
            }
            return Document(request, body);
        }

        /// <summary>
        /// Returns the localized heading and saved body as editor fields.
        /// </summary>
        /// <param name="request">The request providing the display language.</param>
        /// <param name="body">The saved rich text.</param>
        /// <returns>The JSON document.</returns>
        private static IResponse Document(Request request, string body)
        {
            return new ResponseOK
            {
                Content = JsonSerializer.SerializeToUtf8Bytes(new
                {
                    Title = I18N.Translate(request, "vila:vila.description.title"),
                    Body = body ?? string.Empty
                })
            }.AddHeaderContentType("application/json");
        }
    }
}
