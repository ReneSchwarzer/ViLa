using ViLa.Controls;

namespace ViLa.Pages
{
    public sealed class PageSettings : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageSettings()
            : base("Einstellungen")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();

            Main.Content.Add(new ControlSettingForm()
            {

            });
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();
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
