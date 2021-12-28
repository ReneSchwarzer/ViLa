namespace ViLa.Model
{
    /// <summary>
    /// Die unterstützten Betriebsmodi
    /// </summary>
    public enum Mode
    {
        ManuallyControlled,
        AutomaticControlled,
        TimeControlled
    }

    /// <summary>
    /// Erweiterung des Mode
    /// </summary>
    public static class ModeExtensions
    {
        /// <summary>
        /// Umwandlung in eine String-Klasse
        /// </summary>
        /// <param name="mode">Der Betriebsmodus</param>
        /// <returns>Der String</returns>
        public static string ToText(this Mode mode)
        {
            return mode switch
            {
                Mode.AutomaticControlled => "vila:vila.setting.mode.automaticcontrolled",
                Mode.TimeControlled => "vila:vila.setting.mode.timecontrolled",
                _ => "vila:vila.setting.mode.manuallycontrolled",
            };
        }
    }
}
