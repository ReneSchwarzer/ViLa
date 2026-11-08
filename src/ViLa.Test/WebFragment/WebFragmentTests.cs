using ViLa.WebFragment;
using Xunit;

namespace ViLa.Test.WebFragment;

/// <summary>
/// Unit tests for web UI fragments.
/// </summary>
public class WebFragmentTests
{
    /// <summary>
    /// Tests instantiation and text rendering of the version footer fragment.
    /// </summary>
    [Fact]
    public void FragmentFooterVersion_InstantiatesAndHasText()
    {
        // Act
        var fragment = new FragmentFooterVersion(null!);

        // Assert
        Assert.NotNull(fragment);
        Assert.NotNull(fragment.Text);
        var text = fragment.Text(null!);
        Assert.Contains("Vila", text);
    }

    /// <summary>
    /// Tests instantiation of the licence footer fragment.
    /// </summary>
    [Fact]
    public void FragmentFooterLicence_Instantiates()
    {
        // Act
        var fragment = new FragmentFooterLicence(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the help content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentHelp_Instantiates()
    {
        // Act
        var fragment = new FragmentContentHelp(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the primary dashboard content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentDashboardPrimary_Instantiates()
    {
        // Act
        var fragment = new FragmentContentDashboardPrimary(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the secondary dashboard content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentDashboardSecondary_Instantiates()
    {
        // Act
        var fragment = new FragmentContentDashboardSecondary(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the general settings content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentSettingsGeneral_Instantiates()
    {
        // Act
        var fragment = new FragmentContentSettingsGeneral(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the mode settings content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentSettingsMode_Instantiates()
    {
        // Act
        var fragment = new FragmentContentSettingsMode(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the tag content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentTag_Instantiates()
    {
        // Act
        var fragment = new FragmentContentTag(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the comment content fragment.
    /// </summary>
    [Fact]
    public void FragmentContentComment_Instantiates()
    {
        // Act
        var fragment = new FragmentContentComment(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the history view fragment.
    /// </summary>
    [Fact]
    public void FragmentHistoryView_Instantiates()
    {
        // Act
        var fragment = new FragmentHistoryView(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the history search fragment.
    /// </summary>
    [Fact]
    public void FragmentHistorySearchFragment_Instantiates()
    {
        // Act
        var fragment = new FragmentHistorySearchFragment(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the history pagination fragment.
    /// </summary>
    [Fact]
    public void FragmentHistoryPaginationFragment_Instantiates()
    {
        // Act
        var fragment = new FragmentHistoryPaginationFragment(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the history quickfilter fragment.
    /// </summary>
    [Fact]
    public void FragmentHistoryQuickfilterFragment_Instantiates()
    {
        // Act
        var fragment = new FragmentHistoryQuickfilterFragment(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the history table fragment.
    /// </summary>
    [Fact]
    public void FragmentHistoryTableFragment_Instantiates()
    {
        // Act
        var fragment = new FragmentHistoryTableFragment(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the charging property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyCharging_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyCharging(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the cost property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyCost_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyCost(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the current power property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyCurrent_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyCurrent(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the archive property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyArchive_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyArchive(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the delete property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyDelete_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyDelete(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the download property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyDownload_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyDownload(null!);

        // Assert
        Assert.NotNull(fragment);
    }

    /// <summary>
    /// Tests instantiation of the debug property fragment.
    /// </summary>
    [Fact]
    public void FragmentPropertyDebug_Instantiates()
    {
        // Act
        var fragment = new FragmentPropertyDebug(null!);

        // Assert
        Assert.NotNull(fragment);
    }
}
