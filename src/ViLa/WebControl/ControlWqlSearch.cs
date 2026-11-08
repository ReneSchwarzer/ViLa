using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Provides basic and advanced search backed by the specified autocomplete service.
    /// </summary>
    internal sealed class ControlWqlSearch : ControlAdvancedSearch
    {
        /// <summary>
        /// Initializes the prompt with its page identifier and autocomplete endpoint.
        /// </summary>
        /// <param name="id">The unique search control identifier.</param>
        /// <param name="serviceUri">The WQL autocomplete endpoint for the searchable records.</param>
        public ControlWqlSearch(string id, string serviceUri = null) : base(id)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(serviceUri);
        }

        /// <summary>
        /// Renders a WQL search host connected to the page query.
        /// </summary>
        /// <param name="renderContext">The context containing the current query.</param>
        /// <param name="visualTree">The tree containing the search control.</param>
        /// <returns>The search host with its initial mode.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var host = (HtmlElementTextContentDiv)base.Render(renderContext, visualTree);
            var wrapper = new HtmlElementTextContentDiv(host) { Class = "vila-wql-search" };
            if (Id == WebFragment.FragmentHistorySearchFragment.ContentId)
            {
                new Binding().Add(new BindSearch { Source = Id },
                    new BindPaging { Source = WebFragment.FragmentHistoryPaginationFragment.ContentId }).ApplyUserAttributes(wrapper);
            }
            return wrapper;
        }
    }
}
