using System.Reflection;
using WebExpress;
using WebExpress.Plugins;

namespace ViLa
{
    public class ViLaFactory : PluginFactory
    {
        /// <summary>
        /// Liefert oder setzt die ID
        /// </summary>
        public override string ArtifactID => "ViLa";

        /// <summary>
        /// Liefert oder setzt die HerstellerID
        /// </summary>
        public override string ManufacturerID => "org.WebExpress";

        /// <summary>
        /// Liefert oder setzt die Beschreibung
        /// </summary>
        public override string Description => "Eine einfache Wallboxsteuerung.";

        /// <summary>
        /// Liefert oder setzt den Namen
        /// </summary>
        public override string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Liefert den Dateinamen der Konfigurationsdatei
        /// </summary>
        public override string ConfigFileName => "vila.config.xml";

        /// <summary>
        /// Erstellt eine neue Instanz eines Prozesszustandes
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="configFileName">Der Dateiname der Konfiguration oder null</param>
        /// <returns>Die Instanz des Plugins</returns>
        public override IPlugin Create(HttpServerContext context, string configFileName)
        {
            var plugin = Create<ViLaPlugin>(context, configFileName);

            return plugin;
        }
    }
}
