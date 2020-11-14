using System.Collections.Generic;
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
            Title = title;
            
            Favicons.Add(new Favicon("/Assets/img/Favicon.png", TypeFavicon.PNG));
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();

            Header.Logo = Uri?.Root.Append("Assets/img/Logo.png");
            Title = Title;
            
            //Header.HamburgerPrimary.Add(new ControlLink() { Text = "Home", Icon = new PropertyIcon(TypeIcon.Home), Uri = Uri.Root });
            //Header.HamburgerPrimary.Add(new ControlLink() { Text = "Verlauf", Icon = new PropertyIcon(TypeIcon.ChartBar), Uri = Uri.Root.Append("history") });
            //Header.HamburgerPrimary.Add(null);
            //Header.HamburgerPrimary.Add(new ControlLink() { Text = "Logging", Icon = new PropertyIcon(TypeIcon.Book), Uri = Uri.Root.Append("log") });
            //Header.HamburgerPrimary.Add(new ControlLink() { Text = "Einstellungen", Icon = new PropertyIcon(TypeIcon.Cog), Uri = Uri.Root.Append("settings") });
            //Header.HamburgerSecondary.Add(new ControlLink() { Text = "Hilfe", Icon = new PropertyIcon(TypeIcon.InfoCircle), Uri = Uri.Root.Append("help") });
                        
            Footer.Content.Add(new ControlText("now")
            {
                Text = string.Format("{0}", ViewModel.Instance.Now),
                TextColor = new PropertyColorText(TypeColorText.Muted),
                Format = TypeFormatText.Center,
                Size = new PropertySizeText(TypeSizeText.Small)
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
