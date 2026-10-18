using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>
    /// Displays live charging metrics and the power curve through WebExpress controllers.
    /// </summary>
    public class ControlCardCharging : ControlPanel
    {
        /// <summary>
        /// Initializes the live dashboard panel.
        /// </summary>
        /// <param name="id">The optional control identifier.</param>
        public ControlCardCharging(string id = null)
            : base(id)
        {
            Classes = new[] { "wx-vila-chart", "vila-dashboard" };
        }

        /// <summary>
        /// Renders metric cards and an automatically refreshed power chart.
        /// </summary>
        /// <param name="renderContext">The context providing routing and localization.</param>
        /// <param name="visualTree">The visual tree receiving the controls.</param>
        /// <returns>The dashboard host with its refresh configuration.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            Clear();
            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
            var snapshot = ChargingSnapshot.Create(ViewModel.Instance, culture);
            var metrics = new ControlGrid() { Classes = new[] { "vila-metrics" } };
            metrics.Add(CreateMetric("vila-current", "vila:vila.dashboard.power", snapshot.CurrentPower + " kW"));
            metrics.Add(CreateMetric("vila-energy", "vila:vila.dashboard.energy", snapshot.Power + " kWh"));
            metrics.Add(CreateMetric("vila-cost", "vila:vila.dashboard.cost", snapshot.Cost + " " + ViewModel.Instance.Settings.Currency));
            metrics.Add(CreateMetric("vila-duration", "vila:vila.dashboard.duration", snapshot.MeasurementTime));
            Add(metrics);

            var card = new ControlCard()
            {
                Header = _ => "vila:vila.dashboard.live",
                Classes = new[] { "vila-live-card" }
            };
            card.Add(new ControlText("vila-status")
            {
                Text = _ => snapshot.ActiveCharging ? "vila:vila.dashboard.active" : "vila:vila.dashboard.idle",
                Classes = new[] { "vila-status" }
            });
            card.Add(new ControlChart("vila-power-chart")
            {
                Labels = _ => snapshot.ChartLabels,
                Data = _ => new[] { new ControlChartDataset()
                {
                    Title = "kW",
                    Data = new ControlChartDatasetPointCollection(snapshot.ChartData.Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray())
                } },
                TitleX = _ => "vila:vila.charging.title.x",
                TitleY = _ => "kW",
                Height = _ => 280,
                Responsive = _ => true,
                MaintainAspectRatio = _ => false,
                Minimum = _ => 0,
                YBeginAtZero = _ => true
            });
            card.Add(new ControlText("vila-connection")
            {
                Text = _ => "vila:vila.dashboard.connecting",
                Format = _ => TypeFormatText.Small,
                Role = _ => "status"
            });
            Add(card);

            var html = (HtmlElementTextContentDiv)base.Render(renderContext, visualTree);
            html.AddUserAttribute("data-url", WebEx.ComponentHub.SitemapManager
                .GetUri<WWW.Api._1_.Charging>(renderContext.Request.ApplicationContext)?.ToString());
            html.AddUserAttribute("data-currency", ViewModel.Instance.Settings.Currency);
            foreach (var key in new[] { "active", "idle", "connected", "offline" })
            {
                html.AddUserAttribute("data-" + key, WebEx.ComponentHub.InternationalizationManager
                    .Translate(culture, "vila:vila.dashboard." + key));
            }
            return html;
        }

        /// <summary>
        /// Creates a responsive metric card with a stable value identifier.
        /// </summary>
        /// <param name="id">The identifier updated by the dashboard controller.</param>
        /// <param name="label">The localized metric label.</param>
        /// <param name="value">The initial formatted value.</param>
        /// <returns>The metric card.</returns>
        private static ControlCard CreateMetric(string id, string label, string value)
        {
            return new ControlCard(null, new ControlText(id)
            {
                Text = _ => value,
                Classes = new[] { "vila-metric-value" }
            })
            {
                Header = _ => label,
                GridColumn = _ => new PropertyGrid(TypeDevice.Medium, 3),
                Classes = new[] { "vila-metric" }
            };
        }
    }
}
