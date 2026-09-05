using ViLa.Model;
using ViLa.WebCondition;
using Xunit;

namespace ViLa.Test.WebCondition;

/// <summary>
/// Unit tests for the <see cref="ConditionTimeControl"/> condition class.
/// </summary>
public class ConditionTimeControlTest
{
    /// <summary>
    /// Tests that the condition is fulfilled when the operating mode is set to time controlled.
    /// </summary>
    [Fact]
    public void Fulfillment_WhenModeIsTimeControlled_ReturnsTrue()
    {
        // Arrange
        var condition = new ConditionTimeControl();
        var previousMode = ViewModel.Instance.Settings.Mode;

        try
        {
            ViewModel.Instance.Settings.Mode = Mode.TimeControlled;

            // Act
            var result = condition.Fulfillment(null!);

            // Assert
            Assert.True(result);
        }
        finally
        {
            ViewModel.Instance.Settings.Mode = previousMode;
        }
    }

    /// <summary>
    /// Tests that the condition is not fulfilled when the operating mode is not time controlled.
    /// </summary>
    [Theory]
    [InlineData(Mode.ManuallyControlled)]
    [InlineData(Mode.AutomaticControlled)]
    public void Fulfillment_WhenModeIsNotTimeControlled_ReturnsFalse(Mode mode)
    {
        // Arrange
        var condition = new ConditionTimeControl();
        var previousMode = ViewModel.Instance.Settings.Mode;

        try
        {
            ViewModel.Instance.Settings.Mode = mode;

            // Act
            var result = condition.Fulfillment(null!);

            // Assert
            Assert.False(result);
        }
        finally
        {
            ViewModel.Instance.Settings.Mode = previousMode;
        }
    }
}
