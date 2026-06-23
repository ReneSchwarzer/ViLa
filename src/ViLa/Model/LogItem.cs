using System;
using System.Runtime.CompilerServices;

namespace ViLa.Model
{
    public class LogItem
    {
        /// <summary>
        /// The log-entry type.
        /// </summary>
        public enum LogLevel { Info, Debug, Warning, Error, Exception }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string Massage { get; set; }

        /// <summary>
        /// Gets or sets the log time.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the triggering instance.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogItem()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="massage">The log message.</param>
        public LogItem(LogLevel level, string massage, [CallerMemberName] string instance = null)
        {
            Level = level;
            Massage = massage;
            Time = DateTime.Now;
            Instance = instance;
        }
    }
}
