using System.Linq;
using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebUri;

namespace ViLa.App
{
    /// <summary>
    /// Helper for resolving sitemap URIs without explicitly threading the
    /// application context through every call site. Looks up the first
    /// available application in the component hub and resolves the URI
    /// relative to it.
    /// </summary>
    public static class RestUriHelper
    {
        /// <summary>
        /// Gets the URI for a registered endpoint without requiring a
        /// render context.
        /// </summary>
        /// <typeparam name="T">The endpoint type.</typeparam>
        /// <returns>The URI of the endpoint, or null if none is registered.</returns>
        public static IUri GetUri<T>()
        {
            var hub = WebExpress.WebCore.WebEx.ComponentHub;
            var apps = hub?.ApplicationManager?.Applications;
            var app = apps != null ? apps.FirstOrDefault() : null;
            if (app == null)
            {
                return null;
            }

            return hub.SitemapManager.GetUri(typeof(T), app);
        }
    }
}
