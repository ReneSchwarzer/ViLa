using System;
using WebExpress.WebIndex;

namespace ViLa.Model
{
    /// <summary>
    /// Indexable wrapper around a measurement log for use with the WQL prompt.
    /// </summary>
    public class IndexMeasurementLog : IIndexItem
    {
        /// <summary>
        /// Gets or sets the wrapped measurement log.
        /// </summary>
        public MeasurementLog Log { get; set; }

        /// <summary>
        /// The id of the item.
        /// </summary>
        public Guid Id => string.IsNullOrEmpty(Log?.ID) ? Guid.Empty : Guid.Parse(Log.ID);

        /// <summary>
        /// Exposes the From date for WQL filtering.
        /// </summary>
        public DateTime From => Log?.From ?? DateTime.MinValue;

        /// <summary>
        /// Exposes the Till date for WQL filtering.
        /// </summary>
        public DateTime Till => Log?.Till ?? DateTime.MinValue;

        /// <summary>
        /// Exposes the final power in kWh for WQL filtering. Falls back to
        /// the raw Power field if FinalPower is unavailable.
        /// </summary>
        public float Power => float.IsNaN(Log?.FinalPower ?? float.NaN) ? Log?.Power ?? 0f : Log.FinalPower;

        /// <summary>
        /// Exposes the final cost for WQL filtering. Falls back to the raw
        /// Cost field if FinalCost is unavailable.
        /// </summary>
        public float Cost => float.IsNaN(Log?.FinalCost ?? float.NaN) ? Log?.Cost ?? 0f : Log.FinalCost;

        /// <summary>
        /// Exposes the tag for WQL filtering.
        /// </summary>
        public string Tag => Log?.Tag ?? string.Empty;
    }
}
