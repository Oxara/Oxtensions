// -----------------------------------------------------------------------
// <copyright file="LongExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Numeric;
using Xunit;

namespace Oxtensions.Tests.Numeric;

public sealed class LongExtensionsTests
{
    // ── Sign checks ───────────────────────────────────────────────────────────

    [Fact] public void IsPositive_Positive_ReturnsTrue()  => 99L.IsPositive().Should().BeTrue();
    [Fact] public void IsPositive_Zero_ReturnsFalse()     => 0L.IsPositive().Should().BeFalse();
    [Fact] public void IsPositive_Negative_ReturnsFalse() => (-1L).IsPositive().Should().BeFalse();

    [Fact] public void IsNegative_Negative_ReturnsTrue()  => (-1L).IsNegative().Should().BeTrue();
    [Fact] public void IsNegative_Zero_ReturnsFalse()     => 0L.IsNegative().Should().BeFalse();
    [Fact] public void IsNegative_Positive_ReturnsFalse() => 1L.IsNegative().Should().BeFalse();

    [Fact] public void IsZero_Zero_ReturnsTrue()     => 0L.IsZero().Should().BeTrue();
    [Fact] public void IsZero_NonZero_ReturnsFalse() => 1L.IsZero().Should().BeFalse();

    // ── Parity ────────────────────────────────────────────────────────────────

    [Fact] public void IsEven_EvenValue_ReturnsTrue()  => 4L.IsEven().Should().BeTrue();
    [Fact] public void IsEven_OddValue_ReturnsFalse()  => 3L.IsEven().Should().BeFalse();
    [Fact] public void IsOdd_OddValue_ReturnsTrue()    => 3L.IsOdd().Should().BeTrue();
    [Fact] public void IsOdd_EvenValue_ReturnsFalse()  => 4L.IsOdd().Should().BeFalse();

    // ── Range ─────────────────────────────────────────────────────────────────

    [Fact] public void Clamp_AboveMax_ReturnsMax()       => 200L.Clamp(0L, 100L).Should().Be(100L);
    [Fact] public void Clamp_BelowMin_ReturnsMin()       => (-5L).Clamp(0L, 100L).Should().Be(0L);
    [Fact] public void Clamp_InRange_ReturnsSame()       => 50L.Clamp(0L, 100L).Should().Be(50L);
    [Fact]
    public void Clamp_MinGreaterThanMax_Throws()
    {
        var act = () => 5L.Clamp(10L, 1L);
        act.Should().Throw<ArgumentException>();
    }

    [Fact] public void IsBetween_InRange_ReturnsTrue()      => 50L.IsBetween(1L, 100L).Should().BeTrue();
    [Fact] public void IsBetween_OnBoundary_ReturnsTrue()   => 1L.IsBetween(1L, 100L).Should().BeTrue();
    [Fact] public void IsBetween_OutOfRange_ReturnsFalse()  => 200L.IsBetween(1L, 100L).Should().BeFalse();

    // ── Math ──────────────────────────────────────────────────────────────────

    [Fact] public void Abs_Negative_ReturnsPositive() => (-99L).Abs().Should().Be(99L);
    [Fact] public void Abs_Positive_ReturnsSame()     => 99L.Abs().Should().Be(99L);
    [Fact] public void Abs_Zero_ReturnsZero()         => 0L.Abs().Should().Be(0L);

    [Theory]
    [InlineData(0L,       1)]
    [InlineData(9L,       1)]
    [InlineData(10L,      2)]
    [InlineData(999L,     3)]
    [InlineData(1000000L, 7)]
    public void DigitCount_ReturnsExpected(long value, int expected)
        => value.DigitCount().Should().Be(expected);

    [Fact]
    public void ToDigits_LargeNumber_ReturnsDigits()
        => 123456L.ToDigits().Should().Equal(1, 2, 3, 4, 5, 6);

    [Fact]
    public void ToDigits_NegativeNumber_ReturnsAbsDigits()
        => (-789L).ToDigits().Should().Equal(7, 8, 9);

    [Fact]
    public void ToDigits_Zero_ReturnsSingleZero()
        => 0L.ToDigits().Should().Equal(0);
}

public sealed class LongExtensions_IsMultipleOfTests
{
    [Theory]
    [InlineData(100L, 25L, true)]
    [InlineData(100L, 7L,  false)]
    [InlineData(0L,   3L,  true)]
    [InlineData(5L,   0L,  false)]
    public void IsMultipleOf_ReturnsExpected(long value, long divisor, bool expected)
        => value.IsMultipleOf(divisor).Should().Be(expected);
}

public sealed class LongExtensions_PowTests
{
    [Theory]
    [InlineData(2L,  10, 1024L)]
    [InlineData(3L,  4,  81L)]
    [InlineData(10L, 0,  1L)]
    public void Pow_ReturnsExpected(long value, int exponent, long expected)
        => value.Pow(exponent).Should().Be(expected);

    [Fact]
    public void Pow_NegativeExponent_ThrowsArgumentException()
    {
        var act = () => 2L.Pow(-1);
        act.Should().Throw<ArgumentException>();
    }
}
