using ViLa.Model;
using ViLa.Pages;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebExpress.Pages;
using WebExpress.Workers;

namespace ViLa
{
    public class ViLaPlugin : WebExpress.Plugins.Plugin
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ViLaPlugin()
        : base("ViLa", "/Assets/img/ViLa.svg")
        {
        }

        /// <summary>
        /// Initialisierung des Prozesszustandes. Hier können z.B. verwaltete Ressourcen geladen werden. 
        /// </summary>
        /// <param name="configFileName">Der Dateiname der Konfiguration oder null</param>
        public override void Init(string configFileName = null)
        {
            base.Init(configFileName);

            ViewModel.Instance.Context = Context;
            ViewModel.Instance.Init();
            Context.Log.Info(MethodBase.GetCurrentMethod(), "ViLaPlugin initialisierung");

            Register(new WorkerFile(new Path(Context, "", "Assets/.*"), Context.AssetBaseFolder));
            Register(new WorkerFile(new Path(Context, "", "measurements/.*"), Context.AssetBaseFolder));

            var root = new VariationPath(Context, "home", new PathItem("Home"));
            var history = new VariationPath(root, "history", new PathItem("Verlauf", "history"));
            var help = new VariationPath(root, "help", new PathItem("Hilfe", "help"));
            var on = new VariationPath(root, "on", new PathItem("On", "on"));
            var off = new VariationPath(root, "off", new PathItem("Off", "off"));
            var log = new VariationPath(root, "log", new PathItem("Logging", "log"));
            var debug = new VariationPath(root, "debug", new PathItem("Debug", "debug"));
            var settings = new VariationPath(root, "settings", new PathItem("Einstellungen", "settings"));
            var api = new VariationPath(root, "api", new PathItem("API", "api"));
            
            var details = new VariationPath(root, "details", new PathItemVariable("Details", "id", "(([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}))"));
            var del = new VariationPath(details, "del", new PathItem("Löschen", "del"));
            var archive = new VariationPath(details, "archive", new PathItem("Archivieren", "archive"));
            var details_his = new VariationPath(history, "details", new PathItemVariable("Details", "id", "(([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}))"));
            var his_del = new VariationPath(details_his, "del", new PathItem("Löschen", "del"));
            var his_archive = new VariationPath(details_his, "archive", new PathItem("Archivieren", "archive"));


            root.GetUrls("Home").ForEach(x => Register(new WorkerPage<PageDashboard>(x) { }));
            history.GetUrls("Verlauf").ForEach(x => Register(new WorkerPage<PageHistory>(x) { }));
            help.GetUrls("Hilfe").ForEach(x => Register(new WorkerPage<PageHelp>(x) { }));
            on.GetUrls("On").ForEach(x => Register(new WorkerPage<PageOn>(x) { }));
            off.GetUrls("Off").ForEach(x => Register(new WorkerPage<PageOff>(x) { }));
            log.GetUrls("Logging").ForEach(x => Register(new WorkerPage<PageLog>(x) { }));
            debug.GetUrls("Debug").ForEach(x => Register(new WorkerPage<PageDebug>(x) { }));
            settings.GetUrls("Einstellungen").ForEach(x => Register(new WorkerPage<PageSettings>(x) { }));
            api.GetUrls("API").ForEach(x => Register(new WorkerPage<PageApiBase>(x) { }));
            details.GetUrls("Details").ForEach(x => Register(new WorkerPage<PageDetails>(x) { }));
            details_his.GetUrls("Details").ForEach(x => Register(new WorkerPage<PageDetails>(x) { }));
            del.GetUrls("Löschen").ForEach(x => Register(new WorkerPage<PageDel>(x) { }));
            archive.GetUrls("Archivieren").ForEach(x => Register(new WorkerPage<PageArchive>(x) { }));
            his_del.GetUrls("Löschen").ForEach(x => Register(new WorkerPage<PageDel>(x) { }));
            his_archive.GetUrls("Archivieren").ForEach(x => Register(new WorkerPage<PageArchive>(x) { }));

            Task.Run(() => { Run(); });
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, nachdem das Fenster aktiv ist.
        /// </summary>
        private void Run()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            
            // Loop
            while (true)
            {
                try
                {
                    Update();
                }
                finally
                {
                    Thread.Sleep(1);
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
