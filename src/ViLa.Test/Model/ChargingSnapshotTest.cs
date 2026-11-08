using System.Globalization;
using System.Text.Json;
using ViLa.Model;
using ViLa.WWW.Api._1_;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies live units, empty measurements, and response caching.
/// </summary>
public class ChargingSnapshotTest
{
    /// <summary>
    /// Preserves an active session when start is requested twice before the first sample.
    /// </summary>
    [Fact]
    public void StartCharging_PreservesSessionAndInitialTimestamp()
    {
        // arrange
        var model = (ViewModel)Activator.CreateInstance(typeof(ViewModel), nonPublic: true)!;
        var before = DateTime.Now;

        // act
        model.StartCharging();
        var session = model.CurrentMeasurementLog;
        model.StartCharging();

        // validation
        Assert.Same(session, model.CurrentMeasurementLog);
        Assert.Empty(session.Measurements);
        Assert.InRange(session.From, before, DateTime.Now);
        Assert.True(model.ActiveCharging);
    }

    /// <summary>
    /// Leaves an inactive model unchanged when stop is requested.
    /// </summary>
    [Fact]
    public void StopCharging_WithoutSessionHasNoEffect()
    {
        // arrange
        var model = (ViewModel)Activator.CreateInstance(typeof(ViewModel), nonPublic: true)!;
        var original = model.CurrentMeasurementLog;

        // act
        model.StopCharging();

        // validation
        Assert.False(model.ActiveCharging);
        Assert.Same(original, model.CurrentMeasurementLog);
        Assert.Empty(model.Logging);
    }

    /// <summary>
    /// Registers both read endpoints with the WebExpress HTTP dispatcher.
    /// </summary>
    /// <param name="endpoint">The endpoint type inspected for its HTTP registration.</param>
    [Theory]
    [InlineData(typeof(Charging))]
    [InlineData(typeof(Tags))]
    public void ReadEndpoint_DeclaresGetMethod(Type endpoint)
    {
        // arrange
        var method = endpoint.GetMethod("Process")!;

        // act
        var attributes = method.GetCustomAttributes(typeof(MethodAttribute), false).Cast<MethodAttribute>();

        // validation
        Assert.Contains(attributes, attribute => attribute.RequestMethod == RequestMethod.GET);
    }

    /// <summary>
    /// Converts minute energy to kilowatts and preserves invariant chart numbers.
    /// </summary>
    [Fact]
    public void Create_ConvertsPowerAndLocalizesDisplay()
    {
        // arrange
        var model = ViewModel.Instance;
        var measurements = model.CurrentMeasurementLog.Measurements;
        var original = measurements.ToArray();
        try
        {
            measurements.Clear();
            measurements.Add(new MeasurementItem { Power = 0.1f });
            measurements.Add(new MeasurementItem { Power = 0.05f });

            // act
            var result = ChargingSnapshot.Create(model, CultureInfo.GetCultureInfo("de-DE"));

            // validation
            Assert.Equal("6,00", result.CurrentPower);
            Assert.Equal(new[] { "6", "3" }, result.ChartData);
            Assert.Equal(new[] { "-1", "0" }, result.ChartLabels);
        }
        finally
        {
            measurements.Clear();
            measurements.AddRange(original);
        }
    }

    /// <summary>
    /// Returns usable values for empty stored sessions.
    /// </summary>
    [Fact]
    public void MeasurementLog_EmptyMeasurementsUseFinalDates()
    {
        // arrange
        var start = new DateTime(2026, 1, 2);
        var log = new MeasurementLog { FinalFrom = start, FinalTill = start.AddHours(1) };

        // act
        var power = log.CurrentPower;

        // validation
        Assert.Equal(0, power);
        Assert.Equal(start, log.From);
        Assert.Equal(start.AddHours(1), log.Till);
        Assert.True(float.IsFinite(log.Power));
    }

    /// <summary>
    /// Disables HTTP caching while retaining the existing JSON property names.
    /// </summary>
    [Fact]
    public void Charging_DisablesCaching()
    {
        // arrange
        var endpoint = new Charging();

        // act
        var response = endpoint.Process(null!);
        using var json = JsonDocument.Parse(Assert.IsType<string>(response.Content));

        // validation
        Assert.Equal("no-store", response.Header.CacheControl);
        Assert.Equal(JsonValueKind.Array, json.RootElement.GetProperty("ChartData").ValueKind);
        Assert.Equal(JsonValueKind.String, json.RootElement.GetProperty("CurrentPower").ValueKind);
    }
}
