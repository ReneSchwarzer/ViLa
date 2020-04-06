using MtWb.Model;
using System;
using System.Text.Json;
using WebExpress.Pages;

namespace MtWb.Pages
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
                MeasurementTime = ViewModel.Instance.ActiveCharging ? converter.Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null).ToString() : string.Empty,
                Impulse = string.Format("{0}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Impulse : 0),
                Power = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Power : 0f),
                Cost = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Cost : 0f),
                Now = ViewModel.Instance.Now
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            Content = JsonSerializer.Serialize(api, options);
        }
    }
}
