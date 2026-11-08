using WebExpress.WebApp.WebPage;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebUri;

namespace ViLa.WWW.Vehicles
{
    /// <summary>
    /// Redirects previous assignment bookmarks to the history route.
    /// </summary>
    [Application<Application>]
    public sealed class Assignments : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Preserves the search while opening the assignment page under history.
        /// </summary>
        /// <param name="renderContext">The request context.</param>
        /// <param name="visualTree">The page tree.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            if (renderContext.Request.Method == RequestMethod.POST)
            {
                new History.Assignments().Process(renderContext, visualTree);
                return;
            }
            var uri = WebEx.ComponentHub.SitemapManager.GetUri<History.Assignments>(renderContext.Request.ApplicationContext);
            throw new RedirectException(new UriEndpoint(uri + "?q=" +
                System.Uri.EscapeDataString(renderContext.Request.GetParameter("q")?.Value ?? string.Empty)));
        }
    }
}
