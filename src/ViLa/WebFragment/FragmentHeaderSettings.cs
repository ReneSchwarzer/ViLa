﻿using ViLa.WebPage;
using ViLa.WebPageSetting;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebAttribute;
using WebExpress.WebComponent;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebFragment
{
    [Section(Section.AppSettingsPrimary)]
    [Module<Module>]
    [Cache]
    public sealed class FragmentHeaderSettings : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentHeaderSettings()
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
            Text = "vila:vila.setting.label";
            Icon = new PropertyIcon(TypeIcon.Cog);
            Uri = ComponentManager.SitemapManager.GetUri<PageSettingGeneral>();
            Active = page is IPageSettings ? TypeActive.Active : TypeActive.None;
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
