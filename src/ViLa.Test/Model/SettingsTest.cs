using System.IO;
using System.Text;
using System.Xml.Serialization;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="Settings"/> class.
/// </summary>
public class SettingsTest
{
    /// <summary>
    /// Tests getting and setting all settings properties.
    /// </summary>
    [Fact]
    public void Properties_SetAndGet_ReturnsExpectedValues()
    {
        // Arrange
        var settings = new Settings
        {
            DebugMode = true,
            ImpulsePerkWh = 1000,
            ElectricityPricePerkWh = 0.35f,
            MaxWattage = 22,
            MinWattage = 1.4f,
            MaxChargingTime = 5,
            Currency = "EUR",
            BillingDayOffset = 3,
            Mode = Mode.AutomaticControlled
        };

        // Assert
        Assert.True(settings.DebugMode);
        Assert.Equal(1000, settings.ImpulsePerkWh);
        Assert.Equal(0.35f, settings.ElectricityPricePerkWh);
        Assert.Equal(22, settings.MaxWattage);
        Assert.Equal(1.4f, settings.MinWattage);
        Assert.Equal(5, settings.MaxChargingTime);
        Assert.Equal("EUR", settings.Currency);
        Assert.Equal(3, settings.BillingDayOffset);
        Assert.Equal(Mode.AutomaticControlled, settings.Mode);
    }

    /// <summary>
    /// Tests XML serialization and deserialization of settings.
    /// </summary>
    [Fact]
    public void XmlSerialization_RoundTrip_PreservesProperties()
    {
        // Arrange
        var settings = new Settings
        {
            DebugMode = true,
            ImpulsePerkWh = 2000,
            ElectricityPricePerkWh = 0.42f,
            MaxWattage = 11,
            MinWattage = 2.0f,
            MaxChargingTime = 8,
            Currency = "€",
            BillingDayOffset = 15,
            Mode = Mode.TimeControlled
        };

        var serializer = new XmlSerializer(typeof(Settings));
        using var memoryStream = new MemoryStream();

        // Act
        serializer.Serialize(memoryStream, settings);
        memoryStream.Position = 0;
        var deserialized = (Settings?)serializer.Deserialize(memoryStream);

        // Assert
        Assert.NotNull(deserialized);
        Assert.True(deserialized.DebugMode);
        Assert.Equal(2000, deserialized.ImpulsePerkWh);
        Assert.Equal(0.42f, deserialized.ElectricityPricePerkWh);
        Assert.Equal(11, deserialized.MaxWattage);
        Assert.Equal(2.0f, deserialized.MinWattage);
        Assert.Equal(8, deserialized.MaxChargingTime);
        Assert.Equal("€", deserialized.Currency);
        Assert.Equal(15, deserialized.BillingDayOffset);
        Assert.Equal(Mode.TimeControlled, deserialized.Mode);
    }
}
