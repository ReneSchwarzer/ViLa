using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WWW.Vehicles
{
    /// <summary>
    /// Provides creation, editing, deactivation, and restoration of vehicle profiles.
    /// </summary>
    [Title("vila:vila.vehicles.title")]
    [WebIcon<IconCar>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Builds the catalog and the selected vehicle editor.
        /// </summary>
        /// <param name="renderContext">The context supplying routing, parameters, and culture.</param>
        /// <param name="visualTree">The page tree receiving the catalog controls.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var vehicles = ViewModel.Instance.Vehicles.GetAll();
            var id = renderContext.Request.GetParameter("vehicle")?.Value;
            var selected = vehicles.FirstOrDefault(x => x.Id == id);
            var uri = WebEx.ComponentHub.SitemapManager.GetUri<Index>(renderContext.Request.ApplicationContext);
            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
            var panel = new ControlPanel() { Classes = new[] { "vila-vehicle-page" } };
            panel.Add(new ControlText { Text = _ => "vila:vila.vehicles.intro" });
            if (!string.IsNullOrEmpty(id) && selected == null)
            {
                panel.Add(new ControlText { Text = _ => "vila:vila.vehicles.missing" });
            }
            else
            {
                var form = new ControlFormVehicle(selected) { RedirectUri = _ => uri };
                var editor = new ControlCard(null, form)
                {
                    Header = _ => selected == null ? "vila:vila.vehicles.create" : "vila:vila.vehicles.edit",
                    Classes = new[] { "vila-vehicle-editor" }
                };
                panel.Add(editor);
            }
            if (selected != null)
            {
                panel.Add(new ControlLink { Text = _ => "vila:vila.vehicles.create", Uri = _ => uri });
            }
            panel.Add(new ControlText { Text = _ => "vila:vila.vehicles.catalog", Format = _ => TypeFormatText.H2 });
            var grid = new ControlGrid { Classes = new[] { "vila-vehicle-grid" } };
            foreach (var vehicle in vehicles)
            {
                var card = new ControlCard()
                {
                    Header = _ => vehicle.Name,
                    GridColumn = _ => new PropertyGrid(TypeDevice.Medium, 4)
                };
                card.Add(new ControlText { Text = _ => vehicle.MaxPowerKw.ToString("0.###", culture) + " kW", Classes = new[] { "vila-metric-value" } });
                card.Add(new ControlText { Text = _ => vehicle.Active ? "vila:vila.vehicles.active" : "vila:vila.vehicles.inactive" });
                card.Add(new ControlText { Text = _ => vehicle.AutomaticAssignment ? "vila:vila.vehicles.auto.label" : "vila:vila.vehicles.manual.label" });
                card.Add(new ControlLink
                {
                    Text = _ => "vila:vila.vehicles.edit",
                    Uri = _ => new UriEndpoint(uri + "?vehicle=" + Uri.EscapeDataString(vehicle.Id))
                });
                grid.Add(card);
            }
            panel.Add(grid);
            if (vehicles.Length == 0)
            {
                panel.Add(new ControlText { Text = _ => "vila:vila.vehicles.empty" });
            }
            visualTree.Content.MainPanel.AddPrimary(panel);
        }
    }
}
