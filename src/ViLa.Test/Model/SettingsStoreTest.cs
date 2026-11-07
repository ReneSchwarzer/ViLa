using System.Text;
using ViLa.Model;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies JSON persistence, framework precedence and legacy migration.
/// </summary>
public sealed class SettingsStoreTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "vila-settings-" + Guid.NewGuid().ToString("N"));

    /// <summary>
    /// Migrates XML from both supported legacy locations and leaves the source intact.
    /// </summary>
    /// <param name="directory">The legacy settings subdirectory.</param>
    [Theory]
    [InlineData("Config")]
    [InlineData("")]
    public void Load_MigratesLegacyXml(string directory)
    {
        // arrange
        var path = Path.Combine(_root, directory, "vila.settings.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var xml = "<settings><debug>true</debug><ImpulsePerkWh>987</ImpulsePerkWh><Currency>EUR</Currency><Mode>TimeControlled</Mode></settings>";
        File.WriteAllText(path, xml);
        var store = new SettingsStore(_root);

        // act
        var settings = store.Load();
        var persisted = SettingsStore.Deserialize(File.ReadAllBytes(store.FilePath));

        // validation
        Assert.Equal(987, settings.ImpulsePerkWh);
        Assert.True(settings.DebugMode);
        Assert.Equal(Mode.TimeControlled, settings.Mode);
        Assert.Equal(settings.ImpulsePerkWh, persisted.ImpulsePerkWh);
        Assert.Equal("EUR", persisted.Currency);
        Assert.Equal(xml, File.ReadAllText(path));
        Assert.Contains("\"TimeControlled\"", File.ReadAllText(store.FilePath));
    }

    /// <summary>
    /// Keeps existing JSON authoritative even when an older XML file remains.
    /// </summary>
    [Fact]
    public void Load_PrefersJsonAndUsesWebExpressOverrides()
    {
        // arrange
        var store = new SettingsStore(_root);
        store.Save(new Settings { Currency = "EUR", ImpulsePerkWh = 500 });
        File.WriteAllText(Path.Combine(_root, "vila.settings.xml"), "invalid legacy XML");
        File.WriteAllText(Path.Combine(_root, "settings", "webexpress.settings.json"),
            """{ "Plugins": { "vila": { "ImpulsePerkWh": 1000 } } }""");
        var original = File.ReadAllBytes(store.FilePath);

        // act
        var settings = store.Load();

        // validation
        Assert.Equal(1000, settings.ImpulsePerkWh);
        Assert.Equal("EUR", settings.Currency);
        Assert.Equal(original, File.ReadAllBytes(store.FilePath));
    }

    /// <summary>
    /// Applies the same environment variable prefix and precedence as WebExpress.
    /// </summary>
    [Fact]
    public void Load_AppliesEnvironmentOverride()
    {
        // arrange
        const string variable = "WEBEXPRESS_Plugins__vila__Currency";
        var original = Environment.GetEnvironmentVariable(variable);
        var store = new SettingsStore(_root);
        store.Save(new Settings { Currency = "EUR" });
        try
        {
            Environment.SetEnvironmentVariable(variable, "USD");

            // act
            var settings = store.Load();

            // validation
            Assert.Equal("USD", settings.Currency);
            Assert.Equal("EUR", SettingsStore.Deserialize(File.ReadAllBytes(store.FilePath)).Currency);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, original);
        }
    }

    /// <summary>
    /// Rejects malformed JSON rather than reverting to legacy settings or rewriting files.
    /// </summary>
    [Fact]
    public void Load_InvalidJsonPreservesFiles()
    {
        // arrange
        var store = new SettingsStore(_root);
        Directory.CreateDirectory(Path.GetDirectoryName(store.FilePath)!);
        File.WriteAllText(store.FilePath, "invalid JSON");
        File.WriteAllText(store.LegacyPath, "<settings><Currency>EUR</Currency></settings>");

        // act
        var error = Record.Exception(() => store.Load());

        // validation
        Assert.NotNull(error);
        Assert.Equal("invalid JSON", File.ReadAllText(store.FilePath));
    }

    /// <summary>
    /// Parses the comments, trailing commas and named enums accepted by framework settings.
    /// </summary>
    [Fact]
    public void Deserialize_AcceptsFrameworkJsonSyntax()
    {
        // arrange
        var json = Encoding.UTF8.GetBytes("""
            { "Plugins": { "vila": { // operating mode
              "Mode": "AutomaticControlled", "Currency": "EUR",
            } } }
            """);

        // act
        var settings = SettingsStore.Deserialize(json);

        // validation
        Assert.Equal(Mode.AutomaticControlled, settings.Mode);
        Assert.Equal("EUR", settings.Currency);
    }

    /// <summary>
    /// Removes the isolated test directory.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, true);
        }
    }
}
