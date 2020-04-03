using MtWb.Model;
using MtWb.Pages;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
            var on = new VariationPath(root, "on", new PathItem("On", "on"));
            var off = new VariationPath(root, "off", new PathItem("Off", "off"));
            var log = new VariationPath(root, "log", new PathItem("Logging", "log"));
            var debug = new VariationPath(root, "debug", new PathItem("Debug", "debug"));

            root.GetUrls("Home").ForEach(x => Register(new WorkerPage<PageDashboard>(x) { }));
            help.GetUrls("Hilfe").ForEach(x => Register(new WorkerPage<PageHelp>(x) { }));
            on.GetUrls("On").ForEach(x => Register(new WorkerPage<PageOn>(x) { }));
            off.GetUrls("Off").ForEach(x => Register(new WorkerPage<PageOff>(x) { }));
            log.GetUrls("Logging").ForEach(x => Register(new WorkerPage<PageLog>(x) { }));
            debug.GetUrls("Debug").ForEach(x => Register(new WorkerPage<PageDebug>(x) { }));

            Task.Run(() => { Run(); });
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, nachdem das Fenster aktiv ist.
        /// </summary>
        private void Run()
        {
            // Loop
            while (true)
            {
                try
                {
                    Update();
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, nachdem das Fenster aktiv ist.
        /// </summary>
        private void Update()
        {
            ViewModel.Instance.Update();
        }
    }
}
