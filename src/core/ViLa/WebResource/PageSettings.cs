using ViLa.WebControl;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.WebApp.WebResource;

namespace ViLa.WebResource
{
    [ID("Setting")]
    [Title("vila.setting.label")]
    [Segment("settings", "vila.setting.label")]
    [Path("/")]
    [Module("ViLa")]
    [Context("general")]
    [Context("setting")]
    public sealed class PageSettings : PageTemplateWebApp, IPageSettings
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettings()
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
