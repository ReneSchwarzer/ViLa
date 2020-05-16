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

            Layout = TypeLayoutTab.Pill;
            HorizontalAlignment = TypeHorizontalAlignmentTab.Center;

            Items.Add(new ControlLink(Page)
            {
                Text = "Home",
                Uri = root,
                Active = (Page is PageDashboard || Page is PageDetails) && !Page.Uri.ContainsSegemtID("Verlauf") ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.Home)
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Verlauf",
                Uri = root.Append("history"),
                Active = Page is PageHistory || Page.Uri.ContainsSegemtID("Verlauf") ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.ChartBar)
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Einstellungen",
                Uri = Page.Uri.Root.Append("settings"),
                Active = Page is PageSettings ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.Cog)
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Logging",
                Uri = Page.Uri.Root.Append("log"),
                Active = Page is PageLog ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.Book)
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Hilfe",
                Uri = Page.Uri.Root.Append("help"),
                Active = Page is PageHelp ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.InfoCircle)
            });
        }
    }
}
