using System;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="LogItem"/> class.
/// </summary>
public class LogItemTest
{
    /// <summary>
    /// Tests the parameterless default constructor.
    /// </summary>
    [Fact]
    public void DefaultConstructor_InitializesEmpty()
    {
        // Act
        var logItem = new LogItem();

        // Assert
        Assert.Equal(LogItem.LogLevel.Info, logItem.Level);
        Assert.Null(logItem.Massage);
        Assert.Null(logItem.Instance);
    }

    /// <summary>
    /// Tests the parameterized constructor and automatic capture of the caller member name.
    /// </summary>
    [Fact]
    public void ParameterizedConstructor_SetsPropertiesAndCallerName()
    {
        // Arrange & Act
        var before = DateTime.Now;
        var logItem = new LogItem(LogItem.LogLevel.Warning, "Test message");
        var after = DateTime.Now;

        // Assert
        Assert.Equal(LogItem.LogLevel.Warning, logItem.Level);
        Assert.Equal("Test message", logItem.Massage);
        Assert.InRange(logItem.Time, before, after);
        Assert.Equal(nameof(ParameterizedConstructor_SetsPropertiesAndCallerName), logItem.Instance);
    }

    /// <summary>
    /// Tests getting and setting properties for all log levels.
    /// </summary>
    [Theory]
    [InlineData(LogItem.LogLevel.Info)]
    [InlineData(LogItem.LogLevel.Debug)]
    [InlineData(LogItem.LogLevel.Warning)]
    [InlineData(LogItem.LogLevel.Error)]
    [InlineData(LogItem.LogLevel.Exception)]
    public void Properties_CanBeSetAndRetrieved(LogItem.LogLevel level)
    {
        // Arrange
        var logItem = new LogItem
        {
            Level = level,
            Massage = "Custom message",
            Time = new DateTime(2026, 1, 1, 12, 0, 0),
            Instance = "CustomInstance"
        };

        // Assert
        Assert.Equal(level, logItem.Level);
        Assert.Equal("Custom message", logItem.Massage);
        Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 0), logItem.Time);
        Assert.Equal("CustomInstance", logItem.Instance);
    }
}
