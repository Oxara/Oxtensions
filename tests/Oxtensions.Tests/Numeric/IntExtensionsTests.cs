// -----------------------------------------------------------------------
// <copyright file="IntExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Numeric;
using Xunit;

namespace Oxtensions.Tests.Numeric;

public sealed class IntExtensionsTests
{
    // ── Sign checks ───────────────────────────────────────────────────────────

    [Fact] public void IsPositive_Positive_ReturnsTrue()  => 5.IsPositive().Should().BeTrue();
    [Fact] public void IsPositive_Zero_ReturnsFalse()     => 0.IsPositive().Should().BeFalse();
    [Fact] public void IsPositive_Negative_ReturnsFalse() => (-1).IsPositive().Should().BeFalse();

    [Fact] public void IsNegative_Negative_ReturnsTrue()  => (-3).IsNegative().Should().BeTrue();
    [Fact] public void IsNegative_Zero_ReturnsFalse()     => 0.IsNegative().Should().BeFalse();
    [Fact] public void IsNegative_Positive_ReturnsFalse() => 1.IsNegative().Should().BeFalse();

    [Fact] public void IsZero_Zero_ReturnsTrue()     => 0.IsZero().Should().BeTrue();
    [Fact] public void IsZero_NonZero_ReturnsFalse() => 1.IsZero().Should().BeFalse();

    // ── Parity ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)] [InlineData(2)] [InlineData(100)] [InlineData(-4)]
    public void IsEven_EvenValues_ReturnsTrue(int value) => value.IsEven().Should().BeTrue();

    [Theory]
    [InlineData(1)] [InlineData(3)] [InlineData(-7)]
    public void IsOdd_OddValues_ReturnsTrue(int value) => value.IsOdd().Should().BeTrue();

    [Fact] public void IsEven_OddValue_ReturnsFalse() => 3.IsEven().Should().BeFalse();
    [Fact] public void IsOdd_EvenValue_ReturnsFalse() => 4.IsOdd().Should().BeFalse();

    // ── Range ─────────────────────────────────────────────────────────────────

