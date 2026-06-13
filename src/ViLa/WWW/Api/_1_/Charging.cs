using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_
{
    /// <summary>
    /// REST api endpoint returning the live charging metrics as a JSON payload.
    /// </summary>
    [Cache]
    [Application<Application>]
    public sealed class Charging : IRestApi
    {
        public Charging()
        {
        }

        public IResponse Process(IRequest request)
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

            var json = System.Text.Json.JsonSerializer.Serialize(api);
            var response = new ResponseOK()
            {
                Content = json
            };
            response.Header.ContentType = "application/json; charset=utf-8";
            return response;
        }
    }
}
