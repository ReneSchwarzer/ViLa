﻿using ViLa.WebControl;
using ViLa.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebAttribute;
using WebExpress.WebResource;
using WebExpress.WebScope;

namespace ViLa.WebPageSetting
{
    [Title("vila:vila.setting.mode.label")]
    [Segment("mode", "vila:vila.setting.mode.label")]
    [ContextPath("/")]
    [Parent<PageSettingGeneral>]
    [Module<Module>]
    [SettingSection(SettingSection.Preferences)]
    [SettingIcon(TypeIcon.Sliders)]
    [SettingGroup("vila:vila.setting.general.label")]
    [SettingContext("webexpress.webapp:setting.tab.general.label")]
    public sealed class PageSettingMode : PageWebAppSetting, IPageSettings, IScope
    {
        /// <summary>
        /// Liefert das Formular
        /// </summary>
        private ControlFormMode Form { get; } = new ControlFormMode()
        {

        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingMode()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        /// <param name="context">Der Kontext zum Rendern der Seite</param>
        public override void Process(RenderContextWebApp context)
        {
            context.VisualTree.Content.Primary.Add(Form);

            base.Process(context);
        }
    }
}