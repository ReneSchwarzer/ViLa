using MtWb.Model;
using System;
using WebExpress.Pages;
using WebExpress.UI.Controls;
using WebServer.Html;

namespace MtWb.Controls
{
    public class ControlChargingCard : ControlPanel
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="page">Die zugehörige Seite</param>
        /// <param name="id">Die ID</param>
        public ControlChargingCard(IPage page, string id = null)
            : base(page, id)
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode ToHtml()
        {
            var card = new ControlPanelCard(Page)
            {
                Layout = TypesLayoutCard.Light
            };

            if (!ViewModel.Instance.ActiveCharging)
            {
                card.Content.Add(new ControlButtonLink(Page)
                {
                    Text = "Ladevorgang starten",
                    Layout = TypesLayoutButton.Success,
                    Icon = Icon.PlayCircle,
                    Url = Page.GetUrl(0, "on")
                });
            }
            else
            {
                card.Content.Add(new ControlButtonLink(Page)
                {
                    Text = "Ladevorgang abbrechen",
                    Layout = TypesLayoutButton.Danger,
                    Icon = Icon.PowerOff,
                    Url = Page.GetUrl(0, "off")
                });
            }

            if (ViewModel.Instance.ActiveCharging)
            {
                card.Content.Add(new ControlPanelCard(Page,
                    new ControlText(Page, "measurementtime")
                    {
                        Text = string.Format("Ladedauer: {0}", new TimeSpanConverter().Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null))
                    }, new ControlText(Page, "cost")
                    {
                        Text = string.Format("Angefallene Kosten: {0:F2} €", ViewModel.Instance.CurrentMeasurementLog?.Cost)
                    }, new ControlText(Page, "power")
                    {
                        Text = string.Format("Verbrauch: {0:F2} kWh", ViewModel.Instance.CurrentMeasurementLog?.Power)
                    })
                {
                    HorizontalAlignment = TypesHorizontalAlignment.Default,
                    Layout = TypesLayoutCard.Default,
                    Class = "mt-5"
                });
            }

            Content.Add(card);

            return base.ToHtml();
        }
    }
}
