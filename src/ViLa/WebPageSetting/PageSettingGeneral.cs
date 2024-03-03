using ViLa.WebControl;
using ViLa.WebPage;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebResource;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebPageSetting
{
    [Title("vila:vila.setting.label")]
    [Segment("settings", "vila:vila.setting.label")]
    [ContextPath("/settings")]
    [Module<Module>]
    [SettingSection(SettingSection.Preferences)]
    [SettingIcon(TypeIcon.Wrench)]
    [SettingGroup("vila:vila.setting.general.label")]
    [SettingContext("webexpress.webapp:setting.tab.general.label")]
    public sealed class PageSettingGeneral : PageWebAppSetting, IPageSettings, IScope
    {
        /// <summary>
        /// Liefert das Formular
        /// </summary>
        private ControlFormSetting Form { get; } = new ControlFormSetting()
        {

        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingGeneral()
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
