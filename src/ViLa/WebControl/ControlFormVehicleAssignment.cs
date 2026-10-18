using System.Linq;
using System.Net;
using ViLa.Model;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Assigns a completed session to a vehicle or explicitly reruns recognition.
    /// </summary>
    public sealed class ControlFormVehicleAssignment : ControlForm
    {
        private readonly string _measurementId;
        private readonly ControlFormItemInputCombo _vehicle = new ControlFormItemInputCombo("measurement_vehicle");

        /// <summary>
        /// Initializes the assignment editor for a completed measurement.
        /// </summary>
        /// <param name="log">The completed measurement to assign.</param>
        public ControlFormVehicleAssignment(MeasurementLog log)
            : base("vehicle_assignment")
        {
            _measurementId = log.ID;
            _vehicle.Label = _ => "vila:vila.vehicles.assignment";
            _vehicle.Help = _ => "vila:vila.vehicles.assignment.help";
            _vehicle.Add(new ControlFormItemInputComboItem { Value = _ => "auto", Text = _ => "vila:vila.vehicles.detect" });
            _vehicle.Add(new ControlFormItemInputComboItem { Value = _ => "none", Text = _ => "vila:vila.vehicles.none" });
            var vehicles = ViewModel.Instance.Vehicles.GetAll();
            foreach (var vehicle in vehicles.Where(x => x.Active || x.Id == log.VehicleId))
            {
                _vehicle.Add(new ControlFormItemInputComboItem { Value = _ => vehicle.Id, Text = _ => WebUtility.HtmlEncode(vehicle.Name) });
            }
            _vehicle.Initialize(args => args.Value.Text = log.VehicleAssignment == VehicleAssignment.Manual
                ? log.VehicleId ?? "none" : "auto");
            Add(_vehicle);
            AddPrimaryButton(new ControlFormItemButtonSubmit { Text = _ => "vila:vila.vehicles.assign" });
            ValidateForm += ValidateSelection;
            ProcessForm += SaveSelection;
        }

        /// <summary>
        /// Rejects unavailable vehicles and invalid request methods.
        /// </summary>
        /// <param name="args">The form context and validation collection.</param>
        private void ValidateSelection(ControlFormEventFormValidate args)
        {
            var selection = args.Context.GetValue<ControlFormInputValueString>(_vehicle)?.Text;
            var log = ViewModel.Instance.GetHistoryMeasurementLog(_measurementId);
            args.Add(args.Context.Request.Method != RequestMethod.POST || log == null, "vila:vila.vehicles.invalid");
            args.Add(selection != "auto" && selection != "none" && !ViewModel.Instance.Vehicles.GetAll()
                .Any(x => x.Id == selection && (x.Active || x.Id == log?.VehicleId)), "vila:vila.vehicles.missing");
        }

        /// <summary>
        /// Persists a validated selection with its manual or automatic origin.
        /// </summary>
        /// <param name="args">The submitted form values.</param>
        private void SaveSelection(ControlFormEventFormProcess args)
        {
            ViewModel.Instance.AssignVehicle(_measurementId, args.GetValue<ControlFormInputValueString>(_vehicle).Text);
        }
    }
}
