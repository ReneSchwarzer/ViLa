using System;
using System.Globalization;
using System.Linq;

namespace ViLa.Model
{
    /// <summary>
    /// Builds the shared representation used by the live API and initial dashboard.
    /// </summary>
    public static class ChargingSnapshot
    {
        /// <summary>
        /// Captures charging values using one session reference and measurement array.
        /// </summary>
        /// <param name="model">The model supplying the current session and settings.</param>
        /// <param name="culture">The culture used for displayed numbers.</param>
        /// <returns>The formatted API payload with invariant chart values.</returns>
        public static API Create(ViewModel model, CultureInfo culture)
        {
            var active = model.ActiveCharging;
            var log = model.CurrentMeasurementLog;
            var measurements = log.Measurements.ToArray();
            var energy = model.Settings.ImpulsePerkWh > 0
                ? measurements.Sum(x => (long)x.Impulse) / (double)model.Settings.ImpulsePerkWh : 0;
            var current = measurements.Length == 0 ? 0 : measurements[Math.Max(0, measurements.Length - 2)].Power * 60;
            return new API()
            {
                ActiveCharging = active,
                MeasurementTime = active && log.From != default
                    ? new TimeSpanConverter().Convert(DateTime.Now - log.From, typeof(string), null, culture.Name).ToString() : "-",
                Impulse = (active ? measurements.Sum(x => (long)x.Impulse) : 0).ToString(culture),
                Power = (active ? energy : 0).ToString("F2", culture),
                Cost = (active ? energy * model.Settings.ElectricityPricePerkWh : 0).ToString("F2", culture),
                CurrentPower = current.ToString("F2", culture),
                Now = ViewModel.Now,
                ChartLabels = measurements.Select((_, index) => (active ? index : index - measurements.Length + 1).ToString(CultureInfo.InvariantCulture)).ToArray(),
                ChartData = measurements.Select(x => (x.Power * 60).ToString(CultureInfo.InvariantCulture)).ToArray()
            };
        }
    }
}
