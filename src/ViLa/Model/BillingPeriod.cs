using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ViLa.Model
{
    /// <summary>
    /// Selects completed sessions within an offset billing month.
    /// </summary>
    public static class BillingPeriod
    {
        /// <summary>
        /// Formats stored costs separately for each currency without applying the current tariff.
        /// </summary>
        /// <param name="logs">The completed sessions to summarize.</param>
        /// <param name="culture">The display culture.</param>
        /// <param name="fallbackCurrency">The currency for legacy sessions without a currency.</param>
        /// <returns>The formatted totals separated by slashes.</returns>
        public static string FormatCosts(IEnumerable<MeasurementLog> logs, CultureInfo culture, string fallbackCurrency)
        {
            return string.Join(" / ", logs.GroupBy(x => string.IsNullOrWhiteSpace(x.Currency) ? fallbackCurrency : x.Currency)
                .Select(group => string.Format(culture, "{0:F2} {1}", group.Sum(x => x.FinalCost), group.Key)));
        }

        /// <summary>
        /// Gets sessions ending within the inclusive start and exclusive end of a billing month.
        /// </summary>
        /// <param name="logs">The completed measurement logs.</param>
        /// <param name="month">Any date in the nominal billing month.</param>
        /// <param name="offset">The number of days subtracted from the calendar boundaries.</param>
        /// <returns>The sessions ordered by completion time.</returns>
        public static MeasurementLog[] GetLogs(IEnumerable<MeasurementLog> logs, DateTime month, int offset)
        {
            var start = new DateTime(month.Year, month.Month, 1).AddDays(-offset);
            var end = new DateTime(month.Year, month.Month, 1).AddMonths(1).AddDays(-offset);

            return logs.Where(x => x.Till >= start && x.Till < end).OrderByDescending(x => x.Till).ToArray();
        }

        /// <summary>
        /// Splits a session label into distinct space or semicolon separated tags.
        /// </summary>
        /// <param name="log">The session whose tags are requested.</param>
        /// <returns>The distinct tags using ordinal comparison.</returns>
        public static IEnumerable<string> GetTags(MeasurementLog log)
        {
            return (log.Tag ?? string.Empty)
                .Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.Ordinal);
        }
    }
}
