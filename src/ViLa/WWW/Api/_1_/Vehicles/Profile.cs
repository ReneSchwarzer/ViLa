using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using ViLa.Model;
using WebExpress.WebCore;
using WebExpress.WebUI.WebNotification;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_.Vehicles
{
    /// <summary>
    /// Loads and saves the fields of the REST vehicle editor.
    /// </summary>
    [Application<Application>]
    public sealed class Profile : IRestApi
    {
        /// <summary>
        /// Loads a saved profile or the defaults for a new vehicle.
        /// </summary>
        /// <param name="request">The request containing the optional vehicle identifier.</param>
        /// <returns>The values keyed by their form field names.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(Request request)
        {
            var id = request.GetParameter("id")?.Value;
            var vehicle = ViewModel.Instance.Vehicles.GetVehicle(id);
            if (!string.IsNullOrEmpty(id) && vehicle == null)
            {
                return Json(new ResponseNotFound(), new { message = I18N.Translate(request, "vila:vila.vehicles.missing") });
            }
            return Json(new ResponseOK(), new { data = new
            {
                vehicle_name = vehicle?.Name ?? string.Empty,
                vehicle_color = vehicle?.Color ?? Vehicle.DefaultColor(null),
                vehicle_power = vehicle?.MaxPowerKw.ToString("0.###", request.Culture) ?? string.Empty,
                vehicle_automatic = (vehicle?.AutomaticAssignment ?? true) ? "true" : "false",
                vehicle_active = (vehicle?.Active ?? true) ? "true" : "false"
            } });
        }

        /// <summary>
        /// Validates and persists a profile using the submitted field values.
        /// </summary>
        /// <param name="request">The JSON request containing the profile fields.</param>
        /// <returns>A confirmation or localized field errors.</returns>
        [Method(RequestMethod.POST)]
        [Method(RequestMethod.PUT)]
        public IResponse Save(Request request)
        {
            var id = request.GetParameter("id")?.Value;
            if (request.Method == RequestMethod.PUT && ViewModel.Instance.Vehicles.GetVehicle(id) == null)
            {
                return Json(new ResponseNotFound(), new { message = I18N.Translate(request, "vila:vila.vehicles.missing") });
            }
            if (request.Method == RequestMethod.POST && !string.IsNullOrEmpty(id))
            {
                return Json(new ResponseBadRequest(), new { message = I18N.Translate(request, "vila:vila.vehicles.invalid") });
            }
            Dictionary<string, JsonElement> fields;
            try
            {
                fields = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Content ?? []) ?? [];
            }
            catch (JsonException)
            {
                return Json(new ResponseBadRequest(), new { message = I18N.Translate(request, "vila:vila.vehicles.invalid") });
            }
            var name = fields.GetValueOrDefault("vehicle_name").ToString();
            var color = fields.GetValueOrDefault("vehicle_color").ToString();
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            {
                errors["vehicle_name"] = I18N.Translate(request, "vila:vila.vehicles.name.invalid");
            }
            if (!Vehicle.IsValidColor(color))
            {
                errors["vehicle_color"] = I18N.Translate(request, "vila:vila.vehicles.color.invalid");
            }
            if (!Vehicle.TryParsePower(fields.GetValueOrDefault("vehicle_power").ToString(), request.Culture ?? CultureInfo.CurrentCulture, out var power))
            {
                errors["vehicle_power"] = I18N.Translate(request, "vila:vila.vehicles.power.invalid");
            }
            if (!bool.TryParse(fields.GetValueOrDefault("vehicle_automatic").ToString(), out var automatic))
            {
                errors["vehicle_automatic"] = I18N.Translate(request, "vila:vila.vehicles.invalid");
            }
            if (!bool.TryParse(fields.GetValueOrDefault("vehicle_active").ToString(), out var active))
            {
                errors["vehicle_active"] = I18N.Translate(request, "vila:vila.vehicles.invalid");
            }
            if (errors.Count > 0)
            {
                return Json(new ResponseBadRequest(), new { errors });
            }
            var saved = ViewModel.Instance.Vehicles.Save(id, name, power, automatic, active, color);
            WebEx.ComponentHub?.GetComponentManager<NotificationManager>()?.AddNotification(
                request.ApplicationContext, request, "vila:vila.vehicles.saved", 5000, type: TypeNotification.Success);
            return Json(new ResponseOK(), new { saved.Id, hideForm = true });
        }

        /// <summary>
        /// Writes a JSON response with the specified HTTP status.
        /// </summary>
        /// <param name="response">The response supplying the status code.</param>
        /// <param name="value">The JSON payload.</param>
        /// <returns>The completed response.</returns>
        private static IResponse Json(Response response, object value)
        {
            response.Content = JsonSerializer.SerializeToUtf8Bytes(value);
            return response.AddHeaderContentType("application/json");
        }
    }
}
