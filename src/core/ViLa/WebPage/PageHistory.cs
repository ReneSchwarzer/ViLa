using System.Linq;
using ViLa.Model;
using WebExpress.WebAttribute;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebResource;

namespace ViLa.WebPage
{
    [ID("History")]
    [Title("vila:vila.history.label")]
    [Segment("history", "vila:vila.history.label")]
    [Path("/")]
    [Module("ViLa")]
    [Context("general")]
    [Context("history")]
    public sealed class PageHistory : PageWebApp, IPageHistory
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageHistory()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        /// <param name="context">Der Kontext zum Rendern der Seite</param>
        public override void Process(RenderContextWebApp context)
        {
            base.Process(context);

            var history = ViewModel.Instance.GetHistoryMeasurementLogs();

            var table = new ControlTable();
            table.AddColumn("Datum", new PropertyIcon(TypeIcon.Calendar), TypesLayoutTableRow.Info);
            table.AddColumn("Ladezeit", new PropertyIcon(TypeIcon.Stopwatch), TypesLayoutTableRow.Info);
            table.AddColumn("Verbrauch", new PropertyIcon(TypeIcon.TachometerAlt), TypesLayoutTableRow.Info);
            table.AddColumn("Kosten", new PropertyIcon(TypeIcon.EuroSign), TypesLayoutTableRow.Info);
            table.AddColumn("");

            foreach (var measurementLog in history.OrderByDescending(x => x.From))
            {
                var row = new ControlTableRow() { };
                row.Cells.Add(new ControlText() { Text = string.Format("{0}", measurementLog.FinalFrom.ToString(Culture.DateTimeFormat.ShortDatePattern)) });
                row.Cells.Add(new ControlText() { Text = string.Format("{0} - {1} Uhr", measurementLog.FinalFrom.ToString(Culture.DateTimeFormat.LongTimePattern), measurementLog.FinalTill.ToString(Culture.DateTimeFormat.LongTimePattern)) });
                row.Cells.Add(new ControlText() { Text = string.Format("{0:F2} kWh", measurementLog.FinalPower) });
                row.Cells.Add(new ControlText() { Text = string.Format("{0:F2} {1}", measurementLog.FinalCost, measurementLog.Currency) });
                row.Cells.Add(new ControlLink() { Text = "Details", Uri = context.Uri.Append(measurementLog.ID) });

                table.Rows.Add(row);
            }

            context.VisualTree.Content.Primary.Add(table);
        }
    }
}
