using ViLa.Model;
using WebExpress.Pages;

namespace ViLa.Pages
{
    public class PageStatusNotFound : PageBase, IPageStatus
    {
        /// <summary>
        /// Liefert oder setzt die Stausnachricht
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageStatusNotFound()
            : base("Seite nicht gefunden")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();

        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            Redirecting(Uri.Root);
        }
    }
}
