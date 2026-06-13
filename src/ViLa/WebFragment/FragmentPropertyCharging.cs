using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a form fragment for configuring property charging settings.
    /// </summary>
    [Section<SectionPropertyPreferences>]
    [Application<Application>]
    [Scope<ViLa.WWW.Index>]
    [Scope<ViLa.WWW.History.Index>]
    public sealed class FragmentPropertyCharging : FragmentControlForm
    {
        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentPropertyCharging(IFragmentContext fragmentContext)
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
