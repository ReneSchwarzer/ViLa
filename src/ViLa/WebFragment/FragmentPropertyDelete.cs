using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>  
    /// Represents a fragment for a button used to delete a property.  
    /// </summary>
    [Section<SectionPropertySecondary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Details.Index>]
    public sealed class FragmentPropertyDelete : FragmentControlButtonLink
    {
        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentPropertyDelete(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>  
        /// Renders the control into an HTML node.  
        /// </summary>  
        /// <param name="renderContext">The render context.</param>  
        /// <param name="visualTree">The visual tree control.</param>  
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
