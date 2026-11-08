using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace ViLa.Model
{
    /// <summary>
    /// Transfers persisted application data in a versioned ZIP archive without replacing existing records.
    /// </summary>
    public sealed class DataBackup
    {
        /// <summary>
        /// Coordinates backup snapshots and measurement file mutations within the application.
        /// </summary>
        internal static object StorageLock { get; } = new object();

        private const long MaximumBytes = 256L * 1024 * 1024;
        private readonly string _root;
        private readonly VehicleStore _vehicles;
        private readonly SettingsStore _settings;

        /// <summary>
        /// Creates a backup service for an application data directory.
        /// </summary>
        /// <param name="root">The application base directory.</param>
        /// <param name="vehicles">The live vehicle catalog.</param>
        /// <param name="settings">The settings storage selected by the host.</param>
        public DataBackup(string root, VehicleStore vehicles, SettingsStore settings = null)
        {
            _root = Path.GetFullPath(root);
            _vehicles = vehicles;
            _settings = settings ?? new SettingsStore(_root);
        }

        /// <summary>
        /// Gets the local JSON settings path.
        /// </summary>
        public string SettingsPath => _settings.FilePath;

        /// <summary>
        /// Resolves the active storage directory for a category.
        /// </summary>
        /// <param name="category">The measurement or archive category.</param>
        /// <returns>The current or legacy directory.</returns>
        private string DirectoryFor(string category)
        {
            return StorageDirectory.Resolve(_root, category);
        }

        /// <summary>
        /// Creates a backup of settings, vehicles, measurements and archived measurements.
        /// </summary>
        /// <param name="settings">The current settings, including defaults that have not been saved.</param>
        /// <param name="includeArchive">Whether archived measurements are included.</param>
        /// <returns>The ZIP download.</returns>
        public byte[] Export(Settings settings, bool includeArchive = false)
        {
            lock (StorageLock)
            {
                using var output = new MemoryStream();
                using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
                {
                    Write(zip, "version.txt", Encoding.UTF8.GetBytes("ViLa/2"));
                    Write(zip, "settings.json", SettingsStore.Serialize(settings));
                    Write(zip, "vehicles.json", JsonSerializer.SerializeToUtf8Bytes(_vehicles.GetAll()));
                    foreach (var category in includeArchive ? new[] { "measurements", "archive" } : new[] { "measurements" })
                    {
                        var directory = DirectoryFor(category);
                        if (!Directory.Exists(directory))
                        {
                            continue;
                        }
                        foreach (var file in Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
                        {
                            Write(zip, category + "/" + Path.GetRelativePath(directory, file).Replace('\\', '/'), File.ReadAllBytes(file));
                        }
                    }
                }
                return output.ToArray();
            }
        }

        /// <summary>
        /// Validates the entire archive and adds only records whose identities are absent locally.
        /// </summary>
        /// <param name="content">The uploaded ZIP bytes.</param>
        /// <param name="reservedIds">The live measurement identities that must remain untouched.</param>
        /// <param name="publish">Receives committed records even when a subsequent storage operation fails.</param>
        /// <returns>The added records and counts used to refresh the live model.</returns>
        public BackupImportResult Import(byte[] content, IEnumerable<string> reservedIds = null, Action<BackupImportResult> publish = null)
        {
            lock (StorageLock)
            {
                if (content == null || content.Length == 0 || content.LongLength > MaximumBytes)
                {
                    throw new InvalidDataException("The backup is empty or exceeds 256 MiB.");
                }
                using var input = new MemoryStream(content);
                using var zip = new ZipArchive(input, ZipArchiveMode.Read);
                if (zip.Entries.Count > 100000 || zip.Entries.Sum(x => x.Length) > MaximumBytes)
                {
                    throw new InvalidDataException("The expanded backup exceeds the import limits.");
                }
                var entries = new Dictionary<string, byte[]>(StringComparer.Ordinal);
                long expandedBytes = 0;
                foreach (var entry in zip.Entries)
                {
                    // zip tools may include empty directory entries alongside the records
                    if (entry.Length == 0 && Regex.IsMatch(entry.FullName,
                        @"\A(?:measurements/|archive/(?:[0-9]{4}/(?:[0-9]{2}/)?)?)\z"))
                    {
                        continue;
                    }
                    using var stream = entry.Open();
                    using var buffer = new MemoryStream();
                    var chunk = new byte[81920];
                    int read;
                    while ((read = stream.Read(chunk, 0, chunk.Length)) > 0)
                    {
                        expandedBytes += read;
                        if (expandedBytes > MaximumBytes)
                        {
                            throw new InvalidDataException("The expanded backup exceeds 256 MiB.");
                        }
                        buffer.Write(chunk, 0, read);
                    }
                    if (!entries.TryAdd(entry.FullName, buffer.ToArray()))
                    {
                        throw new InvalidDataException("The backup contains duplicate paths.");
                    }
                }
                var format = entries.TryGetValue("version.txt", out var version) ? Encoding.UTF8.GetString(version) : null;
                var settingsEntry = format == "ViLa/1" ? "settings.xml" : "settings.json";
                if ((format != "ViLa/1" && format != "ViLa/2")
                    || !entries.ContainsKey(settingsEntry) || !entries.ContainsKey("vehicles.json"))
                {
                    throw new InvalidDataException("The backup format is unsupported.");
                }
                var settings = format == "ViLa/1" ? SettingsStore.DeserializeLegacy(entries[settingsEntry])
                    : SettingsStore.Deserialize(entries[settingsEntry]);
                var vehicles = JsonSerializer.Deserialize<Vehicle[]>(entries["vehicles.json"]);
                VehicleStore.ValidateImport(vehicles);
                var measurements = new List<(string Path, byte[] Content, MeasurementLog Log)>();
                var incomingIds = new HashSet<Guid>();
                foreach (var entry in entries.Where(x => x.Key != "version.txt" && x.Key != settingsEntry && x.Key != "vehicles.json"))
                {
                    if (!Regex.IsMatch(entry.Key, @"\A(?:measurements/|archive/(?:[0-9]{4}/[0-9]{2}/)?)[0-9a-fA-F-]{36}\.xml\z"))
                    {
                        throw new InvalidDataException("The backup contains an unsupported path.");
                    }
                    var log = ReadXml<MeasurementLog>(entry.Value);
                    if (!Guid.TryParse(log.ID, out var id) || !Guid.TryParse(Path.GetFileNameWithoutExtension(entry.Key), out var filenameId)
                        || id != filenameId || !incomingIds.Add(id) || log.Measurements == null || log.Comments == null)
                    {
                        throw new InvalidDataException("The backup contains invalid or duplicate measurements.");
                    }
                    measurements.Add((entry.Key, entry.Value, log));
                }

                var existingIds = new HashSet<Guid>((reservedIds ?? []).Select(Guid.Parse));
                foreach (var category in new[] { "measurements", "archive" })
                {
                    var directory = DirectoryFor(category);
                    if (Directory.Exists(directory))
                    {
                        foreach (var file in Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
                        {
                            var log = ReadXml<MeasurementLog>(File.ReadAllBytes(file));
                            existingIds.Add(Guid.Parse(log.ID));
                        }
                    }
                }
                var result = new BackupImportResult { VehiclesAdded = _vehicles.ImportMissing(vehicles) };
                try
                {
                    result.SettingsAdded = !_settings.Exists
                        && WriteMissing(SettingsPath, SettingsStore.Serialize(settings));
                    foreach (var measurement in measurements)
                    {
                        if (!existingIds.Add(Guid.Parse(measurement.Log.ID)))
                        {
                            result.Skipped++;
                            continue;
                        }
                        var parts = measurement.Path.Split('/', 2);
                        var destination = Path.Combine(DirectoryFor(parts[0]), parts[1]);
                        if (WriteMissing(destination, measurement.Content))
                        {
                            result.MeasurementsAdded++;
                            if (parts[0] == "measurements")
                            {
                                result.Measurements.Add(measurement.Log);
                            }
                        }
                        else
                        {
                            result.Skipped++;
                        }
                    }
                    result.Skipped += vehicles.Length - result.VehiclesAdded + (result.SettingsAdded ? 0 : 1);
                }
                finally
                {
                    publish?.Invoke(result);
                }
                return result;
            }
        }

        /// <summary>
        /// Parses XML with external resources and document type declarations disabled.
        /// </summary>
        /// <typeparam name="T">The persisted record type.</typeparam>
        /// <param name="content">The serialized XML bytes.</param>
        /// <returns>The deserialized record.</returns>
        private static T ReadXml<T>(byte[] content)
        {
            using var stream = new MemoryStream(content);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
            var value = new XmlSerializer(typeof(T)).Deserialize(reader);
            return value is T record ? record : throw new InvalidDataException("The backup contains an empty XML record.");
        }

        /// <summary>
        /// Writes an archive entry with compression.
        /// </summary>
        /// <param name="zip">The destination archive.</param>
        /// <param name="name">The portable entry name.</param>
        /// <param name="content">The serialized record bytes.</param>
        private static void Write(ZipArchive zip, string name, byte[] content)
        {
            using var stream = zip.CreateEntry(name, CompressionLevel.Optimal).Open();
            stream.Write(content);
        }

        /// <summary>
        /// Publishes a complete file without overwriting an existing destination.
        /// </summary>
        /// <param name="path">The destination path.</param>
        /// <param name="content">The file content.</param>
        /// <returns>Whether a new file was added.</returns>
        private static bool WriteMissing(string path, byte[] content)
        {
            if (File.Exists(path))
            {
                return false;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllBytes(temporary, content);
                try
                {
                    File.Move(temporary, path, false);
                    return true;
                }
                catch (IOException) when (File.Exists(path))
                {
                    return false;
                }
            }
            finally
            {
                if (File.Exists(temporary))
                {
                    File.Delete(temporary);
                }
            }
        }
    }

    /// <summary>
    /// Describes the records added by an additive import.
    /// </summary>
    public sealed class BackupImportResult
    {
        /// <summary>
        /// Gets or sets the number of added vehicle profiles.
        /// </summary>
        public int VehiclesAdded { get; set; }
        /// <summary>
        /// Gets or sets the number of added current and archived measurements.
        /// </summary>
        public int MeasurementsAdded { get; set; }
        /// <summary>
        /// Gets or sets the number of preserved existing records.
        /// </summary>
        public int Skipped { get; set; }
        /// <summary>
        /// Gets or sets whether a missing settings file was restored.
        /// </summary>
        public bool SettingsAdded { get; set; }
        /// <summary>
        /// Gets the newly imported history records to publish in the live model.
        /// </summary>
        public List<MeasurementLog> Measurements { get; } = new List<MeasurementLog>();
    }
}
