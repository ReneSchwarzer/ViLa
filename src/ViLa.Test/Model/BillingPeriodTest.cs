using ViLa.Model;

namespace ViLa.Test.Model;

/// <summary>
/// Verifies billing boundaries and exact tag matching.
/// </summary>
public class BillingPeriodTest
{
    /// <summary>
    /// Keeps stored costs in separate currency groups and supports legacy records.
    /// </summary>
    [Fact]
    public void FormatCosts_PreservesStoredCurrencyAndTotals()
    {
        // arrange
        var logs = new[]
        {
            new MeasurementLog { FinalCost = 2.5f, Currency = "EUR" },
            new MeasurementLog { FinalCost = 3f, Currency = "USD" },
            new MeasurementLog { FinalCost = 1f, Currency = "" }
        };

        // act
        var result = BillingPeriod.FormatCosts(logs, System.Globalization.CultureInfo.InvariantCulture, "EUR");

        // validation
        Assert.Equal("3.50 EUR / 3.00 USD", result);
    }

    /// <summary>
    /// Includes January and its final fractional second without counting February twice.
    /// </summary>
    [Fact]
    public void GetLogs_IncludesJanuaryWithExclusiveEnd()
    {
        // arrange
        var start = new DateTime(2026, 1, 1);
        var end = start.AddMonths(1);
        var logs = new[]
        {
            new MeasurementLog { ID = "start", FinalTill = start },
            new MeasurementLog { ID = "last", FinalTill = end.AddTicks(-1) },
            new MeasurementLog { ID = "next", FinalTill = end }
        };

        // act
        var january = BillingPeriod.GetLogs(logs, start, 0);
        var february = BillingPeriod.GetLogs(logs, end, 0);

        // validation
        Assert.Equal(new[] { "last", "start" }, january.Select(x => x.ID));
        Assert.Equal("next", Assert.Single(february).ID);
    }

    /// <summary>
    /// Assigns a December completion to an offset January billing period.
    /// </summary>
    [Fact]
    public void GetLogs_HandlesOffsetAcrossYearBoundary()
    {
        // arrange
        var log = new MeasurementLog { FinalTill = new DateTime(2025, 12, 29) };

        // act
        var result = BillingPeriod.GetLogs(new[] { log }, new DateTime(2026, 1, 1), 5);

        // validation
        Assert.Same(log, Assert.Single(result));
    }

    /// <summary>
    /// Separates semicolon tags without matching name prefixes.
    /// </summary>
    [Fact]
    public void GetTags_MatchesWholeDistinctTokens()
    {
        // arrange
        var log = new MeasurementLog { Tag = "Auto2; Home Home" };

        // act
        var tags = BillingPeriod.GetTags(log).ToArray();

        // validation
        Assert.Equal(new[] { "Auto2", "Home" }, tags);
        Assert.DoesNotContain("Auto", tags);
    }
}