    [Fact] public void Clamp_AboveMax_ReturnsMax() => 200.Clamp(0, 100).Should().Be(100);
    [Fact] public void Clamp_BelowMin_ReturnsMin() => (-5).Clamp(0, 100).Should().Be(0);
    [Fact] public void Clamp_InRange_ReturnsSame() => 50.Clamp(0, 100).Should().Be(50);
    [Fact]
    public void Clamp_MinGreaterThanMax_Throws()
    {
        var act = () => 5.Clamp(10, 1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact] public void IsBetween_InRange_ReturnsTrue()     => 5.IsBetween(1, 10).Should().BeTrue();
    [Fact] public void IsBetween_OnMin_ReturnsTrue()       => 1.IsBetween(1, 10).Should().BeTrue();
    [Fact] public void IsBetween_OnMax_ReturnsTrue()       => 10.IsBetween(1, 10).Should().BeTrue();
    [Fact] public void IsBetween_OutOfRange_ReturnsFalse() => 11.IsBetween(1, 10).Should().BeFalse();

    // ── Math ──────────────────────────────────────────────────────────────────

    [Fact] public void Abs_Negative_ReturnsPositive() => (-7).Abs().Should().Be(7);
    [Fact] public void Abs_Positive_ReturnsSame()     => 7.Abs().Should().Be(7);
    [Fact] public void Abs_Zero_ReturnsZero()         => 0.Abs().Should().Be(0);

    [Theory]
    [InlineData(2,  true)]
    [InlineData(3,  true)]
    [InlineData(7,  true)]
    [InlineData(17, true)]
    [InlineData(97, true)]
    public void IsPrime_PrimeValues_ReturnsTrue(int value, bool expected)
        => value.IsPrime().Should().Be(expected);

    [Theory]
    [InlineData(0,  false)]
    [InlineData(1,  false)]
    [InlineData(4,  false)]
    [InlineData(9,  false)]
    [InlineData(-5, false)]
    public void IsPrime_NonPrimeValues_ReturnsFalse(int value, bool expected)
        => value.IsPrime().Should().Be(expected);

    [Theory]
    [InlineData(0, 1L)]
    [InlineData(1, 1L)]
    [InlineData(5, 120L)]
    [InlineData(10, 3628800L)]
    [InlineData(20, 2432902008176640000L)]
    public void Factorial_ValidInput_ReturnsCorrect(int value, long expected)
        => value.Factorial().Should().Be(expected);

    [Fact]
    public void Factorial_Negative_Throws()
    {
        var act = () => (-1).Factorial();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Factorial_Over20_Throws()
    {
        var act = () => 21.Factorial();
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0,     1)]
    [InlineData(5,     1)]
    [InlineData(99,    2)]
    [InlineData(100,   3)]
    [InlineData(-1234, 4)]
    [InlineData(int.MaxValue, 10)]
    public void DigitCount_ReturnsExpected(int value, int expected)
        => value.DigitCount().Should().Be(expected);

    [Fact]
    public void ToDigits_PositiveNumber_ReturnsDigits()
        => 1234.ToDigits().Should().Equal(1, 2, 3, 4);

    [Fact]
    public void ToDigits_NegativeNumber_ReturnsAbsDigits()
        => (-567).ToDigits().Should().Equal(5, 6, 7);

    [Fact]
    public void ToDigits_Zero_ReturnsSingleZero()
        => 0.ToDigits().Should().Equal(0);
}

public sealed class IntExtensions_ToOrdinalTests
{
    [Theory]
    [InlineData(1,  "1st")]
    [InlineData(2,  "2nd")]
    [InlineData(3,  "3rd")]
    [InlineData(4,  "4th")]
    [InlineData(11, "11th")]
    [InlineData(12, "12th")]
    [InlineData(13, "13th")]
    [InlineData(21, "21st")]
    [InlineData(22, "22nd")]
    [InlineData(23, "23rd")]
    [InlineData(100, "100th")]
    public void ToOrdinal_ReturnsExpected(int value, string expected)
        => value.ToOrdinal().Should().Be(expected);
}

public sealed class IntExtensions_ToRomanTests
{
    [Theory]
    [InlineData(1,    "I")]
    [InlineData(4,    "IV")]
    [InlineData(9,    "IX")]
    [InlineData(14,   "XIV")]
    [InlineData(40,   "XL")]
    [InlineData(1994, "MCMXCIV")]
    [InlineData(3999, "MMMCMXCIX")]
    public void ToRoman_ReturnsExpected(int value, string expected)
        => value.ToRoman().Should().Be(expected);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4000)]
    public void ToRoman_OutOfRange_ThrowsArgumentException(int value)
    {
        var act = () => value.ToRoman();
        act.Should().Throw<ArgumentException>();
    }
}

public sealed class IntExtensions_IsMultipleOfTests
{
    [Theory]
    [InlineData(10, 5,  true)]
    [InlineData(10, 3,  false)]
    [InlineData(0,  7,  true)]
    [InlineData(10, 0,  false)]
    public void IsMultipleOf_ReturnsExpected(int value, int divisor, bool expected)
        => value.IsMultipleOf(divisor).Should().Be(expected);
}

public sealed class IntExtensions_PowTests
{
    [Theory]
    [InlineData(2, 10, 1024)]
    [InlineData(3, 4,  81)]
    [InlineData(5, 0,  1)]
    [InlineData(0, 5,  0)]
    public void Pow_ReturnsExpected(int value, int exponent, int expected)
        => value.Pow(exponent).Should().Be(expected);

    [Fact]
    public void Pow_NegativeExponent_ThrowsArgumentException()
    {
        var act = () => 2.Pow(-1);
        act.Should().Throw<ArgumentException>();
    }
}
