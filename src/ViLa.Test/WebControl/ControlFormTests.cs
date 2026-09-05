using ViLa.Model;
using ViLa.WebControl;
using Xunit;

namespace ViLa.Test.WebControl;

/// <summary>
/// Unit tests for web forms and visual controls.
/// </summary>
public class ControlFormTests
{
    /// <summary>
    /// Tests the initialization of the general settings form controls.
    /// </summary>
    [Fact]
    public void ControlFormSetting_InitializesControls()
    {
        // Act
        var form = new ControlFormSetting("custom_settings");

        // Assert
        Assert.Equal("custom_settings", form.Id);
        Assert.NotNull(form.ImpulsePerkWhCtrl);
        Assert.NotNull(form.ElectricityPricePerkWhCtrl);
        Assert.NotNull(form.MaxWattageCtrl);
        Assert.NotNull(form.MinWattageCtrl);
        Assert.NotNull(form.MaxChargingTimeCtrl);
        Assert.NotNull(form.CurrencyCtrl);
        Assert.NotNull(form.BillingDayOffsetCtrl);
    }

    /// <summary>
    /// Tests the initialization of the operating mode form controls.
    /// </summary>
    [Fact]
    public void ControlFormMode_InitializesControls()
    {
        // Act
        var form = new ControlFormMode();

        // Assert
        Assert.Equal("settings", form.Id);
        Assert.NotNull(form.Mode);
    }

    /// <summary>
    /// Tests the initialization of the comment form controls.
    /// </summary>
    [Fact]
    public void ControlFormComment_InitializesControls()
    {
        // Act
        var form = new ControlFormComment("comment_form");

        // Assert
        Assert.Equal("comment_form", form.Id);
        Assert.NotNull(form.Comment);
    }

    /// <summary>
    /// Tests the initialization of the tag form controls.
    /// </summary>
    [Fact]
    public void ControlFormTag_InitializesControls()
    {
        // Act
        var form = new ControlFormTag("tag_form");

        // Assert
        Assert.Equal("tag_form", form.Id);
        Assert.NotNull(form.Tag);
    }

    /// <summary>
    /// Tests the initialization of the time control form controls.
    /// </summary>
    [Fact]
    public void ControlFormTimeControl_InitializesControls()
    {
        // Act
        var form = new ControlFormTimeControl();

        // Assert
        Assert.Equal("settings", form.Id);
        Assert.NotNull(form.Monday);
    }

    /// <summary>
    /// Tests the instantiation of the charging card control.
    /// </summary>
    [Fact]
    public void ControlCardCharging_InstantiatesSuccessfully()
    {
        // Act
        var card = new ControlCardCharging("card_charging");

        // Assert
        Assert.Equal("card_charging", card.Id);
    }

    /// <summary>
    /// Tests the instantiation of the measurement log card control.
    /// </summary>
    [Fact]
    public void ControlCardMeasurementLog_InstantiatesSuccessfully()
    {
        // Arrange
        var log = new MeasurementLog { ID = "log-123" };

        // Act
        var card = new ControlCardMeasurementLog("card_ml")
        {
            MeasurementLog = log
        };

        // Assert
        Assert.Equal("card_ml", card.Id);
        Assert.Equal(log, card.MeasurementLog);
        Assert.NotNull(card.Icon);
    }
}
