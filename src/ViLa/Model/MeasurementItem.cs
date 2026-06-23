using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ViLa.Model
{
    /// <summary>
    /// Measured value.
    /// </summary>
    public class MeasurementItem
    {
        /// <summary>
        /// Gets or sets the measurement time point.
        /// </summary>
        [XmlAttribute("mtp")]
        public DateTime MeasurementTimePoint { get; set; }

        /// <summary>
        /// Gets or sets the measured impulses.
        /// </summary>
        [XmlAttribute("impulse")]
        public int Impulse { get; set; }

        /// <summary>
        /// Gets or sets the measured power in kWh.
        /// </summary>
        [XmlAttribute("power")]
        public float Power { get; set; } // => (float)Impulse / Settings.ImpulsePerkWh

        /// <summary>
        /// Gets or sets the log items.
        /// </summary>
        [XmlElement("logitem")]
        public List<LogItem> Logitems { get; set; } = new List<LogItem>();
    }
}
