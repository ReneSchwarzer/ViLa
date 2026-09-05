using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="ViewModel"/> class.
/// </summary>
public class ViewModelTest
{
    /// <summary>
    /// Tests that the singleton instance is not null.
    /// </summary>
    [Fact]
    public void Instance_IsNotNull()
    {
        // Assert
        Assert.NotNull(ViewModel.Instance);
    }

    /// <summary>
    /// Tests that Now returns a formatted timestamp string with line break.
    /// </summary>
    [Fact]
    public void Now_ReturnsValidFormattedString()
    {
        // Act
        var nowString = ViewModel.Now;

        // Assert
        Assert.NotNull(nowString);
        Assert.Contains("<br>", nowString);
    }

    /// <summary>
    /// Tests that default gray color is returned for null or whitespace tags.
    /// </summary>
    [Theory]
    [InlineData(null, "#dddddd")]
    [InlineData("", "#dddddd")]
    [InlineData("   ", "#dddddd")]
    public void GetColor_WithNullOrWhitespace_ReturnsDefaultGray(string? tag, string expectedColor)
    {
        // Act
        var color = ViewModel.Instance.GetColor(tag!);

        // Assert
        Assert.Equal(expectedColor, color);
    }

    /// <summary>
    /// Tests deterministic hex color generation for valid tags.
    /// </summary>
    [Theory]
    [InlineData("Work")]
    [InlineData("Home")]
    [InlineData("Tesla")]
    [InlineData("SpecialTag123")]
    public void GetColor_WithValidTag_ReturnsValidHexColor(string tag)
    {
        // Act
        var color1 = ViewModel.Instance.GetColor(tag);
        var color2 = ViewModel.Instance.GetColor(tag);

        // Assert
        Assert.True(Regex.IsMatch(color1, @"^#[0-9A-Fa-f]{6}$"), $"Expected hex color, got {color1}");
        Assert.Equal(color1, color2);
    }

    /// <summary>
    /// Tests adding log entries to the logging list.
    /// </summary>
    [Fact]
    public void Log_AddsLogItemToLoggingList()
    {
        // Arrange
        var initialCount = ViewModel.Instance.Logging.Count;
        var logItem = new LogItem(LogItem.LogLevel.Info, "UnitTest Log Message");

        // Act
        ViewModel.Instance.Log(logItem);

        // Assert
        Assert.True(ViewModel.Instance.Logging.Count > initialCount);
        Assert.Contains(ViewModel.Instance.Logging, x => x.Massage == "UnitTest Log Message");
    }

    /// <summary>
    /// Tests retrieving the current power metric.
    /// </summary>
    [Fact]
    public void CurrentPower_WithNoMeasurements_ReturnsZeroOrValidFloat()
    {
        // Act
        var power = ViewModel.Instance.CurrentPower;

        // Assert
        Assert.False(float.IsNaN(power));
        Assert.True(power >= 0f);
    }
}
