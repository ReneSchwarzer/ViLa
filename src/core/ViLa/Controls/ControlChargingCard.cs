using System;
using System.Collections.Generic;
using ViLa.Model;
using WebExpress.Html;
using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace ViLa.Controls
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
                BackgroundColor = new PropertyColorBackground(TypeColorBackground.Light)
            };

            if (!ViewModel.Instance.ActiveCharging)
            {
                card.Content.Add(new ControlButtonLink(Page, "charging_btn")
                {
                    Text = "Ladevorgang starten",
                    Color = new PropertyColorButton(TypeColorButton.Success),
                    Icon = new PropertyIcon(TypeIcon.PlayCircle),
                    Uri = Page.Uri.Root.Append("on")
                });
            }
            else
            {
                card.Content.Add(new ControlButtonLink(Page, "charging_btn")
                {
                    Text = "Ladevorgang beenden",
                    Color = new PropertyColorButton(TypeColorButton.Success),
                    Icon = new PropertyIcon(TypeIcon.PowerOff),
                    Uri = Page.Uri.Root.Append("off")
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
                    HorizontalAlignment = TypeHorizontalAlignment.Default,
                    Classes = new List<string>(new[] { "mt-5" })
                });

                card.Content.Add(new ControlCanvas(Page, "canvas"));
            }

            Content.Add(card);

            return base.ToHtml();
        }
    }
}
