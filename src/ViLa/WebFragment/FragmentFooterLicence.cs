using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using static WebExpress.WebCore.Internationalization.InternationalizationManager;

namespace ViLa.WebFragment
{
    [Section(Section.FooterPrimary)]
    [Module<Module>]
    public sealed class FragmentFooterLicence : FragmentControlPanel
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
        public FragmentFooterLicence()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IFragmentContext context, IPage page)
        {
            base.Initialization(context, page);

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
            LicenceLink.Uri = I18N(context.Culture, "vila:app.license.uri");

            return base.Render(context);
        }
    }
}
