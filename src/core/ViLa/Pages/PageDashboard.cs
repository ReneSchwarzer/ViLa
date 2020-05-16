using System;
using ViLa.Controls;
using ViLa.Model;
using WebExpress.UI.Controls;

namespace ViLa.Pages
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
            HeaderScriptLinks.Add("/Assets/js/vila.dashboard.js");
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
                Format = TypeFormatText.H1,
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Five)
            });

            Main.Content.Add(new ControlChargingCard(this)
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Four, PropertySpacing.Space.None)
            });

            var history = ViewModel.Instance.GetHistoryMeasurementLogs(DateTime.Now.AddMonths(-1));

            if (history.Count > 0)
            {
                Main.Content.Add(new ControlText(this)
                {
                    Text = "Letzte Ladungen",
                    Format = TypeFormatText.H1,
                    Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Five, PropertySpacing.Space.None)
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
    }
}
