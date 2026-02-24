// -----------------------------------------------------------------------
// <copyright file="DecimalExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Numeric;
using Xunit;

namespace Oxtensions.Tests.Numeric;

public sealed class DecimalExtensionsTests
{
    // ── RoundTo ───────────────────────────────────────────────────────────────

    [Fact]
    public void RoundTo_HalfValue_RoundsAwayFromZero()
        => 2.345m.RoundTo(2).Should().Be(2.35m);

    [Fact]
    public void RoundTo_Zero_ReturnsZero()
        => 0m.RoundTo(2).Should().Be(0m);

    [Theory]
    [InlineData(1.005, 2, 1.01)]
    [InlineData(1.555, 2, 1.56)]
    [InlineData(-2.345, 2, -2.35)]
    public void RoundTo_VariousValues_ReturnsExpected(double input, int decimals, double expected)
        => ((decimal)input).RoundTo(decimals).Should().Be((decimal)expected);

    // ── Percentage ────────────────────────────────────────────────────────────

    [Fact]
    public void Percentage_ValidValues_ReturnsPercentage()
        => 25m.Percentage(200m).Should().Be(12.5m);

    [Fact]
    public void Percentage_ZeroTotal_ReturnsZero()
        => 10m.Percentage(0m).Should().Be(0m);

    [Fact]
    public void Percentage_HundredPercent_Returns100()
        => 100m.Percentage(100m).Should().Be(100m);

    // ── Sign checks ───────────────────────────────────────────────────────────

    [Fact]
    public void IsPositive_PositiveValue_ReturnsTrue()
        => 5m.IsPositive().Should().BeTrue();

    [Fact]
    public void IsPositive_NegativeValue_ReturnsFalse()
        => (-5m).IsPositive().Should().BeFalse();

    [Fact]
    public void IsNegative_NegativeValue_ReturnsTrue()
        => (-3m).IsNegative().Should().BeTrue();

    [Fact]
    public void IsZero_Zero_ReturnsTrue()
        => 0m.IsZero().Should().BeTrue();

    [Fact]
    public void IsZero_NonZero_ReturnsFalse()
        => 1m.IsZero().Should().BeFalse();

    // ── Clamp ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Clamp_AboveMax_ReturnsMax()
        => 150m.Clamp(0m, 100m).Should().Be(100m);

    [Fact]
    public void Clamp_BelowMin_ReturnsMin()
        => (-10m).Clamp(0m, 100m).Should().Be(0m);

    [Fact]
    public void Clamp_WithinRange_ReturnsOriginal()
        => 50m.Clamp(0m, 100m).Should().Be(50m);

    [Fact]
    public void Clamp_MinGreaterThanMax_ThrowsArgumentException()
    {
        var act = () => 5m.Clamp(10m, 5m);
        act.Should().Throw<ArgumentException>();
    }

    // ── ToCurrencyString ──────────────────────────────────────────────────────

    [Fact]
    public void ToCurrencyString_EnUsCulture_FormatsCorrectly()
        => 1234.56m.ToCurrencyString("en-US").Should().Contain("1,234");

    [Fact]
    public void ToCurrencyString_EnUsCulture_StartWithDollar()
        => 99.99m.ToCurrencyString("en-US").Should().Contain("99.99");

    [Fact]
    public void ToCurrencyString_DefaultCulture_DoesNotThrow()
    {
        var act = () => 1234.56m.ToCurrencyString();
        act.Should().NotThrow();
    }

    // ── Abs ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Abs_Negative_ReturnsPositive()
        => (-42.5m).Abs().Should().Be(42.5m);

    [Fact]
    public void Abs_Positive_ReturnsSameValue()
        => 10m.Abs().Should().Be(10m);

    [Fact]
    public void Abs_Zero_ReturnsZero()
        => 0m.Abs().Should().Be(0m);

    // ── IsBetween ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(5, 1, 10, true)]
    [InlineData(0, 1, 10, false)]
    [InlineData(10, 1, 10, true)]
    [InlineData(1, 1, 10, true)]
    public void IsBetween_VariousValues_ReturnsExpected(int v, int min, int max, bool expected)
        => ((decimal)v).IsBetween((decimal)min, (decimal)max).Should().Be(expected);

    // ── ToNearest ─────────────────────────────────────────────────────────

    [Fact]
    public void ToNearest_Quarter_RoundsToNearestQuarter()
        => 1.37m.ToNearest(0.25m).Should().Be(1.25m);

    [Fact]
    public void ToNearest_Quarter_RoundsUp()
        => 1.63m.ToNearest(0.25m).Should().Be(1.75m);

    [Fact]
    public void ToNearest_ZeroStep_ThrowsArgumentException()
    {
        var act = () => 1m.ToNearest(0m);
        act.Should().Throw<ArgumentException>();
    }
}
