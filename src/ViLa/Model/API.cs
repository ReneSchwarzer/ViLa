namespace ViLa.Model
{
    public class API
    {
        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets whether the charging process is active.
        /// </summary>
        public bool ActiveCharging { get; set; }

        /// <summary>
        /// Gets or sets the start of the measurement time.
        /// </summary>
        public string MeasurementTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of measured impulses.
        /// </summary>
        public string Impulse { get; set; }

        /// <summary>
        /// Gets or sets the total measured power in kWh.
        /// </summary>
        public string Power { get; set; }

        /// <summary>
        /// Gets or sets the cost in €.
        /// </summary>
        public string Cost { get; set; }

        /// <summary>
        /// Gets or sets the currently measured total power in kWh.
        /// </summary>
        public string CurrentPower { get; set; }

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        public string Now { get; set; }

        /// <summary>
        /// Gets or sets the chart data labels.
        /// </summary>
        public string[] ChartLabels { get; set; }

        /// <summary>
        /// Gets or sets the chart data values.
        /// </summary>
        public string[] ChartData { get; set; }
    }
}
