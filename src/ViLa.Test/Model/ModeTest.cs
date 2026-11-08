using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="Mode"/> enum and <see cref="ModeExtensions"/>.
/// </summary>
public class ModeTest
{
    /// <summary>
    /// Tests converting operating modes to localized resource string keys.
    /// </summary>
    [Theory]
    [InlineData(Mode.ManuallyControlled, "vila:vila.setting.mode.manuallycontrolled")]
    [InlineData(Mode.AutomaticControlled, "vila:vila.setting.mode.automaticcontrolled")]
    [InlineData(Mode.TimeControlled, "vila:vila.setting.mode.timecontrolled")]
    [InlineData((Mode)999, "vila:vila.setting.mode.manuallycontrolled")]
    public void ToText_ReturnsExpectedResourceKey(Mode mode, string expectedKey)
    {
        // Act
        var result = mode.ToText();

        // Assert
        Assert.Equal(expectedKey, result);
    }
}
