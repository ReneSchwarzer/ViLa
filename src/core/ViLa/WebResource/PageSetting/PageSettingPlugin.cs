using WebExpress.Attribute;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Attribute;
using WebExpress.WebApp.WebResource.PageSetting;

namespace ViLa.WebResource.PageSetting
{
    [ID("SettingPlugin")]
    [Title("vila.setting.plugin.label")]
    [Segment("plugin", "vila.setting.plugin.label")]
    [Path("/SettingGeneral")]
    [SettingSection(SettingSection.Secondary)]
    [SettingIcon(TypeIcon.PuzzlePiece)]
    [SettingGroup("vila.setting.system.label")]
    [SettingContext("vila.setting.general.label")]
    [Module("ViLa")]
    [Context("admin")]
    public sealed class PageSettingPlugin : PageTemplateWebAppSettingPlugin
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingPlugin()
        {
        }
    }
}
