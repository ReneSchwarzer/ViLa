using MtWb.Model;
using WebExpress.Html;
using WebExpress.Pages;
using WebExpress.UI.Controls;
using WebExpress.UI.Pages;

namespace MtWb.Pages
{
    public class PageBase : PageTemplateWebApp
    {
        ///// <summary>
        ///// Liefert oder setzt die Toolbar
        ///// </summary>
        //public ControlToolBar ToolBar { get; set; }
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="title">Der Titel der Seite</param>
        public PageBase(string title)
            : base()
        {
            Title = "Makes the Wallbox better";

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

            Head.Content.Add(new ControlImage(this)
            {
                Source = new Path(Context, "/Assets/img/Logo.png"),
                Height = 50,
                HorizontalAlignment = TypesHorizontalAlignment.Left
            });

            ToolBar = new ControlToolBar(this)
            {
                Class = "p-2 bg-light"

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

            Main.Class = "pl-3 pr-3";

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
