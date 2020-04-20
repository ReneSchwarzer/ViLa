using ViLa.Controls;
using ViLa.Model;
using WebExpress.Html;
using WebExpress.UI.Controls;
using WebExpress.UI.Pages;

namespace ViLa.Pages
{
    public class PageBase : PageTemplateWebApp
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="title">Der Titel der Seite</param>
        public PageBase(string title)
            : base()
        {
            Title = "ViLa";

            if (!string.IsNullOrWhiteSpace(title))
            {
                Title += " - " + title;
            }

            Favicons.Add(new Favicon("/Assets/img/Favicon.png", TypesFavicon.PNG));
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();
            Head.Style = "position: sticky; top: 0; z-index: 99;";
            Head.Content.Add(HamburgerMenu);
            HamburgerMenu.HorizontalAlignment = TypesHorizontalAlignment.Left;
            HamburgerMenu.Image = Uri?.Root.Append("Assets/img/Logo.png");
            HamburgerMenu.Add(new ControlLink(this) { Text = "Home", Icon = Icon.Home, Uri = Uri.Root });
            HamburgerMenu.Add(new ControlLink(this) { Text = "Verlauf", Icon = Icon.ChartBar, Uri = Uri.Root.Append("history") });
            HamburgerMenu.AddSeperator();
            HamburgerMenu.Add(new ControlLink(this) { Text = "Logging", Icon = Icon.Book, Uri = Uri.Root.Append("log") });
            HamburgerMenu.Add(new ControlLink(this) { Text = "Einstellungen", Icon = Icon.Cog, Uri = Uri.Root.Append("settings") });
            HamburgerMenu.AddSeperator();
            HamburgerMenu.Add(new ControlLink(this) { Text = "Hilfe", Icon = Icon.InfoCircle, Uri = Uri.Root.Append("help") });

            // SideBar
            ToolBar = new ControlToolBar(this)
            {
                Class = "sidebar bg-success",
                HorizontalAlignment = TypesHorizontalAlignment.Left
            };

            Head.Content.Add(new ControlPanelCenter(this, new ControlText(this)
            {
                Text = Title,
                Color = TypesTextColor.White,
                Format = TypesTextFormat.H1,
                Size = TypesSize.Default,
                Class = "p-1 mb-0",
                Style = "font-size:190%; height: 50px;"
            }));

            Main.Class = "content";
            PathCtrl.Class = "content";

            Main.Content.Add(new ControlTabMenu(this));
            Main.Content.Add(new ControlLine(this));

            Foot.Content.Add(new ControlText(this, "now")
            {
                Text = string.Format("{0}", ViewModel.Instance.Now),
                Color = TypesTextColor.Muted,
                Format = TypesTextFormat.Center,
                Size = TypesSize.Small
            });
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();
        }
    }
}
