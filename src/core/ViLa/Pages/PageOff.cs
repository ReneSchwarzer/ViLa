using ViLa.Model;

namespace ViLa.Pages
{
    public class PageOff : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageOff()
            : base("Off")
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

            ViewModel.Instance.StopsCharging();

            Redirecting(Uri.Root);
        }

        /// <summary>
        /// In String konvertieren
        /// </summary>
        /// <returns>Das Objekt als String</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
