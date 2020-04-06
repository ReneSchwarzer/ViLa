using System;
using System.Collections.Generic;
using System.Text;

namespace MtWb.Model
{
    public class API
    {
        /// <summary>
        /// Liefert oder setzt den Mandanten
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Liefert oder setzt den Anfang des Messzeitpunkt
        /// </summary>
        public string MeasurementTime { get; set; }

        /// <summary>
        /// Liefert oder setzt die Anzahl der gemessenen Gesammtimpulse
        /// </summary>
        public string Impulse { get; set; }

        /// <summary>
        /// Liefert oder setzt die gemessene Gesammtleistung in kWh
        /// </summary>
        public string Power { get; set; }

        /// <summary>
        /// Liefert oder setzt die Kosten in €
        /// </summary>
        public string Cost { get; set; }

        /// <summary>
        /// Liefert oder setzt die aktuelle Zeit
        /// </summary>
        public string Now { get; set; }
    }
}
