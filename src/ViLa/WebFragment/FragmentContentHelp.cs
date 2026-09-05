using System.Reflection;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents the content fragment for the help page.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Help.Index>]
    public sealed class FragmentContentHelp : FragmentControlPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentContentHelp(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
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
            Clear();

            var img = new ControlImage()
            {
                Uri = ctx => new UriEndpoint(ctx.Request.ApplicationContext?.ContextPath + "/assets/img/vila.svg"),
                Width = _ => 200,
                Height = _ => 200,
                HorizontalAlignment = _ => TypeHorizontalAlignment.Right
            };

            Add(img);

            var card = new ControlPanelCard();

            card.Add(new ControlText()
            {
                Text = _ => "vila:app.label",
                Format = _ => TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:app.description",
                Format = _ => TypeFormatText.Paragraph
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.privacypolicy.label",
                Format = _ => TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.privacypolicy.description",
                Format = _ => TypeFormatText.Paragraph
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.disclaimer.label",
                Format = _ => TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.disclaimer.description",
                Format = _ => TypeFormatText.Paragraph
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.about.label",
                Format = _ => TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.version.label",
                TextColor = _ => new PropertyColorText(TypeColorText.Primary)
            });

            var version = typeof(Application).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? typeof(Application).Assembly.GetName().Version?.ToString();

            card.Add(new ControlText()
            {
                Text = _ => version,
                TextColor = _ => new PropertyColorText(TypeColorText.Dark)
            });

            card.Add(new ControlText()
            {
                Text = _ => "vila:vila.help.contact.label",
                TextColor = _ => new PropertyColorText(TypeColorText.Primary)
            });

            card.Add(new ControlLink()
            {
                Text = _ => "rene_schwarzer@hotmail.de",
                Uri = _ => new UriEndpoint("mailto:rene_schwarzer@hotmail.de"),
                TextColor = _ => new PropertyColorText(TypeColorText.Dark)
            });

            Add(card);

            return base.Render(renderContext, visualTree);
        }
    }
}
