using System.Net;
using ViLa.Model;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Identifies a completed session by its assigned vehicle.
    /// </summary>
    internal sealed class ControlVehicleBadge : ControlBadge
    {
        /// <summary>
        /// Initializes a badge using the catalog name or the name recorded with the session.
        /// </summary>
        /// <param name="log">The completed measurement containing the vehicle assignment.</param>
        public ControlVehicleBadge(MeasurementLog log)
        {
            var id = log.VehicleId;
            var vehicle = ViewModel.Instance.Vehicles.GetVehicle(id);
            Value = context => string.IsNullOrEmpty(id)
                ? I18N.Translate(context, "vila:vila.vehicles.badge.unassigned")
                : WebUtility.HtmlEncode(vehicle?.Name ?? log.VehicleName ?? id);
            BackgroundColor = _ => new PropertyColorBackgroundBadge(string.IsNullOrEmpty(id)
                ? "#6c757d" : vehicle?.Color ?? Vehicle.DefaultColor(id));
        }
    }
}
