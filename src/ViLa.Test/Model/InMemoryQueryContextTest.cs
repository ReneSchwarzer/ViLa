using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="InMemoryQueryContext"/> class.
/// </summary>
public class InMemoryQueryContextTest
{
    /// <summary>
    /// Tests that Dispose can be called without throwing exceptions.
    /// </summary>
    [Fact]
    public void Dispose_CanBeCalledWithoutException()
    {
        // Arrange
        using var context = new InMemoryQueryContext();

        // Act & Assert
        context.Dispose();
    }
}
