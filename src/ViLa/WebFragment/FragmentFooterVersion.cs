using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a centered, small-text fragment control that displays version information in the 
    /// primary footer section.
    /// </summary>
    [Section<SectionFooterPrimary>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class FragmentFooterVersion : FragmentControlText
    {
        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentFooterVersion(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Format = _ => TypeFormatText.Center;
            Size = _ => new PropertySizeText(TypeSizeText.Small);
            Text = _ => $"Vila {typeof(FragmentFooterVersion).Assembly.GetName().Version} / WebExpress {typeof(WebEx).Assembly.GetName().Version}";
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
