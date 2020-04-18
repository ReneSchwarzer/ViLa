using ViLa.Pages;
using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace ViLa.Controls
{
    public class ControlTabMenu : ControlTab
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="page">Die zugehörige Seite</param>
        public ControlTabMenu(IPage page)
            : base(page)
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            var root = Page.Uri.Root;

            Layout = TypesLayoutTab.Pill;
            HorizontalAlignment = TypesTabHorizontalAlignment.Center;

            Items.Add(new ControlLink(Page)
            {
                Text = "Home",
                Uri = root,
                Class = Page is PageDashboard || !Page.Uri.ContainsSegemtID("Verlauf") ? "active" : string.Empty,
                Icon = Icon.Home
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Verlauf",
                Uri = root.Append("history"),
                Class = Page is PageHistory || Page.Uri.ContainsSegemtID("Verlauf") ? "active" : string.Empty,
                Icon = Icon.ChartBar
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Einstellungen",
                Uri = Page.Uri.Root.Append("settings"),
                Class = Page is PageSettings ? "active" : string.Empty,
                Icon = Icon.Cog
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Logging",
                Uri = Page.Uri.Root.Append("log"),
                Class = Page is PageLog ? "active" : string.Empty,
                Icon = Icon.Book
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Hilfe",
                Uri = Page.Uri.Root.Append("help"),
                Class = Page is PageHelp ? "active" : string.Empty,
                Icon = Icon.InfoCircle
            });
        }
    }
}
