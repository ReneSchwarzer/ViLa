using System.Linq;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;
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
    public sealed class FragmentFooterVersion : FragmentControlText
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentFooterVersion()
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

            TextColor = new PropertyColorText(TypeColorText.Muted);
            Format = TypeFormatText.Center;
            Size = new PropertySizeText(TypeSizeText.Small);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            var webexpress = typeof(WebEx).Assembly;
            var plugin = ComponentManager.PluginManager.Plugins.Where(x => x.Assembly == GetType().Assembly).FirstOrDefault();

            Text = string.Format
            (
                I18N(context.Culture, "vila:app.version.label"),
                I18N(context.Culture, plugin?.PluginName),
                plugin?.Version,
                webexpress?.GetName().Name,
                webexpress?.GetName().Version
            );

            return base.Render(context);
        }
    }
}
