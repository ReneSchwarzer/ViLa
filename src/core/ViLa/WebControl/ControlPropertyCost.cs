using System;
using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.PropertyPrimary)]
    [Application("ViLa")]
    [Context("dashboard")]
    [Context("history")]
    public sealed class ControlPropertyCost : ControlList, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyCost()
            : base("history_cost")
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
            void item(List<MeasurementLog> value, string yaer)
            {
                if (value.Count == 0) return;

                Items.Add(new ControlListItem
                (
                    new ControlText()
                    {
                        Text = $"{ yaer }",
                        Format = TypeFormatText.H4,
                        TextColor = new PropertyColorText(TypeColorText.Secondary)
                    },
                    new ControlAttribute()
                    {
                        Name = $"{ Math.Round(value.Sum(x => x.Cost), 2).ToString(context.Culture) } { ViewModel.Instance.Settings.Currency }",
                        Icon = new PropertyIcon(TypeIcon.EuroSign),
                        Value = "",
                        TextColor = new PropertyColorText(TypeColorText.Secondary)
                    },
                    new ControlAttribute()
                    {
                        Name = $"{ Math.Round(value.Sum(x => x.Power), 2).ToString(context.Culture)} kWh",
                        Icon = new PropertyIcon(TypeIcon.Bolt),
                        Value = "",
                        TextColor = new PropertyColorText(TypeColorText.Secondary)
                    }
                ));
            }

            item(ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now), DateTime.Now.Year.ToString());
            item(ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(DateTime.Now.Year - 1, 1, 1), new DateTime(DateTime.Now.Year - 1, 12, 31)), (DateTime.Now.Year - 1).ToString());
            item(ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(DateTime.Now.Year - 2, 1, 1), new DateTime(DateTime.Now.Year - 2, 12, 31)), (DateTime.Now.Year - 2).ToString());

            return base.Render(context);
        }
    }
}
