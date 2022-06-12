using System.Linq;
using WebExpress.WebAttribute;
using WebExpress.Html;
using WebExpress.WebPlugin;
using WebExpress.UI.WebAttribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebPage;
using static WebExpress.Internationalization.InternationalizationManager;

namespace ViLa.WebComponent
{
    [Section(Section.FooterPrimary)]
    [Application("ViLa")]
    public sealed class ComponentFooterVersion : ComponentControlText
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentFooterVersion()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IComponentContext context, IPage page)
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
            var webexpress = PluginManager.Plugins.Where(x => x.PluginId == "webexpress.ui").FirstOrDefault();
            var plugin = PluginManager.Plugins.Where(x => x.Assembly == GetType().Assembly).FirstOrDefault();

            Text = string.Format
            (
                I18N(context.Culture, "vila:app.version.label"),
                I18N(context.Culture, plugin?.PluginName),
                plugin?.Version,
                webexpress?.PluginName,
                webexpress?.Version
            );

            return base.Render(context);
        }
    }
}
