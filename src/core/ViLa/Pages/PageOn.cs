using ViLa.Model;

namespace ViLa.Pages
{
    public class PageOn : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageOn()
            : base("On")
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

            ViewModel.Instance.StartsTheChargingProcess();

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
