using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore;

using WebExpress.WebCore.WebMessage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Edits a vehicle profile using the standard WebExpress form lifecycle.
    /// </summary>
    public sealed class ControlFormVehicle : ControlDataForm
    {
        private readonly ControlFormItemInputColor _color = new ControlFormItemInputColor("vehicle_color");
        private readonly ControlFormItemInputText _name = new ControlFormItemInputText("vehicle_name");
        private readonly ControlFormItemInputText _power = new ControlFormItemInputText("vehicle_power");
        private readonly ControlFormItemInputCombo _automatic = new ControlFormItemInputCombo("vehicle_automatic");
        private readonly ControlFormItemInputCombo _active = new ControlFormItemInputCombo("vehicle_active");

        /// <summary>
        /// Initializes a create or edit form for the selected vehicle.
        /// </summary>
        /// <param name="vehicle">The existing identifier, or null to create a new profile.</param>
        public ControlFormVehicle(string vehicle)
            : base("vehicle_editor")
        {
            ItemId = _ => vehicle;
            Mode = _ => vehicle == null ? "new" : "edit";
            Method = _ => vehicle == null ? RequestMethod.POST : RequestMethod.PUT;
            ServiceFactory = context => DataServiceDescriptor.FormData(WebEx.ComponentHub.SitemapManager
                .GetUri<WWW.Api._1_.Vehicles.Profile>(context.Request.ApplicationContext).ToString());
            Classes = new[] { "vila-vehicle-form" };
            _name.Label = _ => "vila:vila.vehicles.name";
            _name.MaxLength = _ => 100;
            _name.Required = _ => true;
            _color.Label = _ => "vila:vila.vehicles.color";
            _power.Label = _ => "vila:vila.vehicles.power";
            _power.Help = _ => "vila:vila.vehicles.power.help";
            _power.Required = _ => true;
            _automatic.Label = _ => "vila:vila.vehicles.automatic";
            _automatic.Add(Option("true", "vila:vila.vehicles.enabled"), Option("false", "vila:vila.vehicles.disabled"));
            _active.Label = _ => "vila:vila.vehicles.status";
            _active.Add(Option("true", "vila:vila.vehicles.active"), Option("false", "vila:vila.vehicles.inactive"));
            _active.Help = _ => "vila:vila.vehicles.archive.help";
            Add(_name, _color, _power, _automatic, _active);
            AddPreferencesButton(new ControlFormItemButtonSubmit { Text = _ => "vila:vila.vehicles.save" });
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
