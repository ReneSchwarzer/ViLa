using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebIcon;
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
            var metrics = new ControlPanel() { Classes = new[] { "vila-metrics" } };
            metrics.Add(CreateMetric(new IconBolt(), "vila:vila.dashboard.hint.minute", "vila-current", "vila:vila.dashboard.power", snapshot.CurrentPower + " kW"));
            metrics.Add(CreateMetric(new IconChargingStation(), "vila:vila.dashboard.hint.session", "vila-energy", "vila:vila.dashboard.energy", snapshot.Power + " kWh"));
            metrics.Add(CreateMetric(new IconEuroSign(), "vila:vila.dashboard.hint.session", "vila-cost", "vila:vila.dashboard.cost", snapshot.Cost + " " + ViewModel.Instance.Settings.Currency));
            metrics.Add(CreateMetric(new IconClock(), "vila:vila.dashboard.hint.elapsed", "vila-duration", "vila:vila.dashboard.duration", snapshot.MeasurementTime));
            Add(metrics);

            var section = new ControlSection("vila-live-section")
            {
                Header = _ => "vila:vila.dashboard.live",
                Layout = _ => TypeLayoutSection.Rule
            };
            section.Add(new ControlText("vila-status")
            {
                Text = _ => snapshot.ActiveCharging ? "vila:vila.dashboard.active" : "vila:vila.dashboard.idle",
                Classes = new[] { "vila-status" }
            });
            section.Add(new ControlChart("vila-power-chart")
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
            section.Add(new ControlText("vila-connection")
            {
                Text = _ => "vila:vila.dashboard.connecting",
                Format = _ => TypeFormatText.Small,
                Role = _ => "status"
            });
            Add(section);

            var html = (HtmlElementTextContentDiv)base.Render(renderContext, visualTree);
            if (snapshot.ActiveCharging)
            {
                html.Class += " vila-charging";
            }
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
        /// <param name="icon">The icon identifying the metric.</param>
        /// <param name="hint">The localized description of the measurement period.</param>
        /// <param name="id">The identifier updated by the dashboard controller.</param>
        /// <param name="label">The localized metric label.</param>
        /// <param name="value">The initial formatted value.</param>
        /// <returns>The metric card.</returns>
        private static ControlPanel CreateMetric(IIcon icon, string hint, string id, string label, string value)
        {
            var metric = new ControlPanel { Classes = new[] { "vila-metric" } };
            var badge = new ControlPanel { Classes = new[] { "vila-metric-icon" } };
            badge.Add(new ControlIcon { Icon = _ => icon });
            var content = new ControlPanel();
            content.Add(new ControlText { Text = _ => label });
            content.Add(new ControlText(id) { Text = _ => value, Classes = new[] { "vila-metric-value" } });
            content.Add(new ControlText { Text = _ => hint, Format = _ => TypeFormatText.Small });
            metric.Add(badge, content);
            return metric;
        }
    }
}
