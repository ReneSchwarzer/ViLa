using System.Linq;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebResource;

namespace ViLa.WebResource
{
    [ID("History")]
    [Title("vila.history.label")]
    [Segment("history", "vila.history.label")]
    [Path("/")]
    [Module("ViLa")]
    [Context("general")]
    [Context("history")]
    public sealed class PageHistory : PageTemplateWebApp, IPageHistory
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
        public override void Initialization()
        {
            base.Initialization();

            Favicons.Add(new Favicon(Uri.Root.Append("/assets/img/Favicon.png").ToString(), TypeFavicon.PNG));
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

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
                row.Cells.Add(new ControlLink() { Text = "Details", Uri = Uri.Append(measurementLog.ID) });

                table.Rows.Add(row);
            }

            Content.Primary.Add(table);
        }
    }
}
