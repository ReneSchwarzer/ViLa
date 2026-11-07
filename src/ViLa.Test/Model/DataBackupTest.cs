using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using ViLa.Model;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies portable backups, optional archives and additive restoration.
/// </summary>
public sealed class DataBackupTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "vila-backup-" + Guid.NewGuid().ToString("N"));

    /// <summary>
    /// Publishes committed history and settings after a later archive write fails and supports a safe retry.
    /// </summary>
    [Fact]
    public void Import_PublishesCommittedRecordsAfterStorageFailure()
    {
        // arrange
        var current = new MeasurementLog { ID = Guid.NewGuid().ToString() };
        var archived = new MeasurementLog { ID = Guid.NewGuid().ToString() };
        WriteXml(Path.Combine(_root, "source", "measurements", current.ID + ".xml"), current);
        WriteXml(Path.Combine(_root, "source", "archive", "2026", "11", archived.ID + ".xml"), archived);
        var bytes = new DataBackup(Path.Combine(_root, "source"), Store("source")).Export(new Settings(), true);
        var service = new DataBackup(Path.Combine(_root, "target"), Store("target"));
        Directory.CreateDirectory(Path.Combine(_root, "target"));
        var obstacle = Path.Combine(_root, "target", "archive");
        File.WriteAllText(obstacle, "blocks archive directory creation");
        var history = new List<MeasurementLog>();
        BackupImportResult? partial = null;

        // act
        var failure = Record.Exception(() => service.Import(bytes, publish: result =>
        {
            partial = result;
            history.AddRange(result.Measurements);
        }));
        File.Delete(obstacle);
        var repeated = service.Import(bytes, history.Select(x => x.ID), result => history.AddRange(result.Measurements));

        // validation
        Assert.IsAssignableFrom<IOException>(failure);
        Assert.NotNull(partial);
        Assert.True(partial.SettingsAdded);
        Assert.Equal(1, partial.MeasurementsAdded);
        Assert.Equal(current.ID, Assert.Single(history).ID);
        Assert.Equal(1, repeated.MeasurementsAdded);
        Assert.Empty(repeated.Measurements);
        Assert.False(repeated.SettingsAdded);
        Assert.True(File.Exists(Path.Combine(_root, "target", "archive", "2026", "11", archived.ID + ".xml")));
    }

    /// <summary>
    /// Waits for an exclusive measurement writer before reading the backup snapshot.
    /// </summary>
    [Fact]
    public void Export_WaitsForMeasurementWriter()
    {
        // arrange
        var log = new MeasurementLog { ID = Guid.NewGuid().ToString() };
        var path = Path.Combine(_root, "source", "measurements", log.ID + ".xml");
        WriteXml(path, log);
        var service = new DataBackup(Path.Combine(_root, "source"), Store("source"));
        var storageLock = typeof(DataBackup).GetProperty("StorageLock",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!;
        using var started = new ManualResetEventSlim();
        using var completed = new ManualResetEventSlim();
        byte[]? bytes = null;
        Exception? failure = null;
        var exporter = new Thread(() =>
        {
            started.Set();
            try
            {
                bytes = service.Export(new Settings());
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                completed.Set();
            }
        }) { IsBackground = true };
        bool completedDuringWrite;

        // act
        lock (storageLock)
        {
            using var writer = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            exporter.Start();
            Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
            completedDuringWrite = completed.Wait(TimeSpan.FromMilliseconds(250));
        }
        var finished = exporter.Join(TimeSpan.FromSeconds(5));

        // validation
        Assert.False(completedDuringWrite);
        Assert.True(finished);
        Assert.Null(failure);
        Assert.NotNull(bytes);
        var restored = new DataBackup(Path.Combine(_root, "target"), Store("target")).Import(bytes);
        Assert.Equal(log.ID, Assert.Single(restored.Measurements).ID);
    }

    /// <summary>
    /// Accepts explicit ZIP directory entries without treating them as measurement records.
    /// </summary>
    [Fact]
    public void Import_AcceptsDirectoryEntries()
    {
        // arrange
        var bytes = new DataBackup(Path.Combine(_root, "source"), Store("source")).Export(new Settings());
        using var stream = new MemoryStream();
        stream.Write(bytes);
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            foreach (var directory in new[] { "measurements/", "archive/", "archive/2026/", "archive/2026/11/" })
            {
                zip.CreateEntry(directory);
            }
        }

        // act
        var result = new DataBackup(Path.Combine(_root, "target"), Store("target")).Import(stream.ToArray());

        // validation
        Assert.True(result.SettingsAdded);
        Assert.Equal(0, result.MeasurementsAdded);
    }

    /// <summary>
    /// Restores complete records and preserves their nested metadata.
    /// </summary>
    /// <param name="includeArchive">Whether archived measurements should be restored.</param>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ExportImport_RoundTripsDataWithOptionalArchive(bool includeArchive)
    {
        // arrange
        var source = Store("source");
        var vehicle = source.Save(null!, "Family car", 11, false, false, "#123456");
        var log = new MeasurementLog
        {
            ID = Guid.NewGuid().ToString(), Description = "Description", Tag = "home;work",
            VehicleId = vehicle.Id, VehicleName = vehicle.Name, VehicleAssignment = VehicleAssignment.Manual,
            FinalPower = 12, FinalCost = 3, Currency = "EUR",
            Measurements = [new MeasurementItem { Impulse = 100, MeasurementTimePoint = new DateTime(2026, 1, 1) }],
            Comments = [new CommentItem { Comment = "Comment" }]
        };
        WriteXml(Path.Combine(_root, "source", "data", "measurements", log.ID + ".xml"), log);
        var archived = new MeasurementLog { ID = Guid.NewGuid().ToString() };
        WriteXml(Path.Combine(_root, "source", "data", "archive", "2020", "01", archived.ID + ".xml"), archived);
        var target = Store("target");

        // act
        var bytes = new DataBackup(Path.Combine(_root, "source"), source).Export(new Settings { Currency = "EUR", ImpulsePerkWh = 1000 }, includeArchive);
        var service = new DataBackup(Path.Combine(_root, "target"), target);
        var result = service.Import(bytes);
        var repeated = service.Import(bytes);

        // validation
        using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.NotNull(archive.GetEntry("settings.json"));
        Assert.Null(archive.GetEntry("settings.xml"));
        Assert.NotNull(archive.GetEntry("measurements/" + log.ID + ".xml"));
        Assert.Equal("EUR", SettingsStore.Deserialize(File.ReadAllBytes(service.SettingsPath)).Currency);
        Assert.Equal(includeArchive ? 2 : 1, result.MeasurementsAdded);
        Assert.Equal(1, result.VehiclesAdded);
        Assert.True(result.SettingsAdded);
        var restored = Assert.Single(result.Measurements);
        Assert.Equal(log.Description, restored.Description);
        Assert.Equal(log.Tag, restored.Tag);
        Assert.Equal(log.VehicleId, restored.VehicleId);
        Assert.Equal(log.VehicleAssignment, restored.VehicleAssignment);
        Assert.Equal(100, Assert.Single(restored.Measurements).Impulse);
        Assert.Equal("Comment", Assert.Single(restored.Comments).Comment);
        Assert.Equal(vehicle, Assert.Single(new VehicleStore(Path.Combine(_root, "target", "data", "vehicles.json")).GetAll()));
        Assert.Equal(includeArchive, File.Exists(Path.Combine(_root, "target", "archive", "2020", "01", archived.ID + ".xml")));
        Assert.Equal(0, repeated.MeasurementsAdded);
        Assert.Equal(0, repeated.VehiclesAdded);
        Assert.False(repeated.SettingsAdded);
    }

    /// <summary>
    /// Imports the original XML backup format into local JSON settings.
    /// </summary>
    [Fact]
    public void Import_LegacyBackupMigratesSettingsToJson()
    {
        // arrange
        var bytes = new DataBackup(Path.Combine(_root, "source"), Store("source")).Export(new Settings());
        using var stream = new MemoryStream();
        stream.Write(bytes);
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            zip.GetEntry("settings.json")!.Delete();
            zip.GetEntry("version.txt")!.Delete();
            using (var writer = new StreamWriter(zip.CreateEntry("version.txt").Open(), new UTF8Encoding(false)))
            {
                writer.Write("ViLa/1");
            }
            using var xml = zip.CreateEntry("settings.xml").Open();
            new XmlSerializer(typeof(Settings)).Serialize(xml, new Settings { Currency = "EUR", ImpulsePerkWh = 1234 });
        }
        var service = new DataBackup(Path.Combine(_root, "target"), Store("target"));

        // act
        var result = service.Import(stream.ToArray());

        // validation
        Assert.True(result.SettingsAdded);
        Assert.Equal(1234, SettingsStore.Deserialize(File.ReadAllBytes(service.SettingsPath)).ImpulsePerkWh);
        Assert.False(File.Exists(Path.Combine(_root, "target", "vila.settings.xml")));
    }

    /// <summary>
    /// Preserves a local JSON settings file when importing different settings.
    /// </summary>
    [Fact]
    public void Import_PreservesExistingJsonSettings()
    {
        // arrange
        var settings = new SettingsStore(Path.Combine(_root, "target"));
        settings.Save(new Settings { Currency = "EUR", ImpulsePerkWh = 777 });
        var original = File.ReadAllBytes(settings.FilePath);
        var bytes = new DataBackup(Path.Combine(_root, "source"), Store("source")).Export(new Settings { Currency = "USD" });

        // act
        var result = new DataBackup(Path.Combine(_root, "target"), Store("target")).Import(bytes);

        // validation
        Assert.False(result.SettingsAdded);
        Assert.Equal(original, File.ReadAllBytes(settings.FilePath));
    }

    /// <summary>
    /// Preserves settings, conflicting vehicles and measurement identities across archive locations.
    /// </summary>
    [Fact]
    public void Import_PreservesExistingDataAndReservedIdentities()
    {
        // arrange
        var source = Store("source");
        var vehicle = source.Save(null!, "Original", 11, true, true);
        var conflict = new MeasurementLog { ID = Guid.NewGuid().ToString(), Description = "Incoming" };
        var reserved = new MeasurementLog { ID = Guid.NewGuid().ToString() };
        WriteXml(Path.Combine(_root, "source", "measurements", conflict.ID + ".xml"), conflict);
        WriteXml(Path.Combine(_root, "source", "measurements", reserved.ID + ".xml"), reserved);
        var backup = new DataBackup(Path.Combine(_root, "source"), source).Export(new Settings { Currency = "USD" });
        var target = Store("target");
        target.ImportMissing([vehicle]);
        target.Save(vehicle.Id, "Local", 7.4, false, false);
        var settingsPath = Path.Combine(_root, "target", "Config", "vila.settings.xml");
        WriteXml(settingsPath, new Settings { Currency = "EUR" });
        var archivePath = Path.Combine(_root, "target", "archive", "2020", "01", conflict.ID + ".xml");
        conflict.Description = "Local";
        WriteXml(archivePath, conflict);
        var settingsBefore = File.ReadAllBytes(settingsPath);
        var archiveBefore = File.ReadAllBytes(archivePath);

        // act
        var result = new DataBackup(Path.Combine(_root, "target"), target).Import(backup, [reserved.ID]);

        // validation
        Assert.Equal(0, result.MeasurementsAdded);
        Assert.Equal(0, result.VehiclesAdded);
        Assert.False(result.SettingsAdded);
        Assert.Equal(4, result.Skipped);
        Assert.Equal(settingsBefore, File.ReadAllBytes(settingsPath));
        Assert.Equal(archiveBefore, File.ReadAllBytes(archivePath));
        Assert.Equal("Local", Assert.Single(target.GetAll()).Name);
        Assert.False(Directory.Exists(Path.Combine(_root, "target", "measurements")));
    }

    /// <summary>
    /// Rejects malformed archives before writing even their otherwise valid records.
    /// </summary>
    /// <param name="entryName">The invalid entry path.</param>
    /// <param name="content">The invalid entry content.</param>
    [Theory]
    [InlineData("../outside.xml", "invalid")]
    [InlineData("archive/../../outside.xml", "invalid")]
    [InlineData("measurements/00000000-0000-0000-0000-000000000001.xml", "invalid")]
    [InlineData("version.txt", "ViLa/99")]
    [InlineData("vehicles.json", "[null]")]
    [InlineData("settings.json", "null")]
    public void Import_InvalidArchiveDoesNotWrite(string entryName, string content)
    {
        // arrange
        var source = Store("source");
        source.Save(null!, "Car", 11, true, true);
        var bytes = new DataBackup(Path.Combine(_root, "source"), source).Export(new Settings());
        using var stream = new MemoryStream();
        stream.Write(bytes);
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            zip.GetEntry(entryName)?.Delete();
            using var writer = new StreamWriter(zip.CreateEntry(entryName).Open(), Encoding.UTF8);
            writer.Write(content);
        }
        var target = Store("target");

        // act
        var exception = Record.Exception(() => new DataBackup(Path.Combine(_root, "target"), target).Import(stream.ToArray()));

        // validation
        Assert.NotNull(exception);
        Assert.Empty(target.GetAll());
        Assert.False(Directory.Exists(Path.Combine(_root, "target")));
    }

    /// <summary>
    /// Creates an isolated catalog for a source or destination fixture.
    /// </summary>
    /// <param name="name">The fixture directory name.</param>
    /// <returns>The isolated vehicle store.</returns>
    private VehicleStore Store(string name)
    {
        return new VehicleStore(Path.Combine(_root, name, "data", "vehicles.json"));
    }

    /// <summary>
    /// Writes a persisted XML fixture.
    /// </summary>
    /// <typeparam name="T">The serialized record type.</typeparam>
    /// <param name="path">The fixture destination.</param>
    /// <param name="value">The fixture record.</param>
    private static void WriteXml<T>(string path, T value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using var stream = File.Create(path);
        new XmlSerializer(typeof(T)).Serialize(stream, value);
    }

    /// <summary>
    /// Removes the isolated temporary fixture directory.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, true);
        }
    }
}
