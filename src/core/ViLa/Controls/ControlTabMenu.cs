using ViLa.Pages;
using System;
using System.Collections.Generic;
using System.Text;
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
            Layout = TypesLayoutTab.Pill;
            HorizontalAlignment = TypesTabHorizontalAlignment.Center;

            Items.Add(new ControlLink(Page)
            {
                Text = "Home",
                Url = Page.GetPath(0),
                Class = Page is PageDashboard ? "active" : string.Empty,
                Icon = Icon.Home
            });

            Items.Add(new ControlLink(Page)
            {
                Text = "Verlauf",
                Url = Page.GetPath(0, "history"),
                Class = Page is PageHistory ? "active" : string.Empty,
                Icon = Icon.ChartBar
            });
        }
    }
}
