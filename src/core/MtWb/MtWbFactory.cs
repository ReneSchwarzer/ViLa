using System;
using WebExpress;
using WebExpress.Plugins;

namespace MtWb
{
    public class MtWbFactory : PluginFactory
    {
        /// <summary>
        /// Liefert den Dateinamen der Konfigurationsdatei
        /// </summary>
        public override string ConfigFileName => "mtwb.config.xml";

        /// <summary>
        /// Erstellt eine neue Instanz eines Prozesszustandes
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="configFileName">Der Dateiname der Konfiguration oder null</param>
        /// <returns>Die Instanz des Plugins</returns>
        public override IPlugin Create(HttpServerContext context, string configFileName)
        {
            var plugin = Create<MtWbPlugin>(context, configFileName);
            
            return plugin;
        }
    }
}
