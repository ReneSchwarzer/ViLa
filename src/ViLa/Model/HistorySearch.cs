using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebExpress.WebIndex.Wql;

namespace ViLa.Model
{
    /// <summary>
    /// Selects the measurements shared by the history presentations before paging.
    /// </summary>
    internal static class HistorySearch
    {
        /// <summary>
        /// Applies a WQL expression or searches the visible measurement values.
        /// </summary>
        /// <param name="logs">The ordered measurement source.</param>
        /// <param name="wql">The advanced expression.</param>
        /// <param name="text">The basic search text.</param>
        /// <param name="culture">The culture used for the displayed values.</param>
        /// <returns>The matching measurements in query order.</returns>
        public static IQueryable<IndexMeasurementLog> Apply(IQueryable<IndexMeasurementLog> logs, string wql, string text, CultureInfo culture)
        {
            if (!string.IsNullOrWhiteSpace(wql))
            {
                var statement = new WqlParser<IndexMeasurementLog>().Parse(wql);
                return statement.HasErrors ? Enumerable.Empty<IndexMeasurementLog>().AsQueryable() : statement.ToQuery().Apply(logs);
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return logs;
            }
            var vehicles = VehicleNames();
            return logs.AsEnumerable().Where(item => Matches(item.Log, text, culture, vehicles)).AsQueryable();
        }

        /// <summary>
        /// Gets the current catalog names keyed by vehicle identifier, for use with <see cref="Matches"/>.
        /// </summary>
        /// <returns>The vehicle names by identifier.</returns>
        public static IReadOnlyDictionary<string, string> VehicleNames()
        {
            return ViewModel.Instance.Vehicles.GetAll().ToDictionary(vehicle => vehicle.Id, vehicle => vehicle.Name);
        }

        /// <summary>
        /// Determines whether the visible values of a measurement contain the basic search text.
        /// </summary>
        /// <param name="log">The measurement to test.</param>
        /// <param name="text">The basic search text.</param>
        /// <param name="culture">The culture used for the displayed values.</param>
        /// <param name="vehicles">The catalog names keyed by vehicle identifier.</param>
        /// <returns>Whether the measurement matches.</returns>
        public static bool Matches(MeasurementLog log, string text, CultureInfo culture, IReadOnlyDictionary<string, string> vehicles)
        {
            var name = log.VehicleId != null && vehicles.TryGetValue(log.VehicleId, out var vehicle) ? vehicle : log.VehicleName;
            var values = string.Format(culture, "{0:g} {1:N2} kWh {2:0} min {3} {4}", log.From, log.FinalPower,
                (log.Till - log.From).TotalMinutes, name, log.Tag);
            return culture.CompareInfo.IndexOf(values, text.Trim(), CompareOptions.IgnoreCase) >= 0;
        }
    }
}
