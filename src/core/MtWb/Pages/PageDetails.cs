using MtWb.Model;
using System.Linq;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
{
    public class PageDetails : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageDetails()
            : base("Details")
        {
            HeaderScriptLinks.Add("/Assets/js/Chart.min.js");
            HeaderScriptLinks.Add("/Assets/js/details.js");
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

            var id = GetParam("id");
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLogs(id);
            var chartLabels = measurementLog?.Measurements.Select(x => ((int)(x.MeasurementTimePoint - measurementLog.From).TotalMinutes).ToString()).ToArray();
            var chartData = measurementLog?.Measurements.Select(x => x.Power.ToString()).ToArray();

            Main.Content.Add(new ControlText(this)
            {
                Text = string.Format("{0:F2} kWh", measurementLog?.Power) + " / " + string.Format("{0:F2} €", measurementLog?.Cost),
                Format = TypesTextFormat.H1,
                Color = TypesTextColor.Primary
            });

            Main.Content.Add(new ControlText(this)
            {
                Text = measurementLog?.From.ToString("HH:mm:ss") + " - " + measurementLog?.Till.ToString("HH:mm:ss") + " Uhr",
                Format = TypesTextFormat.Paragraph,
                Color = TypesTextColor.Dark
            });

            Main.Content.Add(new ControlCanvas(this, "canvas")
            {
                Class = "mr-4"
            });

            var builder = new StringBuilder();
            builder.Append("config.data.labels = [");
            builder.Append(string.Join(",", chartLabels ?? new string[1]));
            builder.AppendLine("];");
            builder.AppendLine("config.data.datasets.forEach(function(dataset){");
            builder.Append("dataset.data = [");
            builder.Append(string.Join(",", chartData ?? new string[1]));
            builder.AppendLine("];");
            builder.Append("});");

            AddScript("chart", builder.ToString());

            Main.Content.Add(new ControlPanel
            (
                this,
                new ControlButtonLink(this)
                {
                    Text = "Datei herunterladen",
                    Layout = TypesLayoutButton.Primary,
                    Icon = Icon.Download,
                    Color = TypesTextColor.Light,
                    Url = GetPath(0, "measurements/" + id + ".xml")
                },
                new ControlButtonLink(this)
                {
                    Text = "Datei archivieren",
                    Layout = TypesLayoutButton.Primary,
                    Icon = Icon.Clock,
                    Color = TypesTextColor.Light,
                    Modal = new ControlModal
                    (
                        this,
                        "archive",
                        "Archivieren",
                        new ControlText(this)
                        {
                            Text = "Möchten Sie die Datei wirklich archivieren?"
                        },
                        new ControlButton(this)
                        {
                            Text = "Archivieren",
                            Icon = Icon.Clock,
                            Class = "m-1",
                            Layout = TypesLayoutButton.Success,
                            OnClick = "window.location.href = '" + GetPath(2, "archive").ToString() + " '"
                        }
                    )
                },
                new ControlButtonLink(this)
                {
                    Text = "Datei löschen",
                    Layout = TypesLayoutButton.Danger,
                    Icon = Icon.TrashAlt,
                    HorizontalAlignment = TypesHorizontalAlignment.Right,
                    Color = TypesTextColor.Light,
                    Modal = new ControlModal
                    (
                        this,
                        "del",
                        "Löschen",
                        new ControlText(this)
                        {
                            Text = "Möchten Sie die Datei wirklich löschen?"
                        },
                        new ControlButton(this)
                        {
                            Text = "Löschen",
                            Icon = Icon.TrashAlt,
                            Class = "m-1",
                            Layout = TypesLayoutButton.Danger,
                            OnClick = "window.location.href = '" + GetPath(2, "del").ToString() + " '"
                        }
                    )
            })
            {
                Class = "mr-4"
            });
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
