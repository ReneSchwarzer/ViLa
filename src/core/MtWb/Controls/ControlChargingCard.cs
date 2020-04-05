using MtWb.Model;
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
            if (!ViewModel.Instance.ActiveCharging)
            {
                Content.Add(new ControlButtonLink(Page)
                {
                    Text = "Ladevorgang starten",
                    Layout = TypesLayoutButton.Success,
                    Icon = Icon.PlayCircle,
                    Url = Page.GetUrl(0, "on")
                });
            }
            else
            {
                Content.Add(new ControlButtonLink(Page)
                {
                    Text = "Ladevorgang abbrechen",
                    Layout = TypesLayoutButton.Danger,
                    Icon = Icon.PowerOff,
                    Url = Page.GetUrl(0, "off")
                });
            }

            if (ViewModel.Instance.ActiveCharging)
            {
                Content.Add(new ControlText(Page)
                {
                    Text = string.Format("Verbrauch: {0:F2} kWh", ViewModel.Instance.CurrentMeasurementLog?.Power)
                });
            }
            
            return base.ToHtml();
        }
    }
}
