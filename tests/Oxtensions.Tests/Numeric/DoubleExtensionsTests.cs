// -----------------------------------------------------------------------
// <copyright file="DoubleExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Numeric;
using Xunit;

namespace Oxtensions.Tests.Numeric;

public sealed class DoubleExtensionsTests
{
    // ── Special value checks ──────────────────────────────────────────────────

    [Fact] public void IsNaN_NaN_ReturnsTrue()         => double.NaN.IsNaN().Should().BeTrue();
    [Fact] public void IsNaN_NonNaN_ReturnsFalse()     => 1.0d.IsNaN().Should().BeFalse();

    [Fact] public void IsInfinity_PosInf_ReturnsTrue()  => double.PositiveInfinity.IsInfinity().Should().BeTrue();
    [Fact] public void IsInfinity_NegInf_ReturnsTrue()  => double.NegativeInfinity.IsInfinity().Should().BeTrue();
    [Fact] public void IsInfinity_Finite_ReturnsFalse() => 1.0d.IsInfinity().Should().BeFalse();

    [Fact] public void IsFinite_Finite_ReturnsTrue()  => 1.0d.IsFinite().Should().BeTrue();
    [Fact] public void IsFinite_NaN_ReturnsFalse()    => double.NaN.IsFinite().Should().BeFalse();
    [Fact] public void IsFinite_Inf_ReturnsFalse()    => double.PositiveInfinity.IsFinite().Should().BeFalse();

    // ── Sign checks ───────────────────────────────────────────────────────────

    [Fact] public void IsPositive_Positive_ReturnsTrue()       => 1.5d.IsPositive().Should().BeTrue();
    [Fact] public void IsPositive_Zero_ReturnsFalse()          => 0.0d.IsPositive().Should().BeFalse();
    [Fact] public void IsPositive_Negative_ReturnsFalse()      => (-1.5d).IsPositive().Should().BeFalse();
    [Fact] public void IsPositive_PosInfinity_ReturnsFalse()   => double.PositiveInfinity.IsPositive().Should().BeFalse();

    [Fact] public void IsNegative_Negative_ReturnsTrue()       => (-1.5d).IsNegative().Should().BeTrue();
    [Fact] public void IsNegative_Zero_ReturnsFalse()          => 0.0d.IsNegative().Should().BeFalse();
    [Fact] public void IsNegative_Positive_ReturnsFalse()      => 1.5d.IsNegative().Should().BeFalse();
    [Fact] public void IsNegative_NegInfinity_ReturnsFalse()   => double.NegativeInfinity.IsNegative().Should().BeFalse();

    [Fact] public void IsZero_Zero_ReturnsTrue()     => 0.0d.IsZero().Should().BeTrue();
    [Fact] public void IsZero_NonZero_ReturnsFalse() => 0.001d.IsZero().Should().BeFalse();

    // ── Range ─────────────────────────────────────────────────────────────────

    [Fact] public void Clamp_AboveMax_ReturnsMax()      => 200.0d.Clamp(0.0d, 100.0d).Should().Be(100.0d);
    [Fact] public void Clamp_BelowMin_ReturnsMin()      => (-5.0d).Clamp(0.0d, 100.0d).Should().Be(0.0d);
    [Fact] public void Clamp_InRange_ReturnsSame()      => 50.0d.Clamp(0.0d, 100.0d).Should().Be(50.0d);
    [Fact]
    public void Clamp_MinGreaterThanMax_Throws()
    {
        var act = () => 5.0d.Clamp(10.0d, 1.0d);
        act.Should().Throw<ArgumentException>();
    }

    [Fact] public void IsBetween_InRange_ReturnsTrue()     => 50.0d.IsBetween(1.0d, 100.0d).Should().BeTrue();
    [Fact] public void IsBetween_OnBoundary_ReturnsTrue()  => 1.0d.IsBetween(1.0d, 100.0d).Should().BeTrue();
    [Fact] public void IsBetween_OutOfRange_ReturnsFalse() => 200.0d.IsBetween(1.0d, 100.0d).Should().BeFalse();

    // ── Math ──────────────────────────────────────────────────────────────────

    [Fact] public void Abs_Negative_ReturnsPositive() => (-3.14d).Abs().Should().Be(3.14d);
    [Fact] public void Abs_Positive_ReturnsSame()     => 3.14d.Abs().Should().Be(3.14d);
    [Fact] public void Abs_Zero_ReturnsZero()         => 0.0d.Abs().Should().Be(0.0d);

    [Theory]
    [InlineData(3.14159d, 2, 3.14d)]
    [InlineData(3.14159d, 4, 3.1416d)]
    [InlineData(1.236d,   2, 1.24d)]
    [InlineData(0.0d,     3, 0.0d)]
    public void RoundTo_ReturnsExpected(double value, int digits, double expected)
        => value.RoundTo(digits).Should().BeApproximately(expected, 1e-10);
}

public sealed class DoubleExtensions_LerpTests
{
    [Theory]
    [InlineData(0.0,  0.0, 10.0, 0.0)]
    [InlineData(1.0,  0.0, 10.0, 10.0)]
    [InlineData(0.5,  0.0, 10.0, 5.0)]
    [InlineData(0.25, 4.0, 8.0,  5.0)]
    public void Lerp_ReturnsExpected(double t, double a, double b, double expected)
        => t.Lerp(a, b).Should().BeApproximately(expected, 1e-10);
}

public sealed class DoubleExtensions_NormalizeTests
{
    [Theory]
    [InlineData(5.0,  0.0, 10.0, 0.5)]
    [InlineData(0.0,  0.0, 10.0, 0.0)]
    [InlineData(10.0, 0.0, 10.0, 1.0)]
    [InlineData(7.0,  7.0, 7.0,  0.0)]
    public void Normalize_ReturnsExpected(double value, double min, double max, double expected)
        => value.Normalize(min, max).Should().BeApproximately(expected, 1e-10);
}

public sealed class DoubleExtensions_ToRadiansTests
{
    [Theory]
    [InlineData(0.0,   0.0)]
    [InlineData(180.0, Math.PI)]
    [InlineData(360.0, 2 * Math.PI)]
    [InlineData(90.0,  Math.PI / 2)]
    public void ToRadians_ReturnsExpected(double degrees, double expected)
        => degrees.ToRadians().Should().BeApproximately(expected, 1e-10);

    [Theory]
    [InlineData(0.0,        0.0)]
    [InlineData(Math.PI,    180.0)]
    [InlineData(2 * Math.PI, 360.0)]
    [InlineData(Math.PI / 2, 90.0)]
    public void ToDegrees_ReturnsExpected(double radians, double expected)
        => radians.ToDegrees().Should().BeApproximately(expected, 1e-10);
}
