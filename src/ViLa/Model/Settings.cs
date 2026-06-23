using System.Xml.Serialization;

namespace ViLa.Model
{
    [XmlRoot(ElementName = "settings", IsNullable = false)]
    public class Settings
    {
        /// <summary>
        /// Gets or sets the debug mode.
        /// </summary>
        [XmlElement(ElementName = "debug", DataType = "boolean")]
        public bool DebugMode { get; set; }

        /// <summary>
        /// Gets or sets the number of impulses per kWh.
        /// </summary>
        [XmlElement(ElementName = "ImpulsePerkWh")]
        public int ImpulsePerkWh { get; set; }

        /// <summary>
        /// Gets or sets the electricity price per kWh.
        /// </summary>
        [XmlElement(ElementName = "ElectricityPricePerkWh")]
        public float ElectricityPricePerkWh { get; set; }

        /// <summary>
        /// Gets or sets the maximum power consumption in kWh.
        /// </summary>
        [XmlElement(ElementName = "MaxWattage")]
        public int MaxWattage { get; set; }

        /// <summary>
        /// Gets or sets the minimum power consumption in kWh
        /// at which the charging process is aborted.
        /// </summary>
        [XmlElement(ElementName = "MinWattage")]
        public float MinWattage { get; set; }

        /// <summary>
        /// Gets or sets the maximum charging time in hours.
        /// </summary>
        [XmlElement(ElementName = "MaxChargingTime")]
        public int MaxChargingTime { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the billing period offset.
        /// </summary>
        [XmlElement(ElementName = "BillingDayOffset")]
        public int BillingDayOffset { get; set; }

        /// <summary>
        /// Determines how measurements are started.
        /// </summary>
        [XmlElement(ElementName = "Mode")]
        public Mode Mode { get; set; }
    }
}
