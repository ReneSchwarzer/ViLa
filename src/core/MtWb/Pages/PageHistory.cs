using MtWb.Model;
using System.Linq;
using WebExpress.UI.Controls;

namespace MtWb.Pages
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

            Main.Content.Add(new ControlText(this)
            {
                Text = "Messprotokolle",
                Format = TypesTextFormat.Center,
                Color = TypesTextColor.Primary,
                Class = "m-3"
            });

            var table = new ControlTable(this);
            table.AddColumn("Datum", Icon.Calendar, TypesLayoutTableRow.Info);
            table.AddColumn("Ladezeit", Icon.Stopwatch, TypesLayoutTableRow.Info);
            table.AddColumn("Verbrauch", Icon.TachometerAlt, TypesLayoutTableRow.Info);
            table.AddColumn("Kosten", Icon.EuroSign, TypesLayoutTableRow.Info);
            table.AddColumn("");

            foreach (var measurementLog in history.OrderByDescending(x => x.From))
            {
                var row = new ControlTableRow(this) { };
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", measurementLog.From.ToShortDateString()) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0} - {1} Uhr", measurementLog.From.ToShortTimeString(), measurementLog.Till.ToShortTimeString()) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0} kWh", measurementLog.Power) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0} €", measurementLog.Cost) });
                row.Cells.Add(new ControlLink(this) { Text = "Details", Url = GetPath(0, measurementLog.ID) });

                table.Rows.Add(row);
            }

            Main.Content.Add(table);
        }
    }
}
