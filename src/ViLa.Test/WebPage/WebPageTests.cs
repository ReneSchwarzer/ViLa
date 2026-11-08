using Xunit;

namespace ViLa.Test.WebPage;

/// <summary>
/// Unit tests for web pages in the application.
/// </summary>
public class WebPageTests
{
    /// <summary>
    /// Tests instantiation of the main dashboard page.
    /// </summary>
    [Fact]
    public void Page_WWW_Index_Instantiates()
    {
        var page = new ViLa.WWW.Index();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the help page.
    /// </summary>
    [Fact]
    public void Page_WWW_Help_Index_Instantiates()
    {
        var page = new ViLa.WWW.Help.Index();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the history page.
    /// </summary>
    [Fact]
    public void Page_WWW_History_Index_Instantiates()
    {
        var page = new ViLa.WWW.History.Index();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the log page.
    /// </summary>
    [Fact]
    public void Page_WWW_Log_Index_Instantiates()
    {
        var page = new ViLa.WWW.Log.Index();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the details page.
    /// </summary>
    [Fact]
    public void Page_WWW_Details_Index_Instantiates()
    {
        var page = new ViLa.WWW.Details.Index();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the general settings page.
    /// </summary>
    [Fact]
    public void Page_WWW_Settings_General_Instantiates()
    {
        var page = new ViLa.WWW.Settings.General();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the mode settings page.
    /// </summary>
    [Fact]
    public void Page_WWW_Settings_Mode_Instantiates()
    {
        var page = new ViLa.WWW.Settings.Mode();
        Assert.NotNull(page);
    }

    /// <summary>
    /// Tests instantiation of the time control settings page.
    /// </summary>
    [Fact]
    public void Page_WWW_Settings_TimeControl_Instantiates()
    {
        var page = new ViLa.WWW.Settings.TimeControl();
        Assert.NotNull(page);
    }
}
