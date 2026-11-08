using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ViLa.Model;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;
using WebExpress.WebUI.WebEditor;
using WebExpress.WebUI.WebPdf;
using WebExpress.WebUI.WebPdf.Element;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// Exports a completed measurement as its XML record or a printable PDF report.
    /// </summary>
    [Application<Application>]
    public sealed class Export : IRestApi
    {
        /// <summary>
        /// Returns a download for the requested measurement and format.
        /// </summary>
        /// <param name="request">The request supplying the measurement identifier, format and culture.</param>
        /// <returns>The attachment or an error for an invalid request.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(IRequest request)
        {
            if (!Guid.TryParse(request.GetParameter("id")?.Value, out var id))
            {
                return new ResponseBadRequest();
            }
            var log = ViewModel.Instance.GetHistoryMeasurementLog(id.ToString());
            if (log == null)
            {
                return new ResponseNotFound();
            }
            var format = request.GetParameter("format")?.Value;
            var response = new ResponseOK();
            lock (log)
            {
                switch (format)
                {
                    case "xml":
                        using (var stream = new MemoryStream())
                        {
                            new XmlSerializer(typeof(MeasurementLog)).Serialize(stream, log);
                            response.Content = stream.ToArray();
                        }
                        break;
                    case "pdf":
                        response.Content = Report(request, log).ToArray();
                        break;
                    default:
                        return new ResponseBadRequest();
                }
            }
            response.Header.ContentType = "application/" + format;
            response.Header.ContentDisposition = $"attachment; filename=\"ViLa-{id}.{format}\"";
            return response;
        }

        /// <summary>
        /// Creates a report containing the session summary, description and sampled readings.
        /// </summary>
        /// <param name="request">The request providing the report language.</param>
        /// <param name="log">The completed measurement to report.</param>
        /// <returns>The printable measurement document.</returns>
        private static PdfDocument Report(IRequest request, MeasurementLog log)
        {
            var culture = request.Culture;
            var title = I18N.Translate(request, "vila:vila.details.report");
            var doc = new PdfDocument { Title = title, Author = "ViLa", Header = "ViLa", Language = culture?.Name };
            doc.Add(new PdfBlockElementHeading(1, title), new PdfBlockElementParagraph(log.ID));
            var vehicle = ViewModel.Instance.Vehicles.GetVehicle(log.VehicleId)?.Name ?? log.VehicleName
                ?? I18N.Translate(request, "vila:vila.vehicles.badge.unassigned");
            foreach (var (key, value) in new[]
            {
                ("vehicles.vehicle", vehicle), ("history.from", log.From.ToString("G", culture)),
                ("history.till", log.Till.ToString("G", culture)), ("history.power", log.FinalPower.ToString("N2", culture) + " kWh"),
                ("history.cost", BillingPeriod.FormatCosts([log], culture, ViewModel.Instance.Settings.Currency)),
                ("history.tag", string.Join(", ", BillingPeriod.GetTags(log)))
            })
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    doc.Add(new PdfBlockElementParagraph(I18N.Translate(request, "vila:vila." + key) + ": " + value));
                }
            }
            if (!string.IsNullOrWhiteSpace(log.Description))
            {
                doc.Add(new PdfBlockElementHeading(2, I18N.Translate(request, "vila:vila.description.title")));
                // ConvertToPdf recognizes stored editor state as well as plain or HTML descriptions
                doc.Add(EditorContent.ConvertToPdf(log.Description).Elements);
            }
            if (log.Measurements.Count > 0)
            {
                doc.Add(new PdfBlockElementHeading(2, I18N.Translate(request, "vila:vila.details.samples")));
                var table = new PdfBlockElementTable { Striped = true };
                table.Add(new PdfBlockElementTableRow(new[] { "vila.log.time", "vila.details.pulses", "vila.details.power" }
                    .Select(key => new PdfBlockElementTableCell(I18N.Translate(request, "vila:" + key)))) { Header = true });
                foreach (var item in log.Measurements)
                {
                    table.Add(new PdfBlockElementTableRow([
                        new PdfBlockElementTableCell(item.MeasurementTimePoint.ToString("G", culture)),
                        new PdfBlockElementTableCell(item.Impulse.ToString(culture)),
                        new PdfBlockElementTableCell((item.Power * 60).ToString("N2", culture))]));
                }
                doc.Add(table);
            }
            return doc;
        }
    }
}
