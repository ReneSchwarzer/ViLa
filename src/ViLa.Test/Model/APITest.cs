using System.Text.Json;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="API"/> class.
/// </summary>
public class APITest
{
    /// <summary>
    /// Tests getting and setting all DTO properties.
    /// </summary>
    [Fact]
    public void Properties_SetAndGet_ReturnsExpectedValues()
    {
        // Arrange
        var api = new API
        {
            Client = "DefaultClient",
            ActiveCharging = true,
            MeasurementTime = "0d 01h 15m 00s",
            Impulse = "1500",
            Power = "1.50",
            Cost = "0.45",
            CurrentPower = "3.60",
            Now = "05.09.2026<br>18:00:00",
            ChartLabels = ["0", "1", "2"],
            ChartData = ["1.2", "2.4", "3.6"]
        };

        // Assert
        Assert.Equal("DefaultClient", api.Client);
        Assert.True(api.ActiveCharging);
        Assert.Equal("0d 01h 15m 00s", api.MeasurementTime);
        Assert.Equal("1500", api.Impulse);
        Assert.Equal("1.50", api.Power);
        Assert.Equal("0.45", api.Cost);
        Assert.Equal("3.60", api.CurrentPower);
        Assert.Equal("05.09.2026<br>18:00:00", api.Now);
        Assert.Equal(3, api.ChartLabels.Length);
        Assert.Equal(3, api.ChartData.Length);
    }

    /// <summary>
    /// Tests JSON serialization and deserialization of the DTO.
    /// </summary>
    [Fact]
    public void JsonSerialization_RoundTrip_PreservesProperties()
    {
        // Arrange
        var api = new API
        {
            Client = "TestTenant",
            ActiveCharging = false,
            MeasurementTime = "-",
            Impulse = "0",
            Power = "0.00",
            Cost = "0.00",
            CurrentPower = "0.00",
            Now = "01.01.2026<br>00:00:00",
            ChartLabels = ["1", "2"],
            ChartData = ["0.0", "0.0"]
        };

        // Act
        var json = JsonSerializer.Serialize(api);
        var deserialized = JsonSerializer.Deserialize<API>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("TestTenant", deserialized.Client);
        Assert.False(deserialized.ActiveCharging);
        Assert.Equal("-", deserialized.MeasurementTime);
        Assert.Equal(2, deserialized.ChartLabels.Length);
        Assert.Equal(2, deserialized.ChartData.Length);
    }
}
