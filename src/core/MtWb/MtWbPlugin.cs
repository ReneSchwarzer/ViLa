using MtWb.Model;
using MtWb.Pages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WebExpress.Pages;
using WebExpress.Workers;

namespace MtWb
{
    public class MtWbPlugin : WebExpress.Plugins.Plugin
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public MtWbPlugin()
        : base("MtWb")
        {
        }

        /// <summary>
        /// Initialisierung des Prozesszustandes. Hier können z.B. verwaltete Ressourcen geladen werden. 
        /// </summary>
        /// <param name="configFileName">Der Dateiname der Konfiguration oder null</param>
        public override void Init(string configFileName = null)
        {
            ViewModel.Instance.Host = Host;
            ViewModel.Instance.Init();
            Host.Context.Log.Info(MethodBase.GetCurrentMethod(), "MtWbPlugin initialisierung");

            Register(new WorkerFile(new Path("", "Assets/.*"), Host.Context.AssetBaseFolder));

            var root = new VariationPath("home", new PathItem("Home"));
            var help = new VariationPath(root, "help", new PathItem("Hilfe", "help"));
 
            root.GetUrls("Home").ForEach(x => Register(new WorkerPage<PageDashboard>(x) { }));
             help.GetUrls("Hilfe").ForEach(x => Register(new WorkerPage<PageHelp>(x) { }));
 
        }
    }
}
