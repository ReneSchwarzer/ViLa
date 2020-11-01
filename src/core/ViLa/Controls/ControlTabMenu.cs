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
            : base()
        {
            Init(page);
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="page">Die zugehörige Seite</param>
        private void Init(IPage page)
        {
            var root = page.Uri.Root;

            Layout = TypeLayoutTab.Pill;
            HorizontalAlignment = TypeHorizontalAlignmentTab.Center;

            Items.Add(new ControlLink()
            {
                Text = "Home",
                Uri = root,
                Active = (page is PageDashboard || page is PageDetails) && !page.Uri.ContainsSegemtID("Verlauf") ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.Home)
            });

            Items.Add(new ControlLink()
            {
                Text = "Verlauf",
                Uri = root.Append("history"),
                Active = page is PageHistory || page.Uri.ContainsSegemtID("Verlauf") ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.ChartBar)
            });

            Items.Add(new ControlLink()
            {
                Text = "Einstellungen",
                Uri = page.Uri.Root.Append("settings"),
                Active = page is PageSettings ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.Cog)
            });

            Items.Add(new ControlLink()
            {
                Text = "Logging",
                Uri = page.Uri.Root.Append("log"),
                Active = page is PageLog ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.Book)
            });

            Items.Add(new ControlLink()
            {
                Text = "Hilfe",
                Uri = page.Uri.Root.Append("help"),
                Active = page is PageHelp ? TypeActive.Active : TypeActive.None,
                Icon = new PropertyIcon(TypeIcon.InfoCircle)
            });
        }
    }
}
