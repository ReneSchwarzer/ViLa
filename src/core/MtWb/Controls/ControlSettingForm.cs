using MtWb.Model;
using System;
using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace MtWb.Controls
{
    public class ControlSettingForm : ControlPanelFormular
    {
        /// <summary>
        /// Liefert oder setzt die Impulse pro kWh
        /// </summary>
        private ControlFormularItemTextBox ImpulsePerkWhCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt den Strompreis pro kWh
        /// </summary>
        private ControlFormularItemTextBox ElectricityPricePerkWhCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt die maximale Leistung in kWh
        /// </summary>
        private ControlFormularItemTextBox MaxPowerCtrl { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlSettingForm(IPage page)
            : base(page, "settings")
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public void Init()
        {
            Name = "settings";
            EnableCancelButton = false;
            Class = "m-3";

            ImpulsePerkWhCtrl = new ControlFormularItemTextBox(this)
            {
                Name = "ImpulsePerkWhCtrl",
                Label = "Die Anzahl der Impulse pro kWh:"
            };

            ElectricityPricePerkWhCtrl = new ControlFormularItemTextBox(this)
            {
                Name = "ElectricityPricePerkWhCtrl",
                Label = "Der Strompreis in € pro kWh:"
            };

            MaxPowerCtrl = new ControlFormularItemTextBox(this)
            {
                Name = "MaxPowerCtrl",
                Label = "Die maximale Leistung in kWh:"
            };

            Add(ImpulsePerkWhCtrl);
            Add(ElectricityPricePerkWhCtrl);
            Add(MaxPowerCtrl);

            InitFormular += (s, e) =>
            {
                ImpulsePerkWhCtrl.Value = ViewModel.Instance.Settings.ImpulsePerkWh.ToString();
                ElectricityPricePerkWhCtrl.Value = ViewModel.Instance.Settings.ElectricityPricePerkWh.ToString();
                MaxPowerCtrl.Value = ViewModel.Instance.Settings.MaxPower.ToString();
            };

            ProcessFormular += (s, e) =>
            {
                ViewModel.Instance.Settings.ImpulsePerkWh = Convert.ToInt32(ImpulsePerkWhCtrl.Value);
                ViewModel.Instance.Settings.ElectricityPricePerkWh = (float)Convert.ToDouble(ElectricityPricePerkWhCtrl.Value);
                ViewModel.Instance.Settings.MaxPower = Convert.ToInt32(MaxPowerCtrl.Value);
                ViewModel.Instance.SaveSettings();
            };

            ImpulsePerkWhCtrl.Validation += (s, e) =>
            {
                try
                {

                }
                catch (Exception ex)
                {
                    e.Results.Add(new ValidationResult()
                    {
                        Text = ex.Message,
                        Type = TypesInputValidity.Error
                    });
                }
            };

            ElectricityPricePerkWhCtrl.Validation += (s, e) =>
            {
                try
                {
                    var value = Convert.ToDouble(e.Value);

                    if (value < 0)
                    {
                        e.Results.Add(new ValidationResult()
                        {
                            Text = "Der Strompreis darf nicht negativ sein",
                            Type = TypesInputValidity.Error
                        });
                    }
                }
                catch (Exception ex)
                {
                    e.Results.Add(new ValidationResult()
                    {
                        Text = ex.Message,
                        Type = TypesInputValidity.Error
                    });
                }
            };

            MaxPowerCtrl.Validation += (s, e) =>
            {
                try
                {

                }
                catch (Exception ex)
                {
                    e.Results.Add(new ValidationResult()
                    {
                        Text = ex.Message,
                        Type = TypesInputValidity.Error
                    });
                }
            };
        }
    }
}
