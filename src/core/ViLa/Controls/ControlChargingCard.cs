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
        /// <param name="id">Die ID</param>
        public ControlChargingCard(string id = null)
            : base(id)
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
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            var card = new ControlPanelCard()
            {
                BackgroundColor = new PropertyColorBackground(TypeColorBackground.Light)
            };

            if (!ViewModel.Instance.ActiveCharging)
            {
                card.Content.Add(new ControlButtonLink("charging_btn")
                {
                    Text = "Ladevorgang starten",
                    Color = new PropertyColorButton(TypeColorButton.Success),
                    Icon = new PropertyIcon(TypeIcon.PlayCircle),
                    Uri = context.Page.Uri.Root.Append("on")
                });
            }
            else
            {
                card.Content.Add(new ControlButtonLink("charging_btn")
                {
                    Text = "Ladevorgang beenden",
                    Color = new PropertyColorButton(TypeColorButton.Success),
                    Icon = new PropertyIcon(TypeIcon.PowerOff),
                    Uri = context.Page.Uri.Root.Append("off")
                });
            }

            if (ViewModel.Instance.ActiveCharging)
            {
                card.Content.Add(new ControlPanelCard
                (
                    new ControlText("measurementtime")
                    {
                        Text = string.Format("Ladedauer: {0}", new TimeSpanConverter().Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null))
                    }, new ControlText("cost")
                    {
                        Text = string.Format("Angefallene Kosten: {0:F2} €", ViewModel.Instance.CurrentMeasurementLog?.Cost)
                    }, new ControlText("power")
                    {
                        Text = string.Format("Verbrauch: {0:F2} kWh", ViewModel.Instance.CurrentMeasurementLog?.Power)
                    })
                {
                    HorizontalAlignment = TypeHorizontalAlignment.Default,
                    Classes = new List<string>(new[] { "mt-5" })
                });

                card.Content.Add(new ControlCanvas("canvas"));
            }

            Content.Add(card);

            return base.Render(context);
        }
    }
}
