using System;
using System.Collections.Generic;
using System.Text;
using WebExpress.Pages;
using WebExpress.UI.Controls;
using WebServer.Html;

namespace MtWb.Controls
{
    public class ControlMenu : ControlDropdownMenu
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="page">Die zugehörige Seite</param>
        public ControlMenu(IPage page)
            : base(page, null)
        {
            
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Icon = Icon.Bars;
            Text = "";
            Layout = TypesLayoutButton.Primary;

            Items.Add(new ControlLink(Page) { Text = "Home", Icon = Icon.Home, Url = Page.GetUrl(0) });
            Items.Add(new ControlDropdownMenuDivider(Page) { });
 
            if (Page.Url == Page.GetUrl("/"))
            {
                Items.Add(new ControlDropdownMenuDivider(Page) { });
                Items.Add(new ControlLink(Page) { Text = "Export", Icon = Icon.Download, Url = Page.GetUrl(0) });
                Items.Add(new ControlDropdownMenuDivider(Page) { });
                Items.Add(new ControlLink(Page) { Text = "Hilfe", Icon = Icon.InfoCircle, Url = Page.GetUrl(0, "help") });
            }
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode ToHtml()
        {
            Init();

            return base.ToHtml();
        }
    }
}
