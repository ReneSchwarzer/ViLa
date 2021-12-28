using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.Attribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.Uri;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebPage;
using static WebExpress.Internationalization.InternationalizationManager;

namespace ViLa.WebComponent
{
    [Section(Section.FooterPrimary)]
    [Application("ViLa")]
    public sealed class ComponentFooterLicence : ControlPanel, IComponent
    {
        /// <summary>
        /// Die Lizenz
        /// </summary>
        private ControlLink LicenceLink { get; } = new ControlLink()
        {
            TextColor = new PropertyColorText(TypeColorText.Muted),
            Size = new PropertySizeText(TypeSizeText.Small)
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentFooterLicence()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public void Initialization(IComponentContext context)
        {
            Classes.Add("text-center");

            Content.Add(LicenceLink);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            LicenceLink.Text = I18N(context.Culture, "vila:app.license.label");
            LicenceLink.Uri = new UriAbsolute(I18N(context.Culture, "vila:app.license.uri"));

            return base.Render(context);
        }
    }
}
