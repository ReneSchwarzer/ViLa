using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebPage;

namespace ViLa.WWW.Vehicles
{
    /// <summary>
    /// Serves the REST form loaded by the vehicle catalog dialogs.
    /// </summary>
    [Application<Application>]
    public sealed class Editor : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Adds the form for the selected vehicle.
        /// </summary>
        /// <param name="renderContext">The request containing the optional identifier.</param>
        /// <param name="visualTree">The tree supplying the remote form markup.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var id = renderContext.Request.GetParameter("id")?.Value;
            visualTree.Title = I18N.Translate(renderContext, id == null ? "vila:vila.vehicles.create" : "vila:vila.vehicles.edit");
            visualTree.Content.MainPanel.AddPrimary(new ControlFormVehicle(id));
        }
    }
}
