using System;
using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using ViLa.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebAttribute;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebFragment
{
    [Section(Section.PropertyPrimary)]
    [Module<Module>]
    [Scope<PageDashboard>]
    [Scope<PageHistory>]
    public sealed class FragmentPropertyCost : FragmentControlList
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentPropertyCost()
            : base("history_cost")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IFragmentContext context, IPage page)
        {
            base.Initialization(context, page);

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
            Items.Clear();

            void item(IEnumerable<MeasurementLog> value, string yaer)
            {
                if (!value.Any()) return;

                Items.Add(new ControlListItem
                (
                    new ControlText()
                    {
                        Text = $"{yaer}",
                        Format = TypeFormatText.H4,
                        TextColor = new PropertyColorText(TypeColorText.Secondary)
                    },
                    new ControlAttribute()
                    {
                        Name = $"{string.Format(context.Culture, "{0:F2}", value.Sum(x => x.Cost))} {ViewModel.Instance.Settings.Currency}",
                        Icon = new PropertyIcon(TypeIcon.EuroSign),
                        Value = "",
                        TextColor = new PropertyColorText(TypeColorText.Secondary)
                    },
                    new ControlAttribute()
                    {
                        Name = $"{string.Format(context.Culture, "{0:F2}", value.Sum(x => x.Power))} kWh",
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
