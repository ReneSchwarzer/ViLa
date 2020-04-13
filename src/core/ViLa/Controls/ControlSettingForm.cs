using ViLa.Model;
using System;
using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace ViLa.Controls
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
        private ControlFormularItemTextBox MaxWattageCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt die maximale Ladezeit in h
        /// </summary>
        private ControlFormularItemTextBox MaxChargingTimeCtrl { get; set; }

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

            MaxWattageCtrl = new ControlFormularItemTextBox(this)
            {
                Name = "MaxWattageCtrl",
                Label = "Der maximale Stromverbrauch in kWh:"
            };

            MaxChargingTimeCtrl = new ControlFormularItemTextBox(this)
            {
                Name = "MaxChargingTime",
                Label = "Die maximale Ladedauer in h:"
            };

            Add(ImpulsePerkWhCtrl);
            Add(ElectricityPricePerkWhCtrl);
            Add(MaxWattageCtrl);
            Add(MaxChargingTimeCtrl);

            InitFormular += (s, e) =>
            {
                ImpulsePerkWhCtrl.Value = ViewModel.Instance.Settings.ImpulsePerkWh.ToString();
                ElectricityPricePerkWhCtrl.Value = ViewModel.Instance.Settings.ElectricityPricePerkWh.ToString();
                MaxWattageCtrl.Value = ViewModel.Instance.Settings.MaxWattage.ToString();
                MaxChargingTimeCtrl.Value = ViewModel.Instance.Settings.MaxChargingTime.ToString();
            };

            ProcessFormular += (s, e) =>
            {
                ViewModel.Instance.Settings.ImpulsePerkWh = Convert.ToInt32(ImpulsePerkWhCtrl.Value);
                ViewModel.Instance.Settings.ElectricityPricePerkWh = (float)Convert.ToDouble(ElectricityPricePerkWhCtrl.Value);
                ViewModel.Instance.Settings.MaxWattage = !string.IsNullOrWhiteSpace(MaxWattageCtrl.Value) ? Convert.ToInt32(MaxWattageCtrl.Value) : -1;
                ViewModel.Instance.Settings.MaxChargingTime = !string.IsNullOrWhiteSpace(MaxChargingTimeCtrl.Value) ? Convert.ToInt32(MaxChargingTimeCtrl.Value) : -1;
                ViewModel.Instance.SaveSettings();
            };

            ImpulsePerkWhCtrl.Validation += (s, e) =>
            {
                try
                {
                    Convert.ToInt32(e.Value);
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

            MaxWattageCtrl.Validation += (s, e) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var value = Convert.ToInt32(e.Value);

                        if (value < -1)
                        {
                            e.Results.Add(new ValidationResult()
                            {
                                Text = "Der Wert ist zu klein",
                                Type = TypesInputValidity.Error
                            });
                        }
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

            MaxChargingTimeCtrl.Validation += (s, e) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var value = Convert.ToInt32(e.Value);

                        if (value < -1)
                        {
                            e.Results.Add(new ValidationResult()
                            {
                                Text = "Der Wert ist zu klein",
                                Type = TypesInputValidity.Error
                            });
                        }
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
        }
    }
}
