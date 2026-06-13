using ViLa.Model;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a control form for settings.
    /// </summary>
    public class ControlFormSetting : ControlForm
    {
        public ControlFormItemInputText ImpulsePerkWhCtrl { get; } = new ControlFormItemInputText("ImpulsePerkWhCtrl");
        public ControlFormItemInputText ElectricityPricePerkWhCtrl { get; } = new ControlFormItemInputText("ElectricityPricePerkWhCtrl");
        public ControlFormItemInputText MaxWattageCtrl { get; } = new ControlFormItemInputText("MaxWattageCtrl");
        public ControlFormItemInputText MinWattageCtrl { get; } = new ControlFormItemInputText("MinWattageCtrl");
        public ControlFormItemInputText MaxChargingTimeCtrl { get; } = new ControlFormItemInputText("MaxChargingTime");
        public ControlFormItemInputText CurrencyCtrl { get; } = new ControlFormItemInputText("currency");
        public ControlFormItemInputText BillingDayOffsetCtrl { get; } = new ControlFormItemInputText("BillingDayOffsetCtrl");

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary> 
        /// <param name="id">
        /// The identifier for the form.
        /// </param>
        public ControlFormSetting(string id = "settings")
            : base(id)
        {
            ImpulsePerkWhCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.ImpulsePerkWh.ToString());
            ElectricityPricePerkWhCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.ElectricityPricePerkWh.ToString());
            MaxWattageCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.MaxWattage.ToString());
            MinWattageCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.MinWattage.ToString());
            MaxChargingTimeCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.MaxChargingTime.ToString());
            CurrencyCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.Currency);
            BillingDayOffsetCtrl.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.BillingDayOffset.ToString());

            Add(ImpulsePerkWhCtrl);
            Add(ElectricityPricePerkWhCtrl);
            Add(MaxWattageCtrl);
            Add(MinWattageCtrl);
            Add(MaxChargingTimeCtrl);
            Add(CurrencyCtrl);
            Add(BillingDayOffsetCtrl);
            AddPrimaryButton
            (
                new ControlFormItemButtonSubmit()
            );
        }
    }
}
