using System.Reflection;
using ViLa.Model;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies measurement storage resolution for mounted and legacy directories.
/// </summary>
public class MeasurementDirectoryTest
{
    /// <summary>
    /// Verifies that an empty data mount receives a measurement directory without losing legacy storage.
    /// </summary>
    /// <param name="hasDataDirectory">Whether the application has a mounted data directory.</param>
    /// <param name="hasLegacyDirectory">Whether legacy measurement storage already exists.</param>
    /// <param name="hasCurrentDirectory">Whether current measurement storage already exists.</param>
    /// <param name="usesCurrentDirectory">Whether resolution should select current measurement storage.</param>
    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, true, true, true)]
    [InlineData(false, false, false, false)]
    public void ResolveMeasurementsDirectory_SelectsStorage(
        bool hasDataDirectory, bool hasLegacyDirectory, bool hasCurrentDirectory, bool usesCurrentDirectory)
    {
        // arrange
        var root = Path.Combine(Path.GetTempPath(), "vila-measurements-" + Guid.NewGuid());
        var current = Path.Combine(root, "data", "measurements");
        var legacy = Path.Combine(root, "measurements");
        Directory.CreateDirectory(root);
        try
        {
            if (hasDataDirectory)
            {
                Directory.CreateDirectory(Path.Combine(root, "data"));
            }
            if (hasLegacyDirectory)
            {
                Directory.CreateDirectory(legacy);
                File.WriteAllText(Path.Combine(legacy, "existing.xml"), "existing measurement");
            }
            if (hasCurrentDirectory)
            {
                Directory.CreateDirectory(current);
            }
            var method = typeof(ViewModel).GetMethod("ResolveMeasurementsDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

            // act
            var result = (string)method.Invoke(null, new object[] { root })!;

            // validation
            Assert.Equal(usesCurrentDirectory ? current : legacy, result);
            Assert.True(Directory.Exists(result));
            if (hasLegacyDirectory)
            {
                Assert.Equal("existing measurement", File.ReadAllText(Path.Combine(legacy, "existing.xml")));
            }
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
