using System.Globalization;
using System.Xml.Serialization;
using ViLa.Model;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies conservative power recognition and backward compatible assignments.
/// </summary>
public class VehicleRecognitionTest
{
    /// <summary>
    /// Matches a stable maximum against distinct charging profiles.
    /// </summary>
    /// <param name="power">The sustained measured power.</param>
    /// <param name="expected">The expected profile power.</param>
    [Theory]
    [InlineData(3.65, 3.7)]
    [InlineData(7.3, 7.4)]
    [InlineData(10.8, 11)]
    public void Find_RecognizesDistinctMaximum(double power, double expected)
    {
        // arrange
        var vehicles = new[] { Profile(3.7), Profile(7.4), Profile(11) };
        var log = Session(power, power, power, 0);

        // act
        var result = VehicleRecognition.Find(log, vehicles);

        // validation
        Assert.Equal(expected, result?.MaxPowerKw);
    }

    /// <summary>
    /// Leaves equal and overlapping maximum power profiles unassigned.
    /// </summary>
    /// <param name="secondPower">The competing maximum power.</param>
    [Theory]
    [InlineData(11)]
    [InlineData(11.3)]
    public void Find_RejectsAmbiguousProfiles(double secondPower)
    {
        // arrange
        var vehicles = new[] { Profile(11), Profile(secondPower) };
        var log = Session(11, 11, 11);

        // act
        var result = VehicleRecognition.Find(log, vehicles);

        // validation
        Assert.Null(result);
    }

    /// <summary>
    /// Rejects insufficient, interrupted, excessive, and invalid power evidence.
    /// </summary>
    /// <param name="power">The measured power samples in kilowatts.</param>
    [Theory]
    [InlineData(new double[] { })]
    [InlineData(new double[] { 11, 11 })]
    [InlineData(new double[] { 0, 0, 0 })]
    [InlineData(new double[] { 7, 7, 7 })]
    [InlineData(new double[] { 11, 0, 11, 0, 11 })]
    [InlineData(new double[] { 11, 11, 11, 22 })]
    [InlineData(new double[] { 11, double.NaN, 11 })]
    public void Find_RejectsUnreliableEvidence(double[] power)
    {
        // arrange
        var log = Session(power);

        // act
        var result = VehicleRecognition.Find(log, new[] { Profile(11) });

        // validation
        Assert.Null(result);
    }

    /// <summary>
    /// Requires contiguous minute samples rather than three unrelated peaks.
    /// </summary>
    [Fact]
    public void Find_RejectsTimestampGaps()
    {
        // arrange
        var log = Session(11, 11, 11);
        log.Measurements[1].MeasurementTimePoint = log.Measurements[0].MeasurementTimePoint.AddMinutes(10);

        // act
        var result = VehicleRecognition.Find(log, new[] { Profile(11) });

        // validation
        Assert.Null(result);
    }

    /// <summary>
    /// Excludes inactive profiles but includes manual profiles when checking ambiguity.
    /// </summary>
    [Fact]
    public void Find_RespectsVehicleAvailabilityAndAutomaticSetting()
    {
        // arrange
        var enabled = Profile(11);
        var manual = Profile(11) with { AutomaticAssignment = false };
        var inactive = Profile(11) with { Active = false };
        var log = Session(11, 11, 11);

        // act
        var ambiguous = VehicleRecognition.Find(log, new[] { enabled, manual });
        var unique = VehicleRecognition.Find(log, new[] { enabled, inactive });
        var disabled = VehicleRecognition.Find(log, new[] { manual });

        // validation
        Assert.Null(ambiguous);
        Assert.Same(enabled, unique);
        Assert.Null(disabled);
    }

    /// <summary>
    /// Preserves both manual assignment and deliberate manual non-assignment.
    /// </summary>
    /// <param name="vehicleId">The manually selected identifier, or null.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("manual-car")]
    public void Apply_PreservesManualDecision(string? vehicleId)
    {
        // arrange
        var log = Session(11, 11, 11);
        log.VehicleId = vehicleId;
        log.VehicleName = vehicleId;
        log.VehicleAssignment = VehicleAssignment.Manual;

        // act
        VehicleRecognition.Apply(log, new[] { Profile(11) });

        // validation
        Assert.Equal(vehicleId, log.VehicleId);
        Assert.Equal(vehicleId, log.VehicleName);
        Assert.Equal(VehicleAssignment.Manual, log.VehicleAssignment);
    }

    /// <summary>
    /// Stores recognition results in XML while reading older logs without vehicle fields.
    /// </summary>
    [Fact]
    public void Assignment_RoundTripsAndSupportsLegacyXml()
    {
        // arrange
        var vehicle = Profile(11);
        var log = Session(11, 11, 11);
        var serializer = new XmlSerializer(typeof(MeasurementLog));
        using var writer = new StringWriter();

        // act
        VehicleRecognition.Apply(log, new[] { vehicle });
        serializer.Serialize(writer, log);
        using var reader = new StringReader(writer.ToString());
        var restored = (MeasurementLog)serializer.Deserialize(reader)!;
        using var legacy = new StringReader("<measurementlog id=\"legacy\" />");
        var old = (MeasurementLog)serializer.Deserialize(legacy)!;

        // validation
        Assert.Equal(vehicle.Id, restored.VehicleId);
        Assert.Equal(vehicle.Name, restored.VehicleName);
        Assert.Equal(VehicleAssignment.Automatic, restored.VehicleAssignment);
        Assert.Null(old.VehicleId);
        Assert.Equal(VehicleAssignment.Unassigned, old.VehicleAssignment);
    }

    /// <summary>
    /// Parses local decimal separators without silently accepting thousands separators.
    /// </summary>
    /// <param name="text">The submitted text.</param>
    /// <param name="expected">Whether the value is valid.</param>
    [Theory]
    [InlineData("7,4", true)]
    [InlineData("7.4", true)]
    [InlineData("0", false)]
    [InlineData("-1", false)]
    [InlineData("NaN", false)]
    [InlineData("1.000,5", false)]
    [InlineData("1001", false)]
    public void TryParsePower_ValidatesLocalizedInput(string text, bool expected)
    {
        // arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // act
        var valid = Vehicle.TryParsePower(text, culture, out var power);

        // validation
        Assert.Equal(expected, valid);
        if (expected)
        {
            Assert.Equal(7.4, power);
        }
    }

    /// <summary>
    /// Creates a vehicle with a distinct identifier.
    /// </summary>
    /// <param name="power">The expected maximum power.</param>
    /// <returns>The vehicle profile.</returns>
    private static Vehicle Profile(double power)
    {
        return new Vehicle { Id = Guid.NewGuid().ToString(), Name = "Car " + power, MaxPowerKw = power };
    }

    /// <summary>
    /// Creates successive minute samples from kilowatt readings.
    /// </summary>
    /// <param name="power">The power readings in kilowatts.</param>
    /// <returns>A measurement log containing minute energy values.</returns>
    private static MeasurementLog Session(params double[] power)
    {
        return new MeasurementLog
        {
            ID = Guid.NewGuid().ToString(),
            Measurements = power.Select((value, index) => new MeasurementItem
            {
                MeasurementTimePoint = new DateTime(2026, 1, 1).AddMinutes(index),
                Power = (float)(value / 60)
            }).ToList()
        };
    }
}
