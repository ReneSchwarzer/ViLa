using System;
using System.Globalization;

namespace ViLa.Model
{
    /// <summary>
    /// Describes a vehicle and its expected maximum AC charging power.
    /// </summary>
    public sealed record Vehicle
    {
        /// <summary>
        /// Gets the stable vehicle identifier.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Gets the vehicle display name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the expected maximum charging power in kilowatts.
        /// </summary>
        public double MaxPowerKw { get; init; }

        /// <summary>
        /// Gets whether an unambiguous power match may assign this vehicle automatically.
        /// </summary>
        public bool AutomaticAssignment { get; init; } = true;

        /// <summary>
        /// Gets whether the vehicle is available for new assignments.
        /// </summary>
        public bool Active { get; init; } = true;

        /// <summary>
        /// Parses a positive finite power without accepting thousands separators.
        /// </summary>
        /// <param name="text">The submitted power value.</param>
        /// <param name="culture">The culture used by the input form.</param>
        /// <param name="power">The parsed power in kilowatts.</param>
        /// <returns>Whether the value is within the supported range.</returns>
        public static bool TryParsePower(string text, CultureInfo culture, out double power)
        {
            const NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
            return (double.TryParse(text, styles, culture, out power)
                || double.TryParse(text, styles, CultureInfo.InvariantCulture, out power))
                && double.IsFinite(power) && power > 0 && power <= 1000;
        }
    }
}
