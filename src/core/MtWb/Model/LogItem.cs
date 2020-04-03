using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MtWb.Model
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
        /// <param name="level">Das Loglevel</param>
        /// <param name="massage">Die Lognachricht</param>
        public LogItem(LogLevel level, string massage)
        {
            Level = level;
            Massage = massage;

            Time = DateTime.Now;

            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();

            var callingFrame = stackFrames[1];
            var method = callingFrame.GetMethod();

            Instance = string.Format("{0}.{1}", method.DeclaringType.Name, method.Name);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="level">Das Loglevel</param>
        /// <param name="massage">Die Lognachricht</param>
        /// <param name="instance">Die Instanz</param>
        public LogItem(LogLevel level, string massage, string instance)
        {
            Level = level;
            Massage = massage;

            Time = DateTime.Now;

            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();

            var callingFrame = stackFrames[1];
            var method = callingFrame.GetMethod();

            Instance = instance;
        }
    }
}
