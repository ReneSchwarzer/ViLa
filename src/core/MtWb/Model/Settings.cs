using System.Xml.Serialization;

namespace MtWb.Model
{
    [XmlRoot(ElementName = "settings", IsNullable = false)]
    public class Settings
    {
        /// <summary>
        /// Liefert oder setzt den Debug-Modus
        /// </summary>
        [XmlElement(ElementName = "debug", DataType = "boolean")]
        public bool DebugMode { get; set; }

        /// <summary>
        /// Liefert oder setzt die Anzahl der Impulse pro kWh
        /// </summary>
        [XmlElement(ElementName = "impulseperkwh")]
        public int ImpulsePerkWh { get; set; }
    }
}
