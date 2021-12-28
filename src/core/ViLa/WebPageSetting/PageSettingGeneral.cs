using ViLa.WebControl;
using ViLa.WebPage;
using WebExpress.Attribute;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Attribute;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebResource;

namespace ViLa.WebPageSetting
{
    [ID("SettingGeneral")]
    [Title("vila:vila.setting.label")]
    [Segment("settings", "vila:vila.setting.label")]
    [Path("/")]
    [Module("ViLa")]
    [SettingSection(SettingSection.Preferences)]
    [SettingIcon(TypeIcon.Wrench)]
    [SettingGroup("vila:vila.setting.general.label")]
    [SettingContext("webexpress.webapp:setting.tab.general.label")]
    [Context("general")]
    [Context("setting")]
    public sealed class PageSettingGeneral : PageWebAppSetting, IPageSettings
    {
        /// <summary>
        /// Liefert das Formular
        /// </summary>
        private ControlFormSetting Form = new ControlFormSetting()
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
