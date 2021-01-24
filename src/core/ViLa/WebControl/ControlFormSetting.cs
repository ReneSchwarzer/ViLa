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
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Initialize(RenderContext context)
        {
            base.Initialize(context);

            Name = "settings";
            EnableCancelButton = false;
            Classes = new List<string>(new[] { "m-3" });

            ImpulsePerkWhCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "ImpulsePerkWhCtrl",
                Label = "vila.setting.form.impulseperkwhctrl.label",
                Help = "vila.setting.form.impulseperkwhctrl.description"
            };

            ElectricityPricePerkWhCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "ElectricityPricePerkWhCtrl",
                Label = string.Format(context.I18N("vila.setting.form.electricitypriceperkwhctrl.label"), ViewModel.Instance.Settings.Currency),
                Help = "vila.setting.form.electricitypriceperkwhctrl.description"
            };

            MaxWattageCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "MaxWattageCtrl",
                Label = "vila.setting.form.maxwattagectrl.label",
                Help = "vila.setting.form.maxwattagectrl.description"
            };

            MinWattageCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "MinWattageCtrl",
                Label = "vila.setting.form.minwattagectrl.label",
                Help = "vila.setting.form.minwattagectrl.description"
            };

            MaxChargingTimeCtrl = new ControlFormularItemInputTextBox()
            {
                Name = "MaxChargingTime",
                Label = "vila.setting.form.maxchargingtime.label",
                Help = "vila.setting.form.maxchargingtime.description"
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

            InitializeFormular += (s, e) =>
            {
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
                                Text = context.I18N("vila.setting.form.electricitypriceperkwhctrl.validation.negative"),
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
                                    Text = context.I18N("vila.setting.form.validation.low"),
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
                                    Text = context.I18N("vila.setting.form.validation.low"),
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
                                    Text = context.I18N("vila.setting.form.validation.low"),
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
            };

            return base.Render(context);
        }
    }
}
