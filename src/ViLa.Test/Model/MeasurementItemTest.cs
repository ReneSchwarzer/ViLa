using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="MeasurementItem"/> class.
/// </summary>
public class MeasurementItemTest
{
    /// <summary>
    /// Tests getting and setting all properties.
    /// </summary>
    [Fact]
    public void Properties_SetAndGet_ReturnsExpectedValues()
    {
        // Arrange
        var time = new DateTime(2026, 4, 1, 14, 0, 0);
        var item = new MeasurementItem
        {
            MeasurementTimePoint = time,
            Impulse = 150,
            Power = 0.15f
        };
        item.Logitems.Add(new LogItem(LogItem.LogLevel.Info, "Measurement logged"));

        // Assert
        Assert.Equal(time, item.MeasurementTimePoint);
        Assert.Equal(150, item.Impulse);
        Assert.Equal(0.15f, item.Power);
        Assert.Single(item.Logitems);
        Assert.Equal("Measurement logged", item.Logitems[0].Massage);
    }

    /// <summary>
    /// Tests XML serialization and deserialization of a measurement item.
    /// </summary>
    [Fact]
    public void XmlSerialization_RoundTrip_PreservesProperties()
    {
        // Arrange
        var item = new MeasurementItem
        {
            MeasurementTimePoint = new DateTime(2026, 4, 1, 14, 30, 0),
            Impulse = 250,
            Power = 0.25f
        };
        item.Logitems.Add(new LogItem { Level = LogItem.LogLevel.Debug, Massage = "Sample debug" });

        var serializer = new XmlSerializer(typeof(MeasurementItem));
        using var memoryStream = new MemoryStream();

        // Act
        serializer.Serialize(memoryStream, item);
        memoryStream.Position = 0;
        var deserialized = (MeasurementItem?)serializer.Deserialize(memoryStream);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(item.MeasurementTimePoint, deserialized.MeasurementTimePoint);
        Assert.Equal(250, deserialized.Impulse);
        Assert.Equal(0.25f, deserialized.Power);
    }
}
