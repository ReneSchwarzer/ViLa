using ViLa.Model;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies vehicle persistence, deactivation, and catalog validation.
/// </summary>
public sealed class VehicleStoreTest : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "vila-vehicles-" + Guid.NewGuid().ToString("N"));

    /// <summary>
    /// Persists edits and deactivation while retaining stable identifiers.
    /// </summary>
    [Fact]
    public void Save_ReloadsUpdatedAndInactiveVehicle()
    {
        // arrange
        var path = Path.Combine(_directory, "vehicles.json");
        var store = new VehicleStore(path);

        // act
        var created = store.Save(null!, "Family car", 11, true, true);
        store.Save(created.Id, "Renamed car", 7.4, false, false);
        var restored = new VehicleStore(path).GetAll();

        // validation
        var vehicle = Assert.Single(restored);
        Assert.Equal(created.Id, vehicle.Id);
        Assert.Equal("Renamed car", vehicle.Name);
        Assert.Equal(7.4, vehicle.MaxPowerKw);
        Assert.False(vehicle.Active);
        Assert.False(vehicle.AutomaticAssignment);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    /// <summary>
    /// Rejects invalid writes without changing the existing catalog.
    /// </summary>
    [Fact]
    public void Save_InvalidChangesPreserveCatalog()
    {
        // arrange
        var path = Path.Combine(_directory, "vehicles.json");
        var store = new VehicleStore(path);
        store.Save(null!, "Car", 11, true, true);
        var original = File.ReadAllText(path);

        // act
        var invalidPower = Record.Exception(() => store.Save(null!, "Car", double.NaN, true, true));
        var unknownId = Record.Exception(() => store.Save(Guid.NewGuid().ToString(), "Other", 11, true, true));

        // validation
        Assert.IsType<ArgumentException>(invalidPower);
        Assert.IsType<ArgumentException>(unknownId);
        Assert.Equal(original, File.ReadAllText(path));
    }

    /// <summary>
    /// Persists deletion while retaining other profiles and rejecting an incorrect confirmation.
    /// </summary>
    [Fact]
    public void Delete_RequiresMatchingNameAndPersistsRemoval()
    {
        // arrange
        var path = Path.Combine(_directory, "vehicles.json");
        var store = new VehicleStore(path);
        var removed = store.Save(null!, "Family car", 11, true, true);
        var retained = store.Save(null!, "Other car", 7.4, false, false);

        // act
        var incorrect = store.Delete(removed.Id, "Wrong name");
        var before = new VehicleStore(path).GetAll();
        var deleted = store.Delete(removed.Id, removed.Name);
        var repeated = store.Delete(removed.Id, removed.Name);
        var restored = new VehicleStore(path).GetAll();

        // validation
        Assert.False(incorrect);
        Assert.Equal(2, before.Length);
        Assert.True(deleted);
        Assert.False(repeated);
        Assert.Null(store.GetVehicle(removed.Id));
        Assert.Equal(retained.Id, Assert.Single(restored).Id);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    /// <summary>
    /// Keeps the in-memory profile when replacing the catalog fails.
    /// </summary>
    [Fact]
    public void Delete_WriteFailurePreservesProfile()
    {
        // arrange
        var path = Path.Combine(_directory, "vehicles.json");
        var store = new VehicleStore(path);
        var vehicle = store.Save(null!, "Family car", 11, true, true);
        File.Delete(path);
        Directory.CreateDirectory(path);

        // act
        var exception = Record.Exception(() => store.Delete(vehicle.Id, vehicle.Name));
        Directory.Delete(path);

        // validation
        Assert.NotNull(exception);
        Assert.Equal(vehicle.Id, Assert.Single(store.GetAll()).Id);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    /// <summary>
    /// Removes only the files created in this test's dedicated temporary directory.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            foreach (var file in Directory.GetFiles(_directory))
            {
                File.Delete(file);
            }
            Directory.Delete(_directory);
        }
    }
}
