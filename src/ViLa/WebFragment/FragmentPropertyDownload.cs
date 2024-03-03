using ViLa.WebPage;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace ViLa.WebFragment
{
    [Section(Section.PropertyPreferences)]
    [Module<Module>]
    [Scope<PageDetails>]
    public sealed class FragmentPropertyDownload : FragmentControlButtonLink
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentPropertyDownload()
            : base("download_btn")
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

            Text = "vila:vila.download.label";
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
            Icon = new PropertyIcon(TypeIcon.Download);
            TextColor = new PropertyColorText(TypeColorText.Light);
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            var id = context.Request.GetParameter("id");

            Uri = context.ApplicationContext.ContextPath.Append("measurements/" + id.Value + ".xml");

            return base.Render(context);
        }
    }
}
