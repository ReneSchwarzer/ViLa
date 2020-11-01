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

            Main.Content.Add(new ControlText()
            {
                Text = "Willkommen",
                Format = TypeFormatText.H1,
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Five)
            });

            Main.Content.Add(new ControlChargingCard()
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Four, PropertySpacing.Space.None)
            });

            var history = ViewModel.Instance.GetHistoryMeasurementLogs(DateTime.Now.AddMonths(-1));

            if (history.Count > 0)
            {
                Main.Content.Add(new ControlText()
                {
                    Text = "Letzte Ladungen",
                    Format = TypeFormatText.H1,
                    Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Five, PropertySpacing.Space.None)
                });

                var grid = new ControlPanelGrid() { Fluid =  TypePanelContainer.Fluid };

                foreach (var measurementLog in history)
                {
                    var card = new ControlMeasurementLogCard()
                    {
                        MeasurementLog = measurementLog,
                        GridColumn = new PropertyGrid(TypeDevice.Medium, 3)
                    };

                    grid.Content.Add(card);
                }

                Main.Content.Add(grid);
            }
        }
    }
}
