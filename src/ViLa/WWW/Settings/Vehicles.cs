using System;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebCore.WebSettingPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WWW.Settings
{
    /// <summary>
    /// Provides creation, editing, deactivation, and restoration of vehicle profiles.
    /// </summary>
    [Title("vila:vila.vehicles.title")]
    [WebIcon<IconCar>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class Vehicles : ISettingPage<VisualTreeWebAppSetting>, IScopeGeneral
    {
        /// <summary>
        /// Builds the catalog and the selected vehicle editor.
        /// </summary>
        /// <param name="renderContext">The context supplying routing, parameters, and culture.</param>
        /// <param name="visualTree">The page tree receiving the catalog controls.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebAppSetting visualTree)
        {
            var app = renderContext.Request.ApplicationContext;
            var sitemap = WebEx.ComponentHub.SitemapManager;
            var editor = sitemap.GetUri<WWW.Vehicles.Editor>(app);
            var panel = new ControlPanel { Classes = new[] { "vila-vehicle-page", "vila-vehicle-catalog" } };
            panel.Add(new ControlText { Text = _ => "vila:vila.vehicles.intro" });
            panel.Add(new ControlText { Text = _ => "vila:vila.vehicles.catalog", Format = _ => TypeFormatText.H2 });
            panel.Add(new ControlAdvancedSearch("vehicle-search")
            {
                ServiceFactory = _ => DataServiceDescriptor.QueryData(sitemap.GetUri<WWW.Api._1_.Vehicles.Wql>(app).ToString())
            });
            visualTree.Content.MainPanel.Headline.AddSecondary(new ControlButton
            {
                BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary),
                Text = _ => "vila:vila.vehicles.create",
                PrimaryAction = _ => new ActionModal("vehicle-create", editor)
            });
            panel.Add(new ControlDataTable("vehicle_catalog")
            {
                ServiceFactory = _ => DataServiceDescriptor.TableData(sitemap.GetUri<WWW.Api._1_.Vehicles.Table>(app).ToString()),
                PageSize = _ => 10,
                EmptyText = _ => "vila:vila.vehicles.no.matches",
                Bind = _ => new Binding().Add(new BindSearch { Source = "vehicle-search" }).Add(new BindPaging { Source = "vehicle-pages" })
            });
            panel.Add(new ControlPagination("vehicle-pages"));
            foreach (var mode in new[] { "create", "edit" })
            {
                panel.Add(new ControlModalRemoteForm("vehicle-" + mode)
                {
                    Header = _ => "vila:vila.vehicles." + mode,
                    CloseLabel = _ => "webexpress.webui:form.cancel.label",
                    Uri = _ => mode == "edit" ? new UriEndpoint(editor + "?id=" + Uri.EscapeDataString(renderContext.Request.GetParameter("vehicle")?.Value ?? string.Empty)) : editor,
                    Selector = _ => "#vehicle_editor"
                });
            }
            visualTree.Content.MainPanel.AddPrimary(panel);
        }
    }
}
