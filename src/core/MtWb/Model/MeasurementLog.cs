using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MtWb.Model
{
    /// <summary>
    /// Messprotokoll
    /// </summary>
    [XmlRoot(ElementName = "measurementlog", IsNullable = false)]
    public class MeasurementLog
    {
        /// <summary>
        /// Die ID
        /// </summary>
        [XmlAttribute("id")]
        public string ID { get; set; }

        /// <summary>
        /// Liefert oder setzt den Mandanten
        /// </summary>
        [XmlAttribute("mandant")]
        public string Client { get; set; }

        /// <summary>
        /// Liefert oder setzt den Anfang des Messzeitpunkt
        /// </summary>
        [XmlAttribute("from")]
        public DateTime From { get; set; }

        /// <summary>
        /// Liefert oder setzt das Ende des Messzeitpunkt
        /// </summary>
        [XmlAttribute("till")]
        public DateTime Till { get; set; }

        /// <summary>
        /// Liefert oder setzt die Anzahl der gemessenen Gesammtimpulse
        /// </summary>
        [XmlAttribute("counter")]
        public long Impulse { get; set; }

        /// <summary>
        /// Liefert oder setzt die gemessene Gesammtleistung in kWh
        /// </summary>
        [XmlAttribute("power")]
        public float Power { get; set; }

        /// <summary>
        /// Liefert oder setzt die Kosten in €
        /// </summary>
        [XmlAttribute("cost")]
        public float Cost { get; set; }

        /// <summary>
        /// Liefert oder setzt den aktuellen Messwert
        /// </summary>
        [XmlIgnore]
        public MeasurementItem CurrentMeasurement => Measurements.LastOrDefault();

        /// <summary>
        /// Liefert oder setzt die Messwerte
        /// </summary>
        [XmlElement("measurements")]
        public List<MeasurementItem> Measurements { get; set; } = new List<MeasurementItem>();
    }
}
