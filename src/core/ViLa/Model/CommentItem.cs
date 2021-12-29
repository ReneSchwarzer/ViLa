using System;
using System.Xml.Serialization;

namespace ViLa.Model
{
    public class CommentItem
    {
        /// <summary>
        /// Die GUID
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Der Kommentar
        /// </summary>
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Der Zeitstempel der Erstellung
        /// </summary>
        [XmlAttribute("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Der Zeitstempel der letzten Änderung
        /// </summary>
        [XmlAttribute("updated")]
        public DateTime Updated { get; set; }


    }
}
