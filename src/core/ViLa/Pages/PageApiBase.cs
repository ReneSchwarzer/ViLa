using System;
using System.Linq;
using System.Text.Json;
using ViLa.Model;
using WebExpress.Pages;

namespace ViLa.Pages
{
    public class PageApiBase : PageApi
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageApiBase()
            : base()
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

            var converter = new TimeSpanConverter();

            var api = new API()
            {
                ActiveCharging = ViewModel.Instance.ActiveCharging,
                MeasurementTime = ViewModel.Instance.ActiveCharging ? converter.Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null).ToString() : string.Empty,
                Impulse = string.Format("{0}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Impulse : 0),
                Power = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Power : 0f),
                Cost = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Cost : 0f),
                Now = ViewModel.Instance.Now,
                ChartLabels = ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => ((int)(x.MeasurementTimePoint - ViewModel.Instance.CurrentMeasurementLog.From).TotalMinutes).ToString()).ToArray() : null,
                ChartData = ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => x.Power.ToString()).ToArray() : null
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            Content = JsonSerializer.Serialize(api, options);
        }
    }
}
