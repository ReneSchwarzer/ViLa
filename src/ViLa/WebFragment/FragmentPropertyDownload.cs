using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a button link fragment for downloading property data.
    /// </summary>
    [Section<SectionPropertyPreferences>]
    [Application<Application>]
    [Scope<ViLa.WWW.Details.Index>]
    public sealed class FragmentPropertyDownload : FragmentControlButtonLink
    {
        private readonly IFragmentContext _fragmentContext;

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentPropertyDownload(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            _fragmentContext = fragmentContext;

            Text = _ => "vila:vila.download.label";
            BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary);
            Icon = _ => new IconDownload();
            TextColor = _ => new PropertyColorText(TypeColorText.Light);
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
        }

        /// <summary>  
        /// Renders the control into an HTML node.  
        /// </summary>  
        /// <param name="renderContext">The render context.</param>  
        /// <param name="visualTree">The visual tree control.</param>  
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            //var id = renderContext.Request.GetParameter("id")?.Value;
            //Uri = _ => _fragmentContext?.ApplicationContext.ContextPath.Append($"measurements/{id}.xml");

            return base.Render(renderContext, visualTree);
        }
    }
}
