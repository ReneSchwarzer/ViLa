using ViLa.WebPage;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace ViLa.WebFragment
{
    [Section(Section.AppHelpPrimary)]
    [Module<Module>]
    [Cache]
    public sealed class FragmentHeaderHelp : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentHeaderHelp()
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

            TextColor = new PropertyColorText(TypeColorText.Dark);
            Text = "vila:vila.help.label";
            Icon = new PropertyIcon(TypeIcon.InfoCircle);
            Uri = ComponentManager.SitemapManager.GetUri<PageHelp>();
            Active = page is IPageHelp ? TypeActive.Active : TypeActive.None;
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            return base.Render(context);
        }
    }
}
