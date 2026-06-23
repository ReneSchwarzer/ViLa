using System;
using System.Xml.Serialization;

namespace ViLa.Model
{
    public class CommentItem
    {
        /// <summary>
        /// The GUID.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// The comment.
        /// </summary>
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// The creation timestamp.
        /// </summary>
        [XmlAttribute("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// The last-modified timestamp.
        /// </summary>
        [XmlAttribute("updated")]
        public DateTime Updated { get; set; }


    }
}
