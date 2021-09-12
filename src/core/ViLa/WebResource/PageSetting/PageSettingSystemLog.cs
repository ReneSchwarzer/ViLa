using WebExpress.Attribute;
using WebExpress.Message;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Attribute;
using WebExpress.WebApp.WebResource.PageSetting;

namespace ViLa.WebResource.PageSetting
{
    [ID("SettingLog")]
    [Title("vila.setting.log.label")]
    [Segment("log", "vila.setting.log.label")]
    [Path("/SettingGeneral")]
    [SettingSection(SettingSection.Secondary)]
    [SettingIcon(TypeIcon.FileMedicalAlt)]
    [SettingGroup("vila.setting.system.label")]
    [SettingContext("vila.setting.general.label")]
    [Module("ViLa")]
    [Context("admin")]
    public sealed class PageSettingLog : PageTemplateWebAppSettingLog
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingLog()
        {
        }

        /// <summary>
        /// Vorverarbeitung
        /// </summary>
        /// <param name="request">Die Anfrage</param>
        public override void PreProcess(Request request)
        {
            DownloadUri = Uri.Append("download");

            base.PreProcess(request);
        }
    }
}
