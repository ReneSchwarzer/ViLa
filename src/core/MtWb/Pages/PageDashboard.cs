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
            HeaderScriptLinks.Add("/Assets/js/Chart.min.js");
            HeaderScriptLinks.Add("/Assets/js/dashboard.js");
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            Main.Content.Add(new ControlText(this)
            {
                Text = "Willkommen",
                Format = TypesTextFormat.H1,
                Class = "mb-5"
            });

            Main.Content.Add(new ControlChargingCard(this)
            {
                Class = "ml-4 mr-4"
            });

            var history = ViewModel.Instance.GetHistoryMeasurementLogs(DateTime.Now.AddMonths(-1));

            if (history.Count > 0)
            {
                Main.Content.Add(new ControlText(this)
                {
                    Text = "Letzte Ladungen",
                    Format = TypesTextFormat.H1,
                    Class = "mt-5"
                });

                var i = 0;
                var grid = new ControlGrid(this) { Fluid = false };

                foreach (var measurementLog in history)
                {
                    var card = new ControlMeasurementLogCard(this)
                    {
                        MeasurementLog = measurementLog
                    };

                    grid.Add(i++, card);
                }

                Main.Content.Add(grid);
            }
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
