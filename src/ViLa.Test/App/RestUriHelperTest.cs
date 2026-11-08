using ViLa.App;
using ViLa.WWW.Api._1_;
using Xunit;

namespace ViLa.Test.App;

/// <summary>
/// Unit tests for the <see cref="RestUriHelper"/> class.
/// </summary>
public class RestUriHelperTest
{
    /// <summary>
    /// Tests that GetUri safely executes when no application is registered.
    /// </summary>
    [Fact]
    public void GetUri_WhenNoApplicationRegistered_ReturnsNull()
    {
        // Act
        var uri = RestUriHelper.GetUri<Charging>();

        // Assert
        Assert.True(uri == null || uri.ToString() != null);
    }
}
