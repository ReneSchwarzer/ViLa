using ViLa.Model;
using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace ViLa.Pages
{
    public class PageStatusInternalServerError : PageBase, IPageStatus
    {
        /// <summary>
        /// Liefert oder setzt die Stausnachricht
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageStatusInternalServerError()
            : base("Serverfehler")
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

            Content.Content.Add(new ControlText()
            {
                Text = StatusMessage
            }); 
        }
    }
}
