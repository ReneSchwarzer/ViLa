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
        [XmlIgnore]
        public DateTime From => Measurements.FirstOrDefault().MeasurementTimePoint;

        /// <summary>
        /// Liefert oder setzt das Ende des Messzeitpunkt
        /// </summary>
        [XmlIgnore]
        public DateTime Till => Measurements.LastOrDefault().MeasurementTimePoint;

        /// <summary>
        /// Liefert oder setzt die Anzahl der gemessenen Gesammtimpulse
        /// </summary>
        [XmlIgnore]
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
        /// Ermittelt die aktuell ermittelte Leistung der letzen Minute in kWh
        /// </summary>
        public float CurrentPower => Measurements.Count < 2 ?
            Measurements.FirstOrDefault().Power :
            Measurements.TakeLast(2).Take(1).FirstOrDefault().Power;

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
        /// Liefert oder setzt den finalen Startzeitpunkt
        /// </summary>
        [XmlAttribute("from")]
        public DateTime FinalFrom { get; set; }

        /// <summary>
        /// Liefert oder setzt die finalen Endzeitpunkt
        /// </summary>
        [XmlAttribute("till")]
        public DateTime FinalTill { get; set; }

        /// <summary>
        /// Liefert oder setzt den Strompreis pro kWh
        /// </summary>
        [XmlAttribute("pricePerkWh")]
        public float ElectricityPricePerkWh { get; set; }

        /// <summary>
        /// Liefert oder setzt die Anzahl der Impulse pro kWh
        /// </summary>
        [XmlAttribute("impulsePerkWh")]
        public int ImpulsePerkWh { get; set; }

        /// <summary>
        /// Liefert oder setzt die Währung
        /// </summary>
        [XmlAttribute("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Setzt die Werte zurück
        /// </summary>
        public void Reset()
        {
            Measurements.Clear();
        }
    }
}
