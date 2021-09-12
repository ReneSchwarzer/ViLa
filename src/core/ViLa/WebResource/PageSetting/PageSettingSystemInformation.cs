using WebExpress.Attribute;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Attribute;
using WebExpress.WebApp.WebResource.PageSetting;

namespace ViLa.WebResource.PageSetting
{
    [ID("SettingSystemInformation")]
    [Title("vila.setting.systeminformation.label")]
    [Segment("systeminformation", "vila.setting.systeminformation.label")]
    [Path("/SettingGeneral")]
    [SettingSection(SettingSection.Secondary)]
    [SettingIcon(TypeIcon.InfoCircle)]
    [SettingGroup("vila.setting.system.label")]
    [SettingContext("vila.setting.general.label")]
    [Module("ViLa")]
    [Context("admin")]
    public sealed class PageSettingSystemInformation : PageTemplateWebAppSettingSystemInformation
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingSystemInformation()
        {
        }
    }
}
