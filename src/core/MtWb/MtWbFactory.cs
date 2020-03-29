using System;
using WebExpress;
using WebExpress.Plugins;

namespace MtWb
{
    public class MtWbFactory : PluginFactory
    {
        /// <summary>
        /// Erstellt eine neue Instanz eines Prozesszustandes
        /// </summary>
        /// <param name="host">Der Benutzer</param>
        /// <param name="configFileName">Der Dateiname der Konfiguration oder null</param>
        /// <returns>Die Instanz des Prozesszustandes</returns>
        public override IPlugin Create(IHost host, string configFileName)
        {
            var plugin = Create<MtWbPlugin>(host, configFileName);
            
            return plugin;
        }
    }
}
