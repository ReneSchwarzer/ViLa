using System;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents the content fragment for general settings.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Settings.General>]
    public sealed class FragmentContentSettingsGeneral : FragmentControlPanel
    {
        private ControlFormSetting Form { get; } = new ControlFormSetting("form_settings_general");

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentContentSettingsGeneral(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);

            Form.ImpulsePerkWhCtrl.Label = _ => "vila:vila.setting.form.impulseperkwhctrl.label";
            Form.ImpulsePerkWhCtrl.Help = _ => "vila:vila.setting.form.impulseperkwhctrl.description";
            Form.ElectricityPricePerkWhCtrl.Label = _ => "vila:vila.setting.form.electricitypriceperkwhctrl.label";
            Form.ElectricityPricePerkWhCtrl.Help = _ => "vila:vila.setting.form.electricitypriceperkwhctrl.description";
            Form.MaxWattageCtrl.Label = _ => "vila:vila.setting.form.maxwattagectrl.label";
            Form.MaxWattageCtrl.Help = _ => "vila:vila.setting.form.maxwattagectrl.description";
            Form.MinWattageCtrl.Label = _ => "vila:vila.setting.form.minwattagectrl.label";
            Form.MinWattageCtrl.Help = _ => "vila:vila.setting.form.minwattagectrl.description";
            Form.MaxChargingTimeCtrl.Label = _ => "vila:vila.setting.form.maxchargingtime.label";
            Form.MaxChargingTimeCtrl.Help = _ => "vila:vila.setting.form.maxchargingtime.description";
            Form.CurrencyCtrl.Label = _ => "vila:vila.setting.currency.label";
            Form.CurrencyCtrl.Help = _ => "vila:vila.setting.currency.description";
            Form.BillingDayOffsetCtrl.Label = _ => "vila:vila.setting.form.billingdayoffsetctrl.label";
            Form.BillingDayOffsetCtrl.Help = _ => "vila:vila.setting.form.billingdayoffsetctrl.description";

            Add(Form);

            Form.InitializeForm += OnInitializeForm;
            Form.ProcessForm += OnProcessForm;
        }

        /// <summary>
        /// Initializes the form state before rendering.
        /// </summary>
        /// <param name="e">Contains the form initialization context.</param>
        private void OnInitializeForm(ControlFormEventFormInitialize e)
        {
        }

        private void OnProcessForm(ControlFormEventFormProcess e)
        {
            var impulsePerKwhValue = e.Context.Request.GetParameter("ImpulsePerkWhCtrl")?.Value;
            var electricityPricePerKwhValue = e.Context.Request.GetParameter("ElectricityPricePerkWhCtrl")?.Value;
            var maxWattageValue = e.Context.Request.GetParameter("MaxWattageCtrl")?.Value;
            var minWattageValue = e.Context.Request.GetParameter("MinWattageCtrl")?.Value;
            var maxChargingTimeValue = e.Context.Request.GetParameter("MaxChargingTime")?.Value;
            var currencyValue = e.Context.Request.GetParameter("currency")?.Value;
            var billingOffsetValue = e.Context.Request.GetParameter("BillingDayOffsetCtrl")?.Value;

            if (int.TryParse(impulsePerKwhValue, out var impulsePerKwh))
            {
                ViewModel.Instance.Settings.ImpulsePerkWh = impulsePerKwh;
            }

            if (double.TryParse(electricityPricePerKwhValue, out var electricityPricePerKwh))
            {
                ViewModel.Instance.Settings.ElectricityPricePerkWh = (float)electricityPricePerKwh;
            }

            if (int.TryParse(maxWattageValue, out var maxWattage))
            {
                ViewModel.Instance.Settings.MaxWattage = maxWattage;
            }

            if (double.TryParse(minWattageValue, out var minWattage))
            {
                ViewModel.Instance.Settings.MinWattage = (float)minWattage;
            }

            if (int.TryParse(maxChargingTimeValue, out var maxChargingTime))
            {
                ViewModel.Instance.Settings.MaxChargingTime = maxChargingTime;
            }

            ViewModel.Instance.Settings.Currency = string.IsNullOrWhiteSpace(currencyValue) ? "€" : currencyValue;

            ViewModel.Instance.Settings.BillingDayOffset = int.TryParse(billingOffsetValue, out var billingOffset)
                ? Math.Clamp(billingOffset, 0, 31)
                : 0;

            ViewModel.Instance.SaveSettings();
        }

        /// <summary>
        /// Renders the control into an HTML node.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree control.</param>
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            Form.RedirectUri = ctx => ctx.Uri;

            return base.Render(renderContext, visualTree);
        }
    }
}
