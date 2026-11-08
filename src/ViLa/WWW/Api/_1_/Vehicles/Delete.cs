using System.Collections.Generic;
using System.Text.Json;
using ViLa.Model;
using WebExpress.WebCore;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;
using WebExpress.WebUI.WebNotification;

namespace ViLa.WWW.Api._1_.Vehicles
{
    /// <summary>
    /// Deletes confirmed vehicle profiles while preserving measurement history.
    /// </summary>
    [Application<Application>]
    public sealed class Delete : IRestApi
    {
        /// <summary>
        /// Initializes an empty confirmation field for an existing vehicle.
        /// </summary>
        /// <param name="request">The request identifying the vehicle.</param>
        /// <returns>The empty field or a missing profile response.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(Request request)
        {
            return ViewModel.Instance.Vehicles.GetVehicle(request.GetParameter("id")?.Value) == null
                ? Json(new ResponseNotFound(), new { message = I18N.Translate(request, "vila:vila.vehicles.missing") })
                : Json(new ResponseOK(), new { data = new { confirmation = string.Empty } });
        }

        /// <summary>
        /// Removes a vehicle after validating its submitted name.
        /// </summary>
        /// <param name="request">The JSON request containing the identifier and confirmation.</param>
        /// <returns>A confirmation or a localized validation error.</returns>
        [Method(RequestMethod.POST)]
        public IResponse Remove(Request request)
        {
            Dictionary<string, JsonElement> fields;
            try
            {
                fields = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Content ?? []) ?? [];
            }
            catch (JsonException)
            {
                return Json(new ResponseBadRequest(), new { message = I18N.Translate(request, "vila:vila.vehicles.invalid") });
            }
            var id = request.GetParameter("id")?.Value;
            if (ViewModel.Instance.Vehicles.GetVehicle(id) == null)
            {
                return Json(new ResponseNotFound(), new { message = I18N.Translate(request, "vila:vila.vehicles.missing") });
            }
            if (!ViewModel.Instance.Vehicles.Delete(id, fields.GetValueOrDefault("confirmation").ToString()))
            {
                return Json(new ResponseBadRequest(), new { errors = new { confirmation = I18N.Translate(request, "vila:vila.vehicles.delete.invalid") } });
            }
            WebEx.ComponentHub?.GetComponentManager<NotificationManager>()?.AddNotification(
                request.ApplicationContext, request, "vila:vila.vehicles.deleted", 5000, type: TypeNotification.Success);
            return Json(new ResponseOK(), new { hideForm = true });
        }

        /// <summary>
        /// Serializes the response payload with its HTTP status.
        /// </summary>
        /// <param name="response">The response supplying the status code.</param>
        /// <param name="value">The payload to serialize.</param>
        /// <returns>The completed JSON response.</returns>
        private static IResponse Json(Response response, object value)
        {
            response.Content = JsonSerializer.SerializeToUtf8Bytes(value);
            return response.AddHeaderContentType("application/json");
        }
    }
}
