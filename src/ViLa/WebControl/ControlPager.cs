using System;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Builds the server-rendered previous/next pager shared by the paged ViLa pages.
    /// </summary>
    internal static class ControlPager
    {
        /// <summary>
        /// Creates a pager for the given page, or null when everything fits on one page.
        /// </summary>
        /// <param name="request">The request supplying the display language.</param>
        /// <param name="page">The current page, starting at 1.</param>
        /// <param name="pageCount">The number of pages.</param>
        /// <param name="pageUri">Returns the address of a page number.</param>
        /// <returns>The pager panel, or null for a single page.</returns>
        public static IControl Create(IRequest request, int page, int pageCount, Func<int, string> pageUri)
        {
            if (pageCount <= 1)
            {
                return null;
            }

            var pager = new ControlPanel { Classes = new[] { "vila-pager" } };
            if (page > 1)
            {
                pager.Add(new ControlLink { Text = _ => "vila:vila.history.paging.previous", Uri = _ => new UriEndpoint(pageUri(page - 1)) });
            }
            pager.Add(new ControlText { Text = _ => I18N.Translate(request, "vila:vila.history.paging.page", page, pageCount) });
            if (page < pageCount)
            {
                pager.Add(new ControlLink { Text = _ => "vila:vila.history.paging.next", Uri = _ => new UriEndpoint(pageUri(page + 1)) });
            }
            return pager;
        }
    }
}
