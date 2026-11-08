using ViLa.WWW.Api._1_;
using WebExpress.WebCore.WebMessage;
using Xunit;

namespace ViLa.Test.WWW;

/// <summary>
/// Unit-Tests für die REST-API-Endpunkte.
/// </summary>
public class ApiTests
{
    /// <summary>
    /// Testet den Ladezustands-API-Endpunkt.
    /// </summary>
    [Fact]
    public void Charging_Process_ReturnsOkResponseWithJson()
    {
        // Arrange
        var endpoint = new Charging();

        // Act
        var response = endpoint.Process(null!);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<ResponseOK>(response);
        Assert.NotNull(response.Content);
        Assert.Contains("application/json", response.Header.ContentType);
    }

    /// <summary>
    /// Testet den Tags-API-Endpunkt.
    /// </summary>
    [Fact]
    public void Tags_Process_ReturnsOkResponseWithJson()
    {
        // Arrange
        var endpoint = new Tags();

        // Act
        var response = endpoint.Process(null!);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<ResponseOK>(response);
        Assert.NotNull(response.Content);
        Assert.Contains("application/json", response.Header.ContentType);
    }
}
