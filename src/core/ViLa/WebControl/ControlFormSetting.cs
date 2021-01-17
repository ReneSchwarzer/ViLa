using System;
using System.Collections.Generic;
using ViLa.Model;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.WebControl;

namespace ViLa.WebControl
{
    public class ControlFormSetting : ControlFormular
    {
        /// <summary>
        /// Liefert oder setzt die Impulse pro kWh
        /// </summary>
        private ControlFormularItemInputTextBox ImpulsePerkWhCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt den Strompreis pro kWh
        /// </summary>
        private ControlFormularItemInputTextBox ElectricityPricePerkWhCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt die maximale Leistung in kWh
        /// </summary>
        private ControlFormularItemInputTextBox MaxWattageCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt die minimale Leistung in kWh
        /// </summary>
        private ControlFormularItemInputTextBox MinWattageCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt die maximale Ladezeit in h
        /// </summary>
        private ControlFormularItemInputTextBox MaxChargingTimeCtrl { get; set; }

        /// <summary>
        /// Liefert oder setzt die Währung
        /// </summary>
        public ControlFormularItemInputTextBox Currency { get; set; }

        /// <summary>
        /// Bestimmt, ob die Messungen automatisch erfolgen sollen
        /// </summary>
        public ControlFormularItemInputCheckbox Auto { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlFormSetting()
            : base("settings")
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
            Classes = new List<string>(new[] { "m-3" });

            ImpulsePerkWhCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "ImpulsePerkWhCtrl",
                Label = "Die Anzahl der Impulse pro kWh:",
                Help = "Der Wert stammt vom Drehstromzähler."
            };

            ElectricityPricePerkWhCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "ElectricityPricePerkWhCtrl",
                Label = "Der Strompreis in € pro kWh:",
                Help = ""
            };

            MaxWattageCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "MaxWattageCtrl",
                Label = "Der maximale Stromverbrauch in kWh:",
                Help = "Abbruch des Ladevorganges erfolt, wenn vorgegebene Stromverbrauch überschritten wird. &le; 0 für kein Abbruch."
            };

            MinWattageCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "MinWattageCtrl",
                Label = "Die minimale aktuelle Ladeleistung in kWh:",
                Help = "Abbruch des Ladevorganges erfolt, wenn die minimale Ladeleistung unterschritten wird. Der Abbruch erfolgt nur, wenn der Gesammtstromverbrauch &ge; 0,5 kWh beträgt. -1 für kein Abbruch."
            };

            MaxChargingTimeCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "MaxChargingTime",
                Label = "Die maximale Ladedauer in h:",
                Help = "Abbruch des Ladevorganges erfolt, wenn die Ladedauer überschritten wird. &le; 0 für kein Abbruch."
            };

            Currency = new ControlFormularItemInputTextBox("currency")
            {
                Name = "currency",
                Label = "vila.setting.currency.label",
                Help = "vila.setting.currency.description",
                Icon = new PropertyIcon(TypeIcon.EuroSign),
                Format = TypesEditTextFormat.Default
            };

            Auto = new ControlFormularItemInputCheckbox("auto")
            {
                Name = "auto",
                //Label = "vila.setting.auto.label",
                Help = "vila.setting.auto.description",
                Description = "vila.setting.auto.label",
                //Icon = new PropertyIcon(TypeIcon.PlayCircle)
            };

            Add(ImpulsePerkWhCtrl);
            Add(ElectricityPricePerkWhCtrl);
            Add(MaxWattageCtrl);
            Add(MinWattageCtrl);
            Add(MaxChargingTimeCtrl);
            Add(Currency);
            Add(Auto);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            FillFormular += (s, e) =>
            {
                ImpulsePerkWhCtrl.Value = ViewModel.Instance.Settings.ImpulsePerkWh.ToString();
                ElectricityPricePerkWhCtrl.Value = ViewModel.Instance.Settings.ElectricityPricePerkWh.ToString();
                MaxWattageCtrl.Value = ViewModel.Instance.Settings.MaxWattage.ToString();
                MinWattageCtrl.Value = ViewModel.Instance.Settings.MinWattage.ToString();
                MaxChargingTimeCtrl.Value = ViewModel.Instance.Settings.MaxChargingTime.ToString();
                Currency.Value = ViewModel.Instance.Settings.Currency.ToString();
                Auto.Value = ViewModel.Instance.Settings.Auto ? "true" : "false";
            };

            ProcessFormular += (s, e) =>
            {
                ViewModel.Instance.Settings.ImpulsePerkWh = Convert.ToInt32(ImpulsePerkWhCtrl.Value);
                ViewModel.Instance.Settings.ElectricityPricePerkWh = (float)Convert.ToDouble(ElectricityPricePerkWhCtrl.Value);
                ViewModel.Instance.Settings.MaxWattage = !string.IsNullOrWhiteSpace(MaxWattageCtrl.Value) ? Convert.ToInt32(MaxWattageCtrl.Value) : -1;
                ViewModel.Instance.Settings.MinWattage = !string.IsNullOrWhiteSpace(MinWattageCtrl.Value) ? Convert.ToInt32(MinWattageCtrl.Value) : -1;
                ViewModel.Instance.Settings.MaxChargingTime = !string.IsNullOrWhiteSpace(MaxChargingTimeCtrl.Value) ? Convert.ToInt32(MaxChargingTimeCtrl.Value) : -1;
                ViewModel.Instance.Settings.Currency = string.IsNullOrWhiteSpace(Currency.Value) ? "€" : Currency.Value;
                ViewModel.Instance.Settings.Auto = Auto.Value != null && Auto.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
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

            MinWattageCtrl.Validation += (s, e) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var value = Convert.ToDouble(e.Value);

                        if (value < 0 && value != -1)
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

            Currency.Validation += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.Results.Add(new ValidationResult()
                        {
                            Text = context.I18N("vila.setting.currency.validation.null"),
                            Type = TypesInputValidity.Error
                        });
                    }
                    else if (e.Value.Length > 10)
                    {
                        e.Results.Add(new ValidationResult()
                        {
                            Text = context.I18N("vila.setting.currency.validation.tolong"),
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

            return base.Render(context);
        }
    }
}
