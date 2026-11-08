using System;
using System.Globalization;
using System.Linq;

namespace ViLa.Model
{
    /// <summary>
    /// Describes a vehicle and its expected maximum AC charging power.
    /// </summary>
    public sealed record Vehicle : WebExpress.WebIndex.IIndexItem
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
        /// Gets the identifier used by vehicle queries.
        /// </summary>
        Guid WebExpress.WebIndex.IIndexItem.Id => Guid.Parse(Id);

        private static readonly string[] Palette = ["#2879b9", "#a44985", "#25816f", "#b86420", "#6957ab", "#8b7532"];
        private string _color;

        /// <summary>
        /// Gets the configured color or a stable palette color for older profiles.
        /// </summary>
        public string Color
        {
            get => IsValidColor(_color) ? _color : DefaultColor(Id);
            init => _color = value;
        }

        /// <summary>
        /// Selects a stable palette color from the vehicle identifier.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <returns>The default hexadecimal color.</returns>
        public static string DefaultColor(string id)
        {
            return Palette[Guid.TryParse(id, out var guid) ? guid.ToByteArray()[0] % Palette.Length : 0];
        }

        /// <summary>
        /// Checks whether a color consists of six hexadecimal RGB digits.
        /// </summary>
        /// <param name="color">The color to check.</param>
        /// <returns>Whether the color can be used as a vehicle color.</returns>
        public static bool IsValidColor(string color)
        {
            return color?.Length == 7 && color[0] == '#' && color.Skip(1).All(Uri.IsHexDigit);
        }

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
