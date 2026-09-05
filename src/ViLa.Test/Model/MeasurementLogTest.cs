using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="MeasurementLog"/> class.
/// </summary>
public class MeasurementLogTest
{
    /// <summary>
    /// Tests calculated properties such as time range, impulses, power, and cost.
    /// </summary>
    [Fact]
    public void ComputedProperties_WithMeasurements_ReturnsCalculatedValues()
    {
        // Arrange
        ViewModel.Instance.Settings.ImpulsePerkWh = 1000;
        ViewModel.Instance.Settings.ElectricityPricePerkWh = 0.30f;

        var t1 = new DateTime(2026, 6, 1, 10, 0, 0);
        var t2 = new DateTime(2026, 6, 1, 10, 1, 0);
        var t3 = new DateTime(2026, 6, 1, 10, 2, 0);

        var log = new MeasurementLog
        {
            ID = "log-001",
            Client = "Tenant1",
            Tag = "Car1 Home",
            Measurements = new List<MeasurementItem>
            {
                new() { MeasurementTimePoint = t1, Impulse = 500, Power = 0.5f },
                new() { MeasurementTimePoint = t2, Impulse = 600, Power = 0.6f },
                new() { MeasurementTimePoint = t3, Impulse = 700, Power = 0.7f }
            }
        };

        // Assert
        Assert.Equal("log-001", log.ID);
        Assert.Equal("Tenant1", log.Client);
        Assert.Equal("Car1 Home", log.Tag);
        Assert.Equal(t1, log.From);
        Assert.Equal(t3, log.Till);
        Assert.Equal(1800, log.Impulse);
        Assert.Equal(1.8f, log.Power);
        Assert.Equal(1.8f * 0.30f, log.Cost, precision: 4);
        Assert.Equal(0.6f, log.CurrentPower);
        Assert.Equal(t3, log.CurrentMeasurement.MeasurementTimePoint);
    }

    /// <summary>
    /// Tests current power retrieval with a single measurement.
    /// </summary>
    [Fact]
    public void CurrentPower_WithSingleMeasurement_ReturnsFirstPower()
    {
        // Arrange
        var t1 = new DateTime(2026, 6, 1, 10, 0, 0);
        var log = new MeasurementLog
        {
            Measurements = new List<MeasurementItem>
            {
                new() { MeasurementTimePoint = t1, Impulse = 100, Power = 0.1f }
            }
        };

        // Assert
        Assert.Equal(0.1f, log.CurrentPower);
    }

    /// <summary>
    /// Tests resetting the measurements of a log.
    /// </summary>
    [Fact]
    public void Reset_ClearsMeasurements()
    {
        // Arrange
        var log = new MeasurementLog
        {
            Measurements = new List<MeasurementItem>
            {
                new() { MeasurementTimePoint = DateTime.Now, Impulse = 100, Power = 0.1f }
            }
        };

        // Act
        log.Reset();

        // Assert
        Assert.Empty(log.Measurements);
    }

    /// <summary>
    /// Tests full XML serialization and deserialization of a measurement log.
    /// </summary>
    [Fact]
    public void XmlSerialization_RoundTrip_PreservesAllAttributesAndElements()
    {
        // Arrange
        var from = new DateTime(2026, 6, 1, 10, 0, 0);
        var till = new DateTime(2026, 6, 1, 11, 0, 0);

        var log = new MeasurementLog
        {
            ID = "test-log-id",
            Client = "DefaultClient",
            FinalPower = 15.5f,
            FinalCost = 4.65f,
            FinalFrom = from,
            FinalTill = till,
            ElectricityPricePerkWh = 0.30f,
            ImpulsePerkWh = 1000,
            Currency = "EUR",
            Tag = "Tag1 Tag2",
            Measurements = new List<MeasurementItem>
            {
                new() { MeasurementTimePoint = from, Impulse = 500, Power = 0.5f }
            },
            Comments = new List<CommentItem>
            {
                new() { Guid = "c1", Comment = "Test comment", Created = from, Updated = till }
            }
        };

        var serializer = new XmlSerializer(typeof(MeasurementLog));
        using var memoryStream = new MemoryStream();

        // Act
        serializer.Serialize(memoryStream, log);
        memoryStream.Position = 0;
        var deserialized = (MeasurementLog?)serializer.Deserialize(memoryStream);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("test-log-id", deserialized.ID);
        Assert.Equal("DefaultClient", deserialized.Client);
        Assert.Equal(15.5f, deserialized.FinalPower);
        Assert.Equal(4.65f, deserialized.FinalCost);
        Assert.Equal(from, deserialized.FinalFrom);
        Assert.Equal(till, deserialized.FinalTill);
        Assert.Equal(0.30f, deserialized.ElectricityPricePerkWh);
        Assert.Equal(1000, deserialized.ImpulsePerkWh);
        Assert.Equal("EUR", deserialized.Currency);
        Assert.Equal("Tag1 Tag2", deserialized.Tag);
        Assert.Single(deserialized.Measurements);
        Assert.Single(deserialized.Comments);
    }
}
