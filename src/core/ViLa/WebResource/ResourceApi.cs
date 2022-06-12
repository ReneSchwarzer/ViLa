using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.Message;
using WebExpress.WebAttribute;
using WebExpress.WebResource;

namespace ViLa.WebResource
{
    [Id("Api")]
    [Segment("api")]
    [Path("/")]
    [Module("ViLa")]
    [Cache]
    public sealed class ResourceApi : WebExpress.WebResource.ResourceRest
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
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        /// <param name="request">Die Anfrage</param>
        /// <returns>Ein Objekt welches mittels JsonSerializer serialisiert werden kann.</returns>
        public override object GetData(Request request)
        {
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
                MeasurementTime = ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? converter.Convert(DateTime.Now - ViewModel.Instance.CurrentMeasurementLog?.From, typeof(string), null, null).ToString() : "-",
                Impulse = string.Format("{0}", ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? ViewModel.Instance.CurrentMeasurementLog.Impulse : 0),
                Power = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? ViewModel.Instance.CurrentMeasurementLog.Power : "-"),
                Cost = string.Format("{0:F2}", ViewModel.Instance.ActiveCharging && ViewModel.Instance.CurrentMeasurementLog.Measurements.Count > 0 ? ViewModel.Instance.CurrentMeasurementLog.Cost : "-"),
                Now = ViewModel.Now,
                CurrentPower = string.Format("{0:F2}", ViewModel.Instance.CurrentPower),
                ChartLabels = ViewModel.Instance.ActiveCharging ?
                    ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => ViewModel.Instance.CurrentMeasurementLog.Measurements.IndexOf(x).ToString()).ToArray() :
                    createArray(ViewModel.Instance.CurrentMeasurementLog.Measurements.Count),
                ChartData = ViewModel.Instance.CurrentMeasurementLog.Measurements.Select(x => (x.Power * 60).ToString(CultureInfo.InvariantCulture)).ToArray()
            };

            return api;
        }
    }
}
