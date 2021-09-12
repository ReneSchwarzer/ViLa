using WebExpress.Attribute;
using WebExpress.WebApp.WebResource.PageSetting;

namespace ViLa.WebResource.PageSetting
{
    [ID("SettingLogDownload")]
    [Segment("download", "")]
    [Path("/SettingGeneral/SettingLog")]
    [Module("ViLa")]
    [Context("admin")]
    public sealed class PageSettingLogDownload : PageTemplateWebAppSettingLogDownload
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettingLogDownload()
        {

        }
    }
}
