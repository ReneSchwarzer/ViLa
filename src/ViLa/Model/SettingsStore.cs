using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using WebExpress.WebCore.WebSetting;

namespace ViLa.Model
{
    /// <summary>
    /// Persists ViLa settings in the WebExpress plugin configuration format.
    /// </summary>
    public sealed class SettingsStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter() }
        };
        private readonly string _root;
        private readonly string _directory;
        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Initializes storage for an application installation.
        /// </summary>
        /// <param name="root">The application base directory.</param>
        /// <param name="directory">The settings directory selected by the WebExpress host.</param>
        /// <param name="configuration">The host configuration, including command line and environment overrides.</param>
        public SettingsStore(string root, string directory = null, IConfigurationRoot configuration = null)
        {
            _root = Path.GetFullPath(root);
            _directory = Path.GetFullPath(directory ?? Path.Combine(_root, "settings"));
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the writable plugin settings file.
        /// </summary>
        public string FilePath => Path.Combine(_directory, "vila.settings.json");

        /// <summary>
        /// Gets the existing legacy XML configuration, if present.
        /// </summary>
        public string LegacyPath => File.Exists(Path.Combine(_root, "Config", "vila.settings.xml"))
            ? Path.Combine(_root, "Config", "vila.settings.xml") : Path.Combine(_root, "vila.settings.xml");

        /// <summary>
        /// Gets whether persisted settings already exist in either supported format.
        /// </summary>
        public bool Exists => File.Exists(FilePath) || File.Exists(LegacyPath);

        /// <summary>
        /// Loads merged WebExpress configuration after migrating legacy settings when necessary.
        /// </summary>
        /// <returns>The effective ViLa settings.</returns>
        public Settings Load()
        {
            if (!File.Exists(FilePath) && File.Exists(LegacyPath))
            {
                Save(DeserializeLegacy(File.ReadAllBytes(LegacyPath)), false);
            }
            if (_configuration != null)
            {
                _configuration.Reload();
                return _configuration.GetPluginSettings("vila").Get<Settings>() ?? new Settings();
            }
            var configuration = new ConfigurationBuilder()
                .AddSettingsDirectory(Path.GetDirectoryName(FilePath), reloadOnChange: false)
                .AddEnvironmentVariables(SettingsLoader.EnvironmentVariablePrefix)
                .Build();
            using (configuration as IDisposable)
            {
                return configuration.GetPluginSettings("vila").Get<Settings>() ?? new Settings();
            }
        }

        /// <summary>
        /// Atomically writes the plugin configuration without exposing incomplete JSON.
        /// </summary>
        /// <param name="settings">The settings to persist.</param>
        /// <param name="overwrite">Whether an existing JSON file may be replaced.</param>
        public void Save(Settings settings, bool overwrite = true)
        {
            var bytes = Serialize(settings);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            var temporary = FilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllBytes(temporary, bytes);
                File.Move(temporary, FilePath, overwrite);
            }
            finally
            {
                if (File.Exists(temporary))
                {
                    File.Delete(temporary);
                }
            }
        }

        /// <summary>
        /// Serializes settings in the shared plugin namespace with named enum values.
        /// </summary>
        /// <param name="settings">The settings to serialize.</param>
        /// <returns>The UTF-8 JSON document.</returns>
        public static byte[] Serialize(Settings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            return JsonSerializer.SerializeToUtf8Bytes(new
            {
                Plugins = new Dictionary<string, Settings> { ["vila"] = settings }
            }, JsonOptions);
        }

        /// <summary>
        /// Validates and reads a ViLa JSON settings document.
        /// </summary>
        /// <param name="content">The UTF-8 JSON document.</param>
        /// <returns>The settings from the required plugin section.</returns>
        public static Settings Deserialize(byte[] content)
        {
            using var document = JsonDocument.Parse(content, new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true
            });
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || !document.RootElement.TryGetProperty("Plugins", out var plugins)
                || plugins.ValueKind != JsonValueKind.Object
                || !plugins.TryGetProperty("vila", out var section) || section.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidDataException("The ViLa plugin settings section is missing.");
            }
            return section.Deserialize<Settings>(JsonOptions)
                ?? throw new InvalidDataException("The settings document is empty.");
        }

        /// <summary>
        /// Reads legacy XML without resolving external resources.
        /// </summary>
        /// <param name="content">The legacy XML document.</param>
        /// <returns>The settings to migrate to JSON.</returns>
        public static Settings DeserializeLegacy(byte[] content)
        {
            using var stream = new MemoryStream(content);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null
            });
            return new XmlSerializer(typeof(Settings)).Deserialize(reader) as Settings
                ?? throw new InvalidDataException("The settings document is empty.");
        }
    }
}
