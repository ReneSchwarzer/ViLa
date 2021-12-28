using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.WebApp.WebPage;
using WebExpress.WebResource;

namespace ViLa.WebPage
{
    [ID("Debug")]
    [Title("vila:vila.debug.label")]
    [Segment("debug", "vila:vila.debug.label")]
    [Path("/")]
    [Module("ViLa")]
    [Context("general")]
    [Context("debug")]
    public sealed class PageDebug : PageWebApp
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageDebug()
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
            base.Process(context);

            ViewModel.Instance.Settings.DebugMode = !ViewModel.Instance.Settings.DebugMode;
            ViewModel.Instance.SaveSettings();

            Redirecting(context.Uri.Root.Append("/log"));
        }
    }
}
