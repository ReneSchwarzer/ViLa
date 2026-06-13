using System.Globalization;
using System.Text;
using ViLa.Model;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a fragment control list for displaying current property information.
    /// </summary>
    [Section<SectionPropertyPreferences>]
    [Application<Application>]
    [Scope<ViLa.WWW.Index>]
    [Scope<ViLa.WWW.History.Index>]
    public sealed class FragmentPropertyCurrent : FragmentControlList
    {
        private readonly IFragmentContext _fragmentContext;

        private ControlText Title { get; } = new ControlText()
        {
            Text = _ => "vila:vila.charging.current",
            Format = _ => TypeFormatText.H4,
            TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
        };

        private ControlAttribute CurrentPower { get; } = new ControlAttribute()
        {
            Icon = _ => new IconBolt(),
            TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
        };

        private ControlText Description { get; } = new ControlText()
        {
            Text = _ => "vila:vila.charging.current.description",
            Format = _ => TypeFormatText.Small,
            TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
        };

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentPropertyCurrent(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            _fragmentContext = fragmentContext;

            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            Layout = _ => TypeLayoutList.Flush;

            var item = new ControlListItem();
            item.Add(Title);
            item.Add(CurrentPower);
            item.Add(Description);

            Add(item);
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
