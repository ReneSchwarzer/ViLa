using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ViLa.Model;
using WebExpress.Attribute;

namespace ViLa.WebResource
{
    [ID("Api")]
    [Segment("api")]
    [Path("/")]
    [Module("ViLa")]
    public sealed class ResourceApi : WebExpress.WebResource.ResourceApi
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResourceApi()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            static string[] createArray(int size)
            {
                var array = new string[size];
                for (var i = size * -1; i < 0; i++)
                {
                    array[size + i] = (i + 1).ToString();
                }

                return array;
            }

            var converter = new TimeSpanConverter();

            var api = new API()
            {
                ActiveCharging = ViewModel.Instance.ActiveCharging,
                MeasurementTime = ViewModel.Instance.ActiveCharging ? converter.Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null).ToString() : "-",
                Impulse = string.Format("{0}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Impulse : 0),
                Power = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Power : "-"),
                Cost = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging ? ViewModel.Instance.CurrentMeasurementLog.Cost : "-"),
                Now = ViewModel.Now,
                CurrentPower = string.Format("{0:F2}", ViewModel.Instance.CurrentPower),
                ChartLabels = ViewModel.Instance.ActiveCharging ? 
                    ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => ViewModel.Instance.CurrentMeasurementLog.Measurements.IndexOf(x).ToString()).ToArray() :
                    createArray(ViewModel.Instance.CurrentMeasurementLog.Measurements.Count),
                ChartData = ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => (x.Power * 60).ToString(CultureInfo.InvariantCulture)).ToArray()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            Content = JsonSerializer.Serialize(api, options);
        }
    }
}
