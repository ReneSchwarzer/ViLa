using System.Linq;
using WebExpress.WebApp.WebPage;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;

namespace ViLa.WWW.Vehicles
{
    /// <summary>
    /// Redirects previous catalog bookmarks to the vehicle settings page.
    /// </summary>
    [Application<Application>]
    public sealed class Index : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Opens the settings catalog with the original search, page and vehicle selection.
        /// </summary>
        /// <param name="renderContext">The request containing the catalog parameters.</param>
        /// <param name="visualTree">The visual tree of the requested page.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var request = renderContext.Request;
            var uri = WebEx.ComponentHub.SitemapManager.GetUri<Settings.Vehicles>(request.ApplicationContext);
            throw new RedirectException(uri.Concat(request.Uri.Query.ToArray()));
        }
    }
}
