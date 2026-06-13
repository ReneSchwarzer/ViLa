using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a control panel fragment for displaying a licence link in the primary footer section.
    /// </summary>
    [Section<SectionFooterPrimary>]
    [Application<Application>]
    [Cache]
    public sealed class FragmentFooterLicence : FragmentControlPanel
    {
        /// <summary>
        /// Gets the control link for the licence.
        /// </summary>
        private ControlLink LicenceLink { get; } = new ControlLink(null, []);

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentFooterLicence(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(LicenceLink);
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
