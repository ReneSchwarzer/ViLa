namespace ViLa.Model
{
    /// <summary>
    /// The supported operating modes.
    /// </summary>
    public enum Mode
    {
        ManuallyControlled,
        AutomaticControlled,
        TimeControlled
    }

    /// <summary>
    /// Extensions for the operating mode.
    /// </summary>
    public static class ModeExtensions
    {
        /// <summary>
        /// Converts the mode to its resource string.
        /// </summary>
        /// <param name="mode">The operating mode.</param>
        /// <returns>The string.</returns>
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
