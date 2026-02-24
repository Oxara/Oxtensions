// -----------------------------------------------------------------------
// <copyright file="TimeSpanExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Durations;
using Xunit;
using Ts = System.TimeSpan;

public sealed class TimeSpanExtensions_IsPositiveNegativeZeroTests
{
    [Fact] public void IsPositive_Positive_ReturnsTrue()  => Ts.FromSeconds(1).IsPositive().Should().BeTrue();
    [Fact] public void IsPositive_Negative_ReturnsFalse() => Ts.FromSeconds(-1).IsPositive().Should().BeFalse();
    [Fact] public void IsPositive_Zero_ReturnsFalse()     => Ts.Zero.IsPositive().Should().BeFalse();

    [Fact] public void IsNegative_Negative_ReturnsTrue()  => Ts.FromSeconds(-1).IsNegative().Should().BeTrue();
    [Fact] public void IsNegative_Positive_ReturnsFalse() => Ts.FromSeconds(1).IsNegative().Should().BeFalse();

    [Fact] public void IsZero_Zero_ReturnsTrue()          => Ts.Zero.IsZero().Should().BeTrue();
    [Fact] public void IsZero_NonZero_ReturnsFalse()      => Ts.FromSeconds(1).IsZero().Should().BeFalse();
}

public sealed class TimeSpanExtensions_AbsTests
{
    [Fact]
    public void Abs_Negative_ReturnsPositive()
        => Ts.FromSeconds(-5).Abs().Should().Be(Ts.FromSeconds(5));

    [Fact]
    public void Abs_Positive_ReturnsSame()
        => Ts.FromSeconds(5).Abs().Should().Be(Ts.FromSeconds(5));

    [Fact]
    public void Abs_Zero_ReturnsZero()
        => Ts.Zero.Abs().Should().Be(Ts.Zero);
}

public sealed class TimeSpanExtensions_TotalWeeksTests
{
    [Fact]
    public void TotalWeeks_SevenDays_ReturnsOne()
        => Ts.FromDays(7).TotalWeeks().Should().BeApproximately(1.0, 1e-10);

    [Fact]
    public void TotalWeeks_FourteenDays_ReturnsTwo()
        => Ts.FromDays(14).TotalWeeks().Should().BeApproximately(2.0, 1e-10);
}

public sealed class TimeSpanExtensions_ToHumanReadableTests
{
    [Fact]
    public void ToHumanReadable_Days_ContainsDays()
        => Ts.FromDays(3).ToHumanReadable().Should().Contain("d");

    [Fact]
    public void ToHumanReadable_Hours_ContainsHours()
        => Ts.FromHours(2).ToHumanReadable().Should().Contain("h");

    [Fact]
    public void ToHumanReadable_Zero_ReturnsZeroSeconds()
        => Ts.Zero.ToHumanReadable().Should().Be("0s");

    [Fact]
    public void ToHumanReadable_KnownDuration_ReturnsExpectedString()
        => new Ts(1, 4, 30, 0).ToHumanReadable().Should().Be("1d 4h 30m 0s");

    [Fact]
    public void ToHumanReadable_HoursOnly_ReturnsExpectedString()
        => Ts.FromHours(2.5d).ToHumanReadable().Should().Be("2h 30m 0s");
}

public sealed class TimeSpanExtensions_ToIso8601DurationTests
{
    [Fact]
    public void ToIso8601Duration_TwoDays_ReturnsCorrectFormat()
        => Ts.FromDays(2).ToIso8601Duration().Should().StartWith("P");

    [Fact]
    public void ToIso8601Duration_TwoHours_ContainsHours()
        => Ts.FromHours(2).ToIso8601Duration().Should().Contain("T");

    [Fact]
    public void ToIso8601Duration_KnownDuration_ReturnsExpectedString()
        => Ts.FromDays(2).ToIso8601Duration().Should().Be("P2DT0H0M0S");

    [Fact]
    public void ToIso8601Duration_MinutesOnly_ReturnsExpectedString()
        => Ts.FromMinutes(150).ToIso8601Duration().Should().Be("PT2H30M0S");
}

public sealed class IntDurationExtensions_FluentBuilderTests
{
    [Fact] public void Days_Int_ReturnsTimeSpan()        => 3.Days().Should().Be(Ts.FromDays(3));
    [Fact] public void Hours_Int_ReturnsTimeSpan()       => 2.Hours().Should().Be(Ts.FromHours(2));
    [Fact] public void Minutes_Int_ReturnsTimeSpan()     => 30.Minutes().Should().Be(Ts.FromMinutes(30));
    [Fact] public void Seconds_Int_ReturnsTimeSpan()     => 45.Seconds().Should().Be(Ts.FromSeconds(45));
    [Fact] public void Milliseconds_Int_ReturnsTimeSpan()=> 500.Milliseconds().Should().Be(Ts.FromMilliseconds(500));
}

public sealed class LongDurationExtensions_FluentBuilderTests
{
    [Fact] public void Days_Long_ReturnsTimeSpan()        => 3L.Days().Should().Be(Ts.FromDays(3));
    [Fact] public void Hours_Long_ReturnsTimeSpan()       => 2L.Hours().Should().Be(Ts.FromHours(2));
    [Fact] public void Minutes_Long_ReturnsTimeSpan()     => 30L.Minutes().Should().Be(Ts.FromMinutes(30));
    [Fact] public void Seconds_Long_ReturnsTimeSpan()     => 45L.Seconds().Should().Be(Ts.FromSeconds(45));
    [Fact] public void Milliseconds_Long_ReturnsTimeSpan()=> 500L.Milliseconds().Should().Be(Ts.FromMilliseconds(500));
}
