using MtWb.Controls;
using MtWb.Model;
using System;
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

            var i = 0;

            var grid = new ControlGrid(this) { Fluid = false };

            grid.Add(i++, new ControlText(this)
            {
                Text = "Willkommen",
                Format = TypesTextFormat.H1
            });

            grid.Add(i++, new ControlChargingCard(this)
            {
            });

            grid.Add(i++, new ControlText(this)
            {
                Text = "Letzter Monat",
                Format = TypesTextFormat.H1
            });

            foreach (var measurementLog in ViewModel.Instance.GetHistoryMeasurementLogs(DateTime.Now.AddMonths(-1)))
            {
                var card = new ControlMeasurementLogCard(this)
                {
                    MeasurementLog = measurementLog
                };

                grid.Add(i++, card);
            }

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
