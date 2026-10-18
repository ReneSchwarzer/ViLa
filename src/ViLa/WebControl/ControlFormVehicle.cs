using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Edits a vehicle profile using the standard WebExpress form lifecycle.
    /// </summary>
    public sealed class ControlFormVehicle : ControlForm
    {
        private readonly string _vehicleId;
        private readonly ControlFormItemInputText _name = new ControlFormItemInputText("vehicle_name");
        private readonly ControlFormItemInputText _power = new ControlFormItemInputText("vehicle_power");
        private readonly ControlFormItemInputCombo _automatic = new ControlFormItemInputCombo("vehicle_automatic");
        private readonly ControlFormItemInputCombo _active = new ControlFormItemInputCombo("vehicle_active");

        /// <summary>
        /// Initializes a create or edit form for the selected vehicle.
        /// </summary>
        /// <param name="vehicle">The existing profile, or null to create a new profile.</param>
        public ControlFormVehicle(Vehicle vehicle)
            : base("vehicle_editor")
        {
            _vehicleId = vehicle?.Id;
            Classes = new[] { "vila-vehicle-form" };
            _name.Label = _ => "vila:vila.vehicles.name";
            _name.MaxLength = _ => 100;
            _name.Required = _ => true;
            _name.Initialize(args => args.Value.Text = vehicle?.Name ?? string.Empty);
            _power.Label = _ => "vila:vila.vehicles.power";
            _power.Help = _ => "vila:vila.vehicles.power.help";
            _power.Required = _ => true;
            _power.Initialize(args => args.Value.Text = vehicle?.MaxPowerKw.ToString("0.###", args.Context.Request.Culture ?? CultureInfo.CurrentCulture) ?? string.Empty);
            _automatic.Label = _ => "vila:vila.vehicles.automatic";
            _automatic.Add(Option("true", "vila:vila.vehicles.enabled"), Option("false", "vila:vila.vehicles.disabled"));
            _automatic.Initialize(args => args.Value.Text = (vehicle?.AutomaticAssignment ?? true) ? "true" : "false");
            _active.Label = _ => "vila:vila.vehicles.status";
            _active.Add(Option("true", "vila:vila.vehicles.active"), Option("false", "vila:vila.vehicles.inactive"));
            _active.Initialize(args => args.Value.Text = (vehicle?.Active ?? true) ? "true" : "false");
            _active.Help = _ => "vila:vila.vehicles.archive.help";
            Add(_name, _power, _automatic, _active);
            AddPrimaryButton(new ControlFormItemButtonSubmit() { Text = _ => "vila:vila.vehicles.save" });
            ValidateForm += ValidateProfile;
            ProcessForm += SaveProfile;
        }

        /// <summary>
        /// Validates posted values before modifying the catalog.
        /// </summary>
        /// <param name="args">The form context and validation result collection.</param>
        private void ValidateProfile(ControlFormEventFormValidate args)
        {
            var name = args.Context.GetValue<ControlFormInputValueString>(_name)?.Text;
            var power = args.Context.GetValue<ControlFormInputValueString>(_power)?.Text;
            var automatic = args.Context.GetValue<ControlFormInputValueString>(_automatic)?.Text;
            var active = args.Context.GetValue<ControlFormInputValueString>(_active)?.Text;
            args.Add(args.Context.Request.Method != RequestMethod.POST, "vila:vila.vehicles.invalid");
            args.Add(string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100, "vila:vila.vehicles.name.invalid");
            args.Add(!Vehicle.TryParsePower(power, args.Context.Request.Culture ?? CultureInfo.CurrentCulture, out _), "vila:vila.vehicles.power.invalid");
            args.Add(automatic != "true" && automatic != "false" || active != "true" && active != "false", "vila:vila.vehicles.invalid");
            args.Add(_vehicleId != null && !ViewModel.Instance.Vehicles.GetAll().Any(x => x.Id == _vehicleId), "vila:vila.vehicles.missing");
        }

        /// <summary>
        /// Saves validated vehicle values to the catalog.
        /// </summary>
        /// <param name="args">The submitted form values.</param>
        private void SaveProfile(ControlFormEventFormProcess args)
        {
            Vehicle.TryParsePower(args.GetValue<ControlFormInputValueString>(_power).Text,
                args.Context.Request.Culture ?? CultureInfo.CurrentCulture, out var power);
            ViewModel.Instance.Vehicles.Save(_vehicleId, args.GetValue<ControlFormInputValueString>(_name).Text, power,
                args.GetValue<ControlFormInputValueString>(_automatic).Text == "true",
                args.GetValue<ControlFormInputValueString>(_active).Text == "true");
        }

        /// <summary>
        /// Creates a localized select option.
        /// </summary>
        /// <param name="value">The submitted value.</param>
        /// <param name="label">The translation key for the option label.</param>
        /// <returns>The option control.</returns>
        private static ControlFormItemInputComboItem Option(string value, string label)
        {
            return new ControlFormItemInputComboItem { Value = _ => value, Text = _ => label };
        }
    }
}
