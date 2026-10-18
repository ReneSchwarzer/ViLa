using System;
using System.Collections.Generic;
using System.Linq;

namespace ViLa.Model
{
    /// <summary>
    /// Matches completed charging sessions to distinct maximum power profiles.
    /// </summary>
    public static class VehicleRecognition
    {
        /// <summary>
        /// Finds a unique active vehicle with at least three samples near its expected maximum.
        /// </summary>
        /// <param name="log">The completed session containing minute energy samples.</param>
        /// <param name="vehicles">The vehicle profiles, including profiles with automatic assignment disabled.</param>
        /// <returns>The matching vehicle, or null when evidence is missing or ambiguous.</returns>
        public static Vehicle Find(MeasurementLog log, IEnumerable<Vehicle> vehicles)
        {
            var power = log.Measurements.Select(x => (double)x.Power * 60).ToArray();
            if (power.Length < 3 || power.Any(x => !double.IsFinite(x) || x < 0))
            {
                return null;
            }
            var peak = power.Max();
            if (peak <= 0)
            {
                return null;
            }
            var matches = vehicles.Where(x => x.Active && Math.Abs(peak - x.MaxPowerKw) <= Tolerance(x.MaxPowerKw)).ToArray();
            if (matches.Length != 1 || !matches[0].AutomaticAssignment)
            {
                return null;
            }
            var vehicle = matches[0];
            var consecutive = 0;
            for (var index = 0; index < power.Length; index++)
            {
                var contiguous = index == 0 || Math.Abs((log.Measurements[index].MeasurementTimePoint
                    - log.Measurements[index - 1].MeasurementTimePoint).TotalSeconds - 60) <= 5;
                consecutive = Math.Abs(power[index] - vehicle.MaxPowerKw) <= Tolerance(vehicle.MaxPowerKw)
                    ? (contiguous ? consecutive : 0) + 1 : 0;
                if (consecutive >= 3)
                {
                    return vehicle;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates an automatic assignment while preserving every manual decision.
        /// </summary>
        /// <param name="log">The completed session to update.</param>
        /// <param name="vehicles">The available vehicle profiles.</param>
        public static void Apply(MeasurementLog log, IEnumerable<Vehicle> vehicles)
        {
            if (log.VehicleAssignment == VehicleAssignment.Manual)
            {
                return;
            }
            var vehicle = Find(log, vehicles);
            log.VehicleId = vehicle?.Id;
            log.VehicleName = vehicle?.Name;
            log.VehicleAssignment = vehicle == null ? VehicleAssignment.Unassigned : VehicleAssignment.Automatic;
        }

        /// <summary>
        /// Returns the allowed deviation for minute averages and meter resolution.
        /// </summary>
        /// <param name="power">The configured maximum power in kilowatts.</param>
        /// <returns>Five percent of the configured power, with a minimum of 0.3 kW.</returns>
        private static double Tolerance(double power)
        {
            return Math.Max(0.3, power * 0.05);
        }
    }
}
