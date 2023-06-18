using System;
using System.Collections.Generic;
using ViLa.Model;
using WebExpress.Internationalization;
using WebExpress.UI.WebControl;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebControl
{
    public class ControlFormSetting : ControlForm
    {
        /// <summary>
        /// Liefert oder setzt die Impulse pro kWh
        /// </summary>
        private ControlFormItemInputTextBox ImpulsePerkWhCtrl { get; } = new ControlFormItemInputTextBox()
        {
            Name = "ImpulsePerkWhCtrl",
            Label = "vila:vila.setting.form.impulseperkwhctrl.label",
            Help = "vila:vila.setting.form.impulseperkwhctrl.description"
        };

        /// <summary>
        /// Liefert oder setzt den Strompreis pro kWh
        /// </summary>
        private ControlFormItemInputTextBox ElectricityPricePerkWhCtrl { get; } = new ControlFormItemInputTextBox()
        {
            Name = "ElectricityPricePerkWhCtrl",
            Help = "vila:vila.setting.form.electricitypriceperkwhctrl.description"
        };

        /// <summary>
        /// Liefert oder setzt die maximale Leistung in kWh
        /// </summary>
        private ControlFormItemInputTextBox MaxWattageCtrl { get; } = new ControlFormItemInputTextBox()
        {
            Name = "MaxWattageCtrl",
            Label = "vila:vila.setting.form.maxwattagectrl.label",
            Help = "vila:vila.setting.form.maxwattagectrl.description"
        };

        /// <summary>
        /// Liefert oder setzt die minimale Leistung in kWh
        /// </summary>
        private ControlFormItemInputTextBox MinWattageCtrl { get; } = new ControlFormItemInputTextBox()
        {
            Name = "MinWattageCtrl",
            Label = "vila:vila.setting.form.minwattagectrl.label",
            Help = "vila:vila.setting.form.minwattagectrl.description"
        };

        /// <summary>
        /// Liefert oder setzt die maximale Ladezeit in h
        /// </summary>
        private ControlFormItemInputTextBox MaxChargingTimeCtrl { get; } = new ControlFormItemInputTextBox()
        {
            Name = "MaxChargingTime",
            Label = "vila:vila.setting.form.maxchargingtime.label",
            Help = "vila:vila.setting.form.maxchargingtime.description"
        };

        /// <summary>
        /// Liefert oder setzt die Währung
        /// </summary>
        public ControlFormItemInputTextBox Currency { get; } = new ControlFormItemInputTextBox("currency")
        {
            Name = "currency",
            Label = "vila:vila.setting.currency.label",
            Help = "vila:vila.setting.currency.description",
            Icon = new PropertyIcon(TypeIcon.EuroSign),
            Format = TypesEditTextFormat.Default
        };

        ///// <summary>
        ///// Bestimmt, ob die Messungen automatisch erfolgen sollen
        ///// </summary>
        //public ControlFormItemInputCheckbox Auto = new ControlFormItemInputCheckbox("auto")
        //{
        //    Name = "auto",
        //    //Label = "vila.setting.auto.label",
        //    Help = "vila:vila.setting.auto.description",
        //    Description = "vila:vila.setting.auto.label",
        //    //Icon = new PropertyIcon(TypeIcon.PlayCircle)
        //};

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlFormSetting()
            : base("settings")
        {
            Name = "settings";
            Classes = new List<string>(new[] { "m-3" });

            Add(ImpulsePerkWhCtrl);
            Add(ElectricityPricePerkWhCtrl);
            Add(MaxWattageCtrl);
            Add(MinWattageCtrl);
            Add(MaxChargingTimeCtrl);
            Add(Currency);
            //Add(Auto);

            FillFormular += OnFillFormular;
            ProcessFormular += OnProcessFormular;

            ImpulsePerkWhCtrl.Validation += OnImpulsePerkWhCtrlValidation;
            ElectricityPricePerkWhCtrl.Validation += OnElectricityPricePerkWhCtrlValidation;
            MinWattageCtrl.Validation += OnMinWattageCtrlValidation;
            MaxWattageCtrl.Validation += OnMaxWattageCtrlValidation;
            MaxChargingTimeCtrl.Validation += OnMaxChargingTimeCtrlValidation;
            Currency.Validation += OnCurrencyValidation;
        }

        /// <summary>
        /// Initialisiert das Formular
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        public override void Initialize(RenderContextFormular context)
        {
            base.Initialize(context);
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Impulsrate geprüft werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnImpulsePerkWhCtrlValidation(object sender, ValidationEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Value);
            }
            catch (Exception ex)
            {
                e.Results.Add(new ValidationResult
                (
                    TypesInputValidity.Error,
                    ex.Message
                ));
            }
        }


        /// <summary>
        /// Wird aufgerufen, wenn der Kw-Preis geprüft werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnElectricityPricePerkWhCtrlValidation(object sender, ValidationEventArgs e)
        {
            try
            {
                var value = Convert.ToDouble(e.Value);

                if (value < 0)
                {
                    e.Results.Add(new ValidationResult
                    (
                        TypesInputValidity.Error,
                        "vila:vila.setting.form.electricitypriceperkwhctrl.validation.negative"
                    ));
                }
            }
            catch (Exception ex)
            {
                e.Results.Add(new ValidationResult
                (
                    TypesInputValidity.Error,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn die minimale Ladungsleistung geprüft werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnMinWattageCtrlValidation(object sender, ValidationEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.Value))
                {
                    var value = Convert.ToDouble(e.Value);

                    if (value < 0 && value != -1)
                    {
                        e.Results.Add(new ValidationResult
                        (
                            TypesInputValidity.Error,
                            "vila:vila.setting.form.validation.low"
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                e.Results.Add(new ValidationResult
                (
                    TypesInputValidity.Error,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn die maximale Ladungsleistung geprüft werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnMaxWattageCtrlValidation(object sender, ValidationEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.Value))
                {
                    var value = Convert.ToInt32(e.Value);

                    if (value < -1)
                    {
                        e.Results.Add(new ValidationResult
                        (
                            TypesInputValidity.Error,
                            "vila:vila.setting.form.validation.low"
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                e.Results.Add(new ValidationResult
                (
                    TypesInputValidity.Error,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Ladehöchstdauer geprüft werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnMaxChargingTimeCtrlValidation(object sender, ValidationEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.Value))
                {
                    var value = Convert.ToInt32(e.Value);

                    if (value < -1)
                    {
                        e.Results.Add(new ValidationResult
                        (
                            TypesInputValidity.Error,
                            "vila:vila.setting.form.validation.low"
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                e.Results.Add(new ValidationResult
                (
                    TypesInputValidity.Error,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Währungsfeld geprüft werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnCurrencyValidation(object sender, ValidationEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(e.Value))
                {
                    e.Results.Add(new ValidationResult
                    (
                        TypesInputValidity.Error,
                        "vila:vila.setting.currency.validation.null"
                    ));
                }
                else if (e.Value.Length > 10)
                {
                    e.Results.Add(new ValidationResult
                    (
                        TypesInputValidity.Error,
                        "vila:vila.setting.currency.validation.tolong"
                    ));
                }
            }
            catch (Exception ex)
            {
                e.Results.Add(new ValidationResult
                (
                    TypesInputValidity.Error,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Formular befüllt werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnFillFormular(object sender, FormularEventArgs e)
        {
            ImpulsePerkWhCtrl.Value = ViewModel.Instance.Settings.ImpulsePerkWh.ToString();
            ElectricityPricePerkWhCtrl.Value = ViewModel.Instance.Settings.ElectricityPricePerkWh.ToString();
            MaxWattageCtrl.Value = ViewModel.Instance.Settings.MaxWattage.ToString();
            MinWattageCtrl.Value = ViewModel.Instance.Settings.MinWattage.ToString();
            MaxChargingTimeCtrl.Value = ViewModel.Instance.Settings.MaxChargingTime.ToString();
            Currency.Value = ViewModel.Instance.Settings.Currency.ToString();
            //Auto.Value = ViewModel.Instance.Settings.Auto ? "true" : "false";
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Formular bearbeitet werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnProcessFormular(object sender, FormularEventArgs e)
        {
            ViewModel.Instance.Settings.ImpulsePerkWh = Convert.ToInt32(ImpulsePerkWhCtrl.Value);
            ViewModel.Instance.Settings.ElectricityPricePerkWh = (float)Convert.ToDouble(ElectricityPricePerkWhCtrl.Value);
            ViewModel.Instance.Settings.MaxWattage = !string.IsNullOrWhiteSpace(MaxWattageCtrl.Value) ? Convert.ToInt32(MaxWattageCtrl.Value) : -1;
            ViewModel.Instance.Settings.MinWattage = !string.IsNullOrWhiteSpace(MinWattageCtrl.Value) ? Convert.ToInt32(MinWattageCtrl.Value) : -1;
            ViewModel.Instance.Settings.MaxChargingTime = !string.IsNullOrWhiteSpace(MaxChargingTimeCtrl.Value) ? Convert.ToInt32(MaxChargingTimeCtrl.Value) : -1;
            ViewModel.Instance.Settings.Currency = string.IsNullOrWhiteSpace(Currency.Value) ? "€" : Currency.Value;
            //ViewModel.Instance.Settings.Auto = Auto.Value != null && Auto.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
            ViewModel.Instance.SaveSettings();
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            ElectricityPricePerkWhCtrl.Label = string.Format(context.I18N("vila:vila.setting.form.electricitypriceperkwhctrl.label"), ViewModel.Instance.Settings.Currency);

            return base.Render(context);
        }
    }
}
