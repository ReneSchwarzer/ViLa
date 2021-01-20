using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ViLa.Model
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
        public long Impulse => Measurements.Sum(x => x.Impulse);

        /// <summary>
        /// Liefert oder setzt die gemessene Gesammtleistung in kWh
        /// </summary>
        [XmlIgnore]
        public float Power => (float)Impulse / ViewModel.Instance.Settings.ImpulsePerkWh;

        /// <summary>
        /// Liefert oder setzt die Kosten in der angegebenen Währung
        /// </summary>
        [XmlIgnore]
        public float Cost => Power * ViewModel.Instance.Settings.ElectricityPricePerkWh;

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

        /// <summary>
        /// Liefert oder setzt den finalen Verbrauch in kWh
        /// </summary>
        [XmlAttribute("power")]
        public float FinalPower { get; set; }

        /// <summary>
        /// Liefert oder setzt die finalen Kosten in der angegebenen Währung
        /// </summary>
        [XmlAttribute("cost")]
        public float FinalCost { get; set; }

        /// <summary>
        /// Setzt die Werte zurück
        /// </summary>
        public void Reset()
        {
            Measurements.Clear();
        }
    }
}
