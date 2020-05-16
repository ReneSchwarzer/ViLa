using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ViLa.Model;
using ViLa.Pages;
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

            var siteMap = new SiteMap(Context);

            siteMap.AddPage("Assets", "Assets", (x) => new WorkerFile(x, Context.AssetBaseFolder));
            siteMap.AddPage("Messprotokolle", "measurements", (x) => new WorkerFile(x, Context.AssetBaseFolder));

            siteMap.AddPage("Home", (x) => new WorkerPage<PageDashboard>(x));
            siteMap.AddPage("Dashboard", "dashboard", (x) => new WorkerPage<PageDashboard>(x));
            siteMap.AddPage("Verlauf", "history", (x) => new WorkerPage<PageHistory>(x));
            siteMap.AddPage("Hilfe", "help", (x) => new WorkerPage<PageHelp>(x));
            siteMap.AddPage("On", "on", (x) => new WorkerPage<PageOn>(x));
            siteMap.AddPage("Off", "off", (x) => new WorkerPage<PageOff>(x));
            siteMap.AddPage("Logging", "log", (x) => new WorkerPage<PageLog>(x));
            siteMap.AddPage("Debug", "debug", (x) => new WorkerPage<PageDebug>(x));
            siteMap.AddPage("Einstellungen", "settings", (x) => new WorkerPage<PageSettings>(x));
            siteMap.AddPage("API", "api", (x) => new WorkerPage<PageApiBase>(x));
            siteMap.AddPage("Details", "details", (x) => new WorkerPage<PageDetails>(x));
            siteMap.AddPage("Löschen", "del", (x) => new WorkerPage<PageDel>(x));
            siteMap.AddPage("Archivieren", "archive", (x) => new WorkerPage<PageArchive>(x));

            siteMap.AddPathSegmentVariable
            (
                "Details",
                new UriPathSegmentDynamicDisplay
                (
                    new UriPathSegmentDynamicDisplayText("Details"),
                    new UriPathSegmentDynamicDisplayText(" "),
                    new UriPathSegmentDynamicDisplayReference("id5")
                ),
                "-",
                new UriPathSegmentVariable("id1", "([0-9A-Fa-f]{8})"),
                new UriPathSegmentVariable("id2", "([0-9A-Fa-f]{4})"),
                new UriPathSegmentVariable("id3", "([0-9A-Fa-f]{4})"),
                new UriPathSegmentVariable("id4", "([0-9A-Fa-f]{4})"),
                new UriPathSegmentVariable("id5", "([0-9A-Fa-f]{12})")
            );

            siteMap.AddPath("Assets", true);
            siteMap.AddPath("Messprotokolle", true);

            siteMap.AddPath("Home");
            siteMap.AddPath("Home/Verlauf");
            siteMap.AddPath("Home/Hilfe");
            siteMap.AddPath("Home/On");
            siteMap.AddPath("Home/Off");
            siteMap.AddPath("Home/Logging");
            siteMap.AddPath("Home/Debug");
            siteMap.AddPath("Home/Einstellungen");
            siteMap.AddPath("Home/API");
            siteMap.AddPath("Home/Details");
            siteMap.AddPath("Home/Details/Löschen");
            siteMap.AddPath("Home/Details/Archivieren");
            siteMap.AddPath("Home/Verlauf/Details");
            siteMap.AddPath("Home/Verlauf/Details/Löschen");
            siteMap.AddPath("Home/Verlauf/Details/Archivieren");

            Register(siteMap);

            RegisterStatusPage(404, () => new PageStatusNotFound());
            RegisterStatusPage(500, () => new PageStatusInternalServerError());

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
