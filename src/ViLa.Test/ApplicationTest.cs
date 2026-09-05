using ViLa;
using Xunit;

namespace ViLa.Test;

/// <summary>
/// Unit tests for the <see cref="Application"/> class.
/// </summary>
public class ApplicationTest
{
    /// <summary>
    /// Tests that the application lifecycle methods execute without exception.
    /// </summary>
    [Fact]
    public void Application_LifecycleMethods_CanBeCalledWithoutException()
    {
        // Arrange
        var app = new Application();

        // Act
        app.Run();
        app.Dispose();

        // Assert
        Assert.NotNull(app);
    }
}
