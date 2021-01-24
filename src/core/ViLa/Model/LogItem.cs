using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ViLa.Model
{
    public class LogItem
    {
        /// <summary>
        /// Die Art des Logeintrages
        /// </summary>
        public enum LogLevel { Info, Debug, Warning, Error, Exception }

        /// <summary>
        /// Liefert oder setzt das Loglevel
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Liefert ider setzt die Lognachricht
        /// </summary>
        public string Massage { get; set; }

        /// <summary>
        /// Liefert oder setzt die Logzeit
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Liefert oder setzt die auslösende Instanz
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LogItem()
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="level">Das Loglevel</param>
        /// <param name="massage">Die Lognachricht</param>
        public LogItem(LogLevel level, string massage, [CallerMemberName] string instance = null)
        {
            Level = level;
            Massage = massage;
            Time = DateTime.Now;
            Instance = instance;
        }
    }
}
