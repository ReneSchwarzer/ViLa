using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ViLa.Model
{
    /// <summary>
    /// Messwert
    /// </summary>
    public class MeasurementItem
    {
        /// <summary>
        /// Liefert oder setzt den Messzeitpunkt
        /// </summary>
        [XmlAttribute("mtp")]
        public DateTime MeasurementTimePoint { get; set; }

        /// <summary>
        /// Liefert oder setzt die gemessenen Impulse
        /// </summary>
        [XmlAttribute("impulse")]
        public int Impulse { get; set; }

        /// <summary>
        /// Liefert oder setzt die gemessene Leistung in kWh
        /// </summary>
        [XmlAttribute("power")]
        public float Power { get; set; }

        /// <summary>
        /// Liefert oder setzt die Logitems
        /// </summary>
        [XmlElement("logitem")]
        public List<LogItem> Logitems { get; set; } = new List<LogItem>();
    }
}
