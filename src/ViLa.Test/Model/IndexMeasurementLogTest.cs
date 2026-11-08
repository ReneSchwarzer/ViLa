using System;
using System.Collections.Generic;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="IndexMeasurementLog"/> class.
/// </summary>
public class IndexMeasurementLogTest
{
    /// <summary>
    /// Tests the default behavior when the underlying measurement log is null.
    /// </summary>
    [Fact]
    public void Properties_WithNullLog_ReturnsDefaults()
    {
        // Arrange
        var indexItem = new IndexMeasurementLog { Log = null };

        // Assert
        Assert.Equal(Guid.Empty, indexItem.Id);
        Assert.Equal(DateTime.MinValue, indexItem.From);
        Assert.Equal(DateTime.MinValue, indexItem.Till);
        Assert.Equal(0f, indexItem.Power);
        Assert.Equal(0f, indexItem.Cost);
        Assert.Equal(string.Empty, indexItem.Tag);
    }

    /// <summary>
    /// Tests that an empty ID returns Guid.Empty.
    /// </summary>
    [Fact]
    public void Properties_WithEmptyId_ReturnsGuidEmpty()
    {
        // Arrange
        var indexItem = new IndexMeasurementLog
        {
            Log = new MeasurementLog { ID = "" }
        };

        // Assert
        Assert.Equal(Guid.Empty, indexItem.Id);
    }

    /// <summary>
    /// Tests parsing a valid GUID as ID.
    /// </summary>
    [Fact]
    public void Properties_WithValidGuid_ParsesId()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var indexItem = new IndexMeasurementLog
        {
            Log = new MeasurementLog { ID = guid.ToString() }
        };

        // Assert
        Assert.Equal(guid, indexItem.Id);
    }

    /// <summary>
    /// Tests exposing final power and cost values when provided.
    /// </summary>
    [Fact]
    public void PowerAndCost_WithFinalValues_ReturnsFinalValues()
    {
        // Arrange
        var indexItem = new IndexMeasurementLog
        {
            Log = new MeasurementLog
            {
                FinalPower = 12.5f,
                FinalCost = 3.75f,
                Tag = "Home"
            }
        };

        // Assert
        Assert.Equal(12.5f, indexItem.Power);
        Assert.Equal(3.75f, indexItem.Cost);
        Assert.Equal("Home", indexItem.Tag);
    }

    /// <summary>
    /// Tests fallback to computed values when final power and cost are NaN.
    /// </summary>
    [Fact]
    public void PowerAndCost_WithNaNFinalValues_FallsBackToComputedValues()
    {
        // Arrange
        ViewModel.Instance.Settings.ImpulsePerkWh = 1000;
        ViewModel.Instance.Settings.ElectricityPricePerkWh = 0.40f;

        var t1 = new DateTime(2026, 7, 1, 12, 0, 0);
        var t2 = new DateTime(2026, 7, 1, 12, 30, 0);

        var log = new MeasurementLog
        {
            FinalPower = float.NaN,
            FinalCost = float.NaN,
            Measurements = new List<MeasurementItem>
            {
                new() { MeasurementTimePoint = t1, Impulse = 1000, Power = 1.0f },
                new() { MeasurementTimePoint = t2, Impulse = 2000, Power = 2.0f }
            }
        };

        var indexItem = new IndexMeasurementLog { Log = log };

        // Assert
        Assert.Equal(t1, indexItem.From);
        Assert.Equal(t2, indexItem.Till);
        Assert.Equal(3.0f, indexItem.Power);
        Assert.Equal(1.2f, indexItem.Cost, precision: 4);
    }
}
