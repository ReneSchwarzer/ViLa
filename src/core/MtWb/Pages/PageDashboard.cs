using MtWb.Controls;
using MtWb.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
{
    public class PageDashboard : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageDashboard()
            : base("Überblick")
        {
            //HeaderScriptLinks.Add("/Assets/js/dashboard.js");
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();

            var menu = new ControlMenu(this);  
            ToolBar.Add(menu);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            var grid = new ControlGrid(this) { Fluid = false };
            int i = 0;

            

            Main.Content.Add(grid);
        }

        /// <summary>
        /// In String konvertieren
        /// </summary>
        /// <returns>Das Objekt als String</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
