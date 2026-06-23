using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ViLa.Model
{
    /// <summary>
    /// Measurement log.
    /// </summary>
    [XmlRoot(ElementName = "measurementlog", IsNullable = false)]
    public class MeasurementLog
    {
        /// <summary>
        /// The ID.
        /// </summary>
        [XmlAttribute("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [XmlAttribute("mandant")]
        public string Client { get; set; }

        /// <summary>
        /// Gets the start of the measurement time.
        /// </summary>
        [XmlIgnore]
        public DateTime From => Measurements.FirstOrDefault().MeasurementTimePoint;

        /// <summary>
        /// Gets the end of the measurement time.
        /// </summary>
        [XmlIgnore]
        public DateTime Till => Measurements.LastOrDefault().MeasurementTimePoint;

        /// <summary>
        /// Gets the total number of measured impulses.
        /// </summary>
        [XmlIgnore]
        public long Impulse => Measurements.Sum(x => x.Impulse);

        /// <summary>
        /// Gets the total measured power in kWh.
        /// </summary>
        [XmlIgnore]
        public float Power => (float)Impulse / ViewModel.Instance.Settings.ImpulsePerkWh;

        /// <summary>
        /// Gets the cost in the configured currency.
        /// </summary>
        [XmlIgnore]
        public float Cost => Power * ViewModel.Instance.Settings.ElectricityPricePerkWh;

        /// <summary>
        /// Gets the power measured in the last minute, in kWh.
        /// </summary>
        public float CurrentPower => Measurements.Count < 2 ?
            Measurements.FirstOrDefault().Power :
            Measurements.TakeLast(2).Take(1).FirstOrDefault().Power;

        /// <summary>
        /// Gets the current measurement value.
        /// </summary>
        [XmlIgnore]
        public MeasurementItem CurrentMeasurement => Measurements.LastOrDefault();

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        [XmlElement("measurements")]
        public List<MeasurementItem> Measurements { get; set; } = new List<MeasurementItem>();

        /// <summary>
        /// Gets or sets the final consumption in kWh.
        /// </summary>
        [XmlAttribute("power")]
        public float FinalPower { get; set; }

        /// <summary>
        /// Gets or sets the final cost in the configured currency.
        /// </summary>
        [XmlAttribute("cost")]
        public float FinalCost { get; set; }

        /// <summary>
        /// Gets or sets the final start time.
        /// </summary>
        [XmlAttribute("from")]
        public DateTime FinalFrom { get; set; }

        /// <summary>
        /// Gets or sets the final end time.
        /// </summary>
        [XmlAttribute("till")]
        public DateTime FinalTill { get; set; }

        /// <summary>
        /// Gets or sets the electricity price per kWh.
        /// </summary>
        [XmlAttribute("pricePerkWh")]
        public float ElectricityPricePerkWh { get; set; }

        /// <summary>
        /// Gets or sets the number of impulses per kWh.
        /// </summary>
        [XmlAttribute("impulsePerkWh")]
        public int ImpulsePerkWh { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        [XmlAttribute("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the log items.
        /// </summary>
        [XmlElement("comment")]
        public List<CommentItem> Comments { get; set; } = new List<CommentItem>();

        /// <summary>
        /// Gets or sets a label.
        /// </summary>
        [XmlElement("label")]
        public string Tag { get; set; }

        /// <summary>
        /// Resets the values.
        /// </summary>
        public void Reset()
        {
            Measurements.Clear();
        }
    }
}
