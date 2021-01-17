using System;
using System.Linq;
using System.Text;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.PropertyPreferences)]
    [Application("ViLa")]
    [Context("dashboard")]
    [Context("history")]
    public sealed class ControlPropertyCurrent : ControlList, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyCurrent()
            : base("current")
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            Layout = TypeLayoutList.Flush;
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            context.Page.HeaderScriptLinks.Add(context.Uri.Root.Append("/assets/js/vila.dashboard.js"));

            Items.Add(new ControlListItem
            (
                new ControlText()
                {
                    Text = context.I18N("vila.charging.current"),
                    Format = TypeFormatText.H4,
                    TextColor = new PropertyColorText(TypeColorText.Secondary)
                },
                new ControlAttribute("current")
                {
                    Name = $"{ Math.Round(ViewModel.Instance.AutoMeasurementLog.Measurements.Sum(x => x.Power), 2).ToString(context.Culture)} kWh",
                    Icon = new PropertyIcon(TypeIcon.Bolt),
                    Value = "",
                    TextColor = new PropertyColorText(TypeColorText.Secondary)
                }
            ));

            var builder = new StringBuilder();
            builder.AppendLine($"var restUrl='{ context.Uri.Root.Append("api") }';");
            builder.AppendLine($"var currency='{ ViewModel.Instance.Settings.Currency }';");
            builder.AppendLine($"var vila_charging_current='{ context.Page.I18N("vila.charging.current") }';");
            builder.AppendLine($"var vila_charging_begin='{ context.Page.I18N("vila.charging.begin") }';");
            builder.AppendLine($"var vila_charging_stop='{ context.Page.I18N("vila.charging.stop") }';");
            builder.AppendLine($"var vila_charging_duration='{ context.Page.I18N("vila.charging.duration") }';");
            builder.AppendLine($"var vila_charging_cost='{ context.Page.I18N("vila.charging.cost") }';");
            builder.AppendLine($"var vila_charging_consumption='{ context.Page.I18N("vila.charging.consumption") }';");

            context.Page.AddScript($"charging_i18n", builder.ToString());

            return base.Render(context);
        }
    }
}
