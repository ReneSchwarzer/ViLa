using ViLa.WebControl;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Attribute;
using WebExpress.WebApp.WebResource.PageSetting;

namespace ViLa.WebResource.PageSetting
{
    [ID("SettingGeneral")]
    [Title("vila.setting.label")]
    [Segment("settings", "vila.setting.label")]
    [Path("/")]
    [Module("ViLa")]
    [SettingSection(SettingSection.Preferences)]
    [SettingIcon(TypeIcon.Wrench)]
    [SettingGroup("vila.setting.general.label")]
    [SettingContext("vila.setting.general.label")]
    [Context("general")]
    [Context("setting")]
    public sealed class PageSettingGeneral : PageTemplateWebAppSetting, IPageSettings
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingGeneral()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();

            Favicons.Add(new Favicon(Uri.Root.Append("/assets/img/Favicon.png").ToString(), TypeFavicon.PNG));
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            Content.Primary.Add(new ControlFormSetting()
            {

            });

            base.Process();
        }
    }
}
