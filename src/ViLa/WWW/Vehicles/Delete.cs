using System.Net;
using ViLa.Model;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebPage;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WWW.Vehicles
{
    /// <summary>
    /// Presents a confirmation form before removing a vehicle profile.
    /// </summary>
    [Application<Application>]
    public sealed class Delete : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Builds the confirmation form for the requested vehicle.
        /// </summary>
        /// <param name="renderContext">The request containing the vehicle identifier.</param>
        /// <param name="visualTree">The tree receiving the remote form.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var id = renderContext.Request.GetParameter("id")?.Value;
            var vehicle = ViewModel.Instance.Vehicles.GetVehicle(id);
            var form = new ControlDataForm("vehicle_delete")
            {
                ItemId = _ => id,
                Mode = _ => "edit",
                Method = _ => RequestMethod.POST,
                ServiceFactory = context => DataServiceDescriptor.FormData(WebEx.ComponentHub.SitemapManager
                    .GetUri<Api._1_.Vehicles.Delete>(context.Request.ApplicationContext).ToString())
            };
            form.Add(new ControlFormItemInputText("confirmation")
            {
                Label = _ => WebUtility.HtmlEncode(vehicle?.Name ?? string.Empty),
                Help = _ => "vila:vila.vehicles.delete.help",
                Required = _ => true,
                MaxLength = _ => 100
            });
            form.AddPreferencesButton(new ControlFormItemButtonSubmit { Text = _ => "vila:vila.vehicles.delete" });
            visualTree.Content.MainPanel.AddPrimary(form);
        }
    }
}
