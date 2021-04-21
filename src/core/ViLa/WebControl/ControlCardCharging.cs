using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViLa.Model;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.WebControl;

namespace ViLa.WebControl
{
    public class ControlCardCharging : ControlPanel
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Die ID</param>
        public ControlCardCharging(string id = null)
            : base(id)
        {
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            context.Page.HeaderScriptLinks.Add(context.Uri.Root.Append("/assets/js/vila.dashboard.js"));

            static string[] createArray(int size)
            {
                var array = new string[size];
                for (var i = size * -1; i < 0; i++)
                {
                    array[size + i] = (i + 1).ToString();
                }

                return array;
            }

            var chartLabels = ViewModel.Instance.ActiveCharging ?
                ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => ViewModel.Instance.CurrentMeasurementLog.Measurements.IndexOf(x).ToString()).ToArray() :
                createArray(ViewModel.Instance.CurrentMeasurementLog.Measurements.Count);
                
            var chartData = ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => x.Power).ToArray();

            var card = new ControlPanelCard()
            {
                BackgroundColor = new PropertyColorBackground(TypeColorBackground.Light)
            };

            card.Content.Add(new ControlPanelCard
            (
                new ControlText("measurementtime")
                {
                    Text = string.Format(context.Culture, context.I18N("vila.charging.duration"), ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? new TimeSpanConverter().Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null) : "-")
                }, 
                new ControlText("cost")
                {
                    Text = string.Format(context.Culture, context.I18N("vila.charging.cost"), ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? ViewModel.Instance.CurrentMeasurementLog?.Cost : "-", ViewModel.Instance.Settings.Currency)
                }, 
                new ControlText("power")
                {
                    Text = string.Format(context.Culture, context.I18N("vila.charging.consumption"), ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? ViewModel.Instance.CurrentMeasurementLog?.Power : "-")
                })
            {
                HorizontalAlignment = TypeHorizontalAlignment.Default,
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
            });

            card.Content.Add(new ControlChart("chart")
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None),
                Title = "",
                Labels = chartLabels,
                Data = new List<ControlChartDataset> { new ControlChartDataset() { Data = chartData, Title = "Verlauf" } },
                TitleX = context.I18N("vila.charging.title.x"),
                TitleY = context.I18N("vila.charging.title.y"),
                Styles = new List<string>() { "max-width: 85%;" },
                Minimum = 0
            });

            Content.Add(card);

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
