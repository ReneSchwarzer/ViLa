using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ViLa.Model;
using WebExpress.UI.Controls;

namespace ViLa.Pages
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

            var id = string.Format("{0}-{1}-{2}-{3}-{4}", GetParam("id1"), GetParam("id2"), GetParam("id3"), GetParam("id4"), GetParam("id5"));
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLogs(id);
            var chartLabels = measurementLog?.Measurements.Select(x => ((int)(x.MeasurementTimePoint - measurementLog.From).TotalMinutes).ToString()).ToArray();
            var chartData = measurementLog?.Measurements.Select(x => x.Power.ToString()).ToArray();

            Main.Content.Add(new ControlText()
            {
                Text = id + ".mxl",
                TextColor = new PropertyColorText(TypeColorText.Muted)
            });

            Main.Content.Add(new ControlText()
            {
                Text = string.Format("{0:F2} kWh", measurementLog?.Power) + " / " + string.Format("{0:F2} €", measurementLog?.Cost),
                Format = TypeFormatText.H1,
                TextColor = new PropertyColorText(TypeColorText.Primary)
            });

            Main.Content.Add(new ControlText()
            {
                Text = measurementLog?.From.ToString("HH:mm:ss") + " - " + measurementLog?.Till.ToString("HH:mm:ss") + " Uhr",
                Format = TypeFormatText.Paragraph,
                TextColor = new PropertyColorText(TypeColorText.Dark)
            });

            Main.Content.Add(new ControlCanvas("canvas")
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None)
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
                new ControlButtonLink()
                {
                    Text = "Datei herunterladen",
                    Color = new PropertyColorButton(TypeColorButton.Primary),
                    Icon = new PropertyIcon(TypeIcon.Download),
                    TextColor = new PropertyColorText(TypeColorText.Light),
                    Uri = Uri.Root.Append("measurements/" + id + ".xml")
                },
                new ControlButtonLink()
                {
                    Text = "Datei archivieren",
                    Color = new PropertyColorButton(TypeColorButton.Primary),
                    Icon = new PropertyIcon(TypeIcon.Clock),
                    TextColor = new PropertyColorText(TypeColorText.Light),
                    Modal = new ControlModal
                    (
                        "archive",
                        "Archivieren",
                        new ControlText()
                        {
                            Text = "Möchten Sie die Datei wirklich archivieren?"
                        },
                        new ControlButton()
                        {
                            Text = "Archivieren",
                            Icon = new PropertyIcon(TypeIcon.Clock),
                            Classes = new List<string>(new[] { "m-1" }),
                            Color = new PropertyColorButton(TypeColorButton.Success),
                            OnClick = "window.location.href = '" + Uri.Append("archive").ToString() + " '"
                        }
                    )
                },
                new ControlButtonLink()
                {
                    Text = "Datei löschen",
                    Color = new PropertyColorButton(TypeColorButton.Danger),
                    Icon = new PropertyIcon(TypeIcon.TrashAlt),
                    HorizontalAlignment = TypeHorizontalAlignment.Right,
                    TextColor = new PropertyColorText(TypeColorText.Light),
                    Modal = new ControlModal
                    (
                        "del",
                        "Löschen",
                        new ControlText()
                        {
                            Text = "Möchten Sie die Datei wirklich löschen?"
                        },
                        new ControlButton()
                        {
                            Text = "Löschen",
                            Icon = new PropertyIcon(TypeIcon.TrashAlt),
                            Margin = new PropertySpacingMargin(PropertySpacing.Space.One),
                            Color = new PropertyColorButton(TypeColorButton.Danger),
                            OnClick = "window.location.href = '" + Uri.Append("del").ToString() + " '"
                        }
                    )
                })
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.None, PropertySpacing.Space.None)
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
