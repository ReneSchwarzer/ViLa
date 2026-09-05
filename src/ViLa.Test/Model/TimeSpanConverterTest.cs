using System;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="TimeSpanConverter"/> class.
/// </summary>
public class TimeSpanConverterTest
{
    /// <summary>
    /// Tests converting a valid TimeSpan into a formatted duration string.
    /// </summary>
    [Fact]
    public void Convert_WithTimeSpan_ReturnsFormattedString()
    {
        // Arrange
        var converter = new TimeSpanConverter();
        var timeSpan = new TimeSpan(1, 2, 3, 4);

        // Act
        var result = converter.Convert(timeSpan, typeof(string), null, null);

        // Assert
        Assert.Equal("1d 02h 03m 04s", result);
    }

    /// <summary>
    /// Tests converting a null value returns null.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        // Arrange
        var converter = new TimeSpanConverter();

        // Act
        var result = converter.Convert(null, typeof(string), null, null);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack throws a NotImplementedException.
    /// </summary>
    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new TimeSpanConverter();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => converter.ConvertBack("1d 00h 00m 00s", typeof(TimeSpan), null, null));
    }
}
