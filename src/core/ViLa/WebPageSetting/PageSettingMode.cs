using ViLa.WebControl;
using ViLa.WebPage;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebAttribute;
using WebExpress.WebResource;

namespace ViLa.WebPageSetting
{
    [Id("SettingMode")]
    [Title("vila:vila.setting.mode.label")]
    [Segment("mode", "vila:vila.setting.mode.label")]
    [Path("/settings")]
    [Module("ViLa")]
    [SettingSection(SettingSection.Preferences)]
    [SettingIcon(TypeIcon.Sliders)]
    [SettingGroup("vila:vila.setting.general.label")]
    [SettingContext("webexpress.webapp:setting.tab.general.label")]
    [Context("general")]
    [Context("setting")]
    public sealed class PageSettingMode : PageWebAppSetting, IPageSettings
    {
        /// <summary>
        /// Liefert das Formular
        /// </summary>
        private ControlFormMode Form = new ControlFormMode()
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
