using System.Linq;
using ViLa.Controls;
using ViLa.Model;
using WebExpress.UI.Controls;

namespace ViLa.Pages
{
    public sealed class PageHistory : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageHistory()
            : base("Verlauf")
        {
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

            var history = ViewModel.Instance.GetHistoryMeasurementLogs();

            Main.Content.Add(new ControlText()
            {
                Text = "Messprotokolle",
                Format = TypeFormatText.H1
            });

            var table = new ControlTable();
            table.AddColumn("Datum", new PropertyIcon(TypeIcon.Calendar), TypesLayoutTableRow.Info);
            table.AddColumn("Ladezeit", new PropertyIcon(TypeIcon.Stopwatch), TypesLayoutTableRow.Info);
            table.AddColumn("Verbrauch", new PropertyIcon(TypeIcon.TachometerAlt), TypesLayoutTableRow.Info);
            table.AddColumn("Kosten", new PropertyIcon(TypeIcon.EuroSign), TypesLayoutTableRow.Info);
            table.AddColumn("");

            foreach (var measurementLog in history.OrderByDescending(x => x.From))
            {
                var row = new ControlTableRow() { };
                row.Cells.Add(new ControlText() { Text = string.Format("{0}", measurementLog.From.ToString("dd.MM.yyyy")) });
                row.Cells.Add(new ControlText() { Text = string.Format("{0} - {1} Uhr", measurementLog.From.ToShortTimeString(), measurementLog.Till.ToShortTimeString()) });
                row.Cells.Add(new ControlText() { Text = string.Format("{0:F2} kWh", measurementLog.Power) });
                row.Cells.Add(new ControlText() { Text = string.Format("{0:F2} €", measurementLog.Cost) });
                row.Cells.Add(new ControlLink() { Text = "Details", Uri = Uri.Append(measurementLog.ID) });

                table.Rows.Add(row);
            }

            Main.Content.Add(table);
        }
    }
}
