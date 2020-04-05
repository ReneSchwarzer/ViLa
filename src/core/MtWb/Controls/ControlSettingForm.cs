using MtWb.Model;
using System;
using System.Collections.Generic;
using System.Text;
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
                Name = "ImpulsePerkWh",
                Label = "Die Anzahl der Impulse pro kWh:"
            };

            Add(ImpulsePerkWhCtrl);

            InitFormular += (s, e) =>
            {
                ImpulsePerkWhCtrl.Value = ViewModel.Instance.Settings.ImpulsePerkWh.ToString();
             };

            ProcessFormular += (s, e) =>
            {
                ViewModel.Instance.Settings.ImpulsePerkWh = Convert.ToInt32(ImpulsePerkWhCtrl.Value);
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
        }
    }
}
