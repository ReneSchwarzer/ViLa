using ViLa.WebCondition;
using ViLa.WebControl;
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
    [Title("vila:vila.setting.timecontrol.label")]
    [Segment("timecontrol", "vila:vila.setting.timecontrol.label")]
    [ContextPath("/")]
    [Parent<PageSettingGeneral>]
    [Module<Module>]
    [SettingSection(SettingSection.Preferences)]
    [SettingIcon(TypeIcon.Clock)]
    [SettingGroup("vila:vila.setting.general.label")]
    [SettingContext("webexpress.webapp:setting.tab.general.label")]
    [Condition<ConditionTimeControl>]
    public sealed class PageSettingTimeControl : PageWebAppSetting, IPageSettings, IScope
    {
        /// <summary>
        /// Liefert das Formular
        /// </summary>
        private ControlFormTimeControl Form { get; } = new ControlFormTimeControl()
        {

        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingTimeControl()
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
