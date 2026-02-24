// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Date;
using Xunit;
using SysDateTime = System.DateTime;

namespace Oxtensions.Tests.Date;

public sealed class DateTimeExtensionsTests
{
    // ── ToUnixTimestamp / FromUnixTimestamp ───────────────────────────────────

    [Fact]
    public void ToUnixTimestamp_KnownDate_ReturnsCorrectTimestamp()
    {
        var dt = new SysDateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dt.ToUnixTimestamp().Should().Be(946684800L);
    }

    [Fact]
    public void FromUnixTimestamp_KnownTimestamp_ReturnsCorrectDate()
    {
        946684800L.FromUnixTimestamp().Should().Be(new SysDateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ToUnixTimestamp_ThenFromUnixTimestamp_RoundTrips()
    {
        var original = new SysDateTime(2026, 2, 22, 12, 0, 0, DateTimeKind.Utc);
        original.ToUnixTimestamp().FromUnixTimestamp().Should().Be(original);
    }

    // ── IsWeekend / IsWeekday ────────────────────────────────────────────────

    [Fact]
    public void IsWeekend_Saturday_ReturnsTrue()
        => new SysDateTime(2026, 2, 21).IsWeekend().Should().BeTrue();

    [Fact]
    public void IsWeekend_Monday_ReturnsFalse()
        => new SysDateTime(2026, 2, 23).IsWeekend().Should().BeFalse();

    [Fact]
    public void IsWeekday_Monday_ReturnsTrue()
        => new SysDateTime(2026, 2, 23).IsWeekday().Should().BeTrue();

    [Theory]
    [InlineData(2026, 2, 21, true)]  // Saturday
    [InlineData(2026, 2, 22, true)]  // Sunday
    [InlineData(2026, 2, 23, false)] // Monday
    public void IsWeekend_VariousDates_ReturnsExpected(int year, int month, int day, bool expected)
        => new SysDateTime(year, month, day).IsWeekend().Should().Be(expected);

    // ── StartOfDay / EndOfDay ────────────────────────────────────────────────

    [Fact]
    public void StartOfDay_DateTime_ReturnsMidnight()
    {
        var dt = new SysDateTime(2026, 2, 22, 15, 30, 45);
        dt.StartOfDay().Should().Be(new SysDateTime(2026, 2, 22, 0, 0, 0));
    }

    [Fact]
    public void EndOfDay_DateTime_Returns235959()
    {
        var dt = new SysDateTime(2026, 2, 22, 8, 0, 0);
        var end = dt.EndOfDay();
        end.Hour.Should().Be(23);
        end.Minute.Should().Be(59);
        end.Second.Should().Be(59);
    }

    // ── StartOfYear / EndOfYear ───────────────────────────────────────────────

    [Fact]
    public void StartOfYear_MidYear_ReturnsJanuary1Midnight()
    {
        var dt = new SysDateTime(2026, 6, 15, 10, 30, 0);
        var result = dt.StartOfYear();
        result.Should().Be(new SysDateTime(2026, 1, 1, 0, 0, 0));
    }

    [Fact]
    public void StartOfYear_PreservesDateTimeKind()
    {
        var dt = new SysDateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc);
        dt.StartOfYear().Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void StartOfYear_AlwaysReturnsJan1()
    {
        var result = new SysDateTime(2026, 12, 31).StartOfYear();
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
    }

    [Fact]
    public void EndOfYear_MidYear_ReturnsDec31At235959()
    {
        var dt = new SysDateTime(2026, 3, 10, 8, 0, 0);
        var result = dt.EndOfYear();
        result.Should().Be(new SysDateTime(2026, 12, 31, 23, 59, 59, 999));
    }

    [Fact]
    public void EndOfYear_PreservesDateTimeKind()
    {
        var dt = new SysDateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        dt.EndOfYear().Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void EndOfYear_AlwaysReturnsDec31()
    {
        var result = new SysDateTime(2026, 1, 1).EndOfYear();
        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
    }

    // ── StartOfMonth / EndOfMonth ─────────────────────────────────────────────

    [Fact]
    public void StartOfMonth_MidMonth_ReturnsFirstDay()
    {
        new SysDateTime(2026, 2, 15).StartOfMonth().Day.Should().Be(1);
    }

    [Fact]
    public void EndOfMonth_February2026_Returns28thDay()
    {
        new SysDateTime(2026, 2, 15).EndOfMonth().Day.Should().Be(28);
    }

    [Fact]
    public void EndOfMonth_LeapYear_Returns29()
    {
        new SysDateTime(2024, 2, 1).EndOfMonth().Day.Should().Be(29);
    }

    // ── StartOfWeek ────────────────────────────────────────────────────────────

    [Fact]
    public void StartOfWeek_Wednesday_ReturnsPreviousMonday()
    {
        var wednesday = new SysDateTime(2026, 2, 18); // Wednesday
        wednesday.StartOfWeek().DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void StartOfWeek_Sunday_SundayStart_ReturnsSunday()
    {
        var sunday = new SysDateTime(2026, 2, 22); // Sunday
        sunday.StartOfWeek(DayOfWeek.Sunday).DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }

    // ── Age ────────────────────────────────────────────────────────────────────

    [Fact]
    public void Age_BirthDateInPast_ReturnsPositiveAge()
    {
        var birthDate = new SysDateTime(1990, 1, 1);
        birthDate.Age().Should().BeGreaterThan(30);
    }

    [Fact]
    public void Age_BirthdayToday_ReturnsCorrectAge()
    {
        var today = SysDateTime.Today;
        var birthDate = today.AddYears(-25);
        birthDate.Age().Should().Be(25);
    }


    // ── IsToday / IsPast / IsFuture ────────────────────────────────────────────

    [Fact]
    public void IsToday_Today_ReturnsTrue()
        => SysDateTime.Today.IsToday().Should().BeTrue();

    [Fact]
    public void IsToday_Yesterday_ReturnsFalse()
        => SysDateTime.Today.AddDays(-1).IsToday().Should().BeFalse();

    [Fact]
    public void IsPast_PastDate_ReturnsTrue()
        => SysDateTime.UtcNow.AddMinutes(-1).IsPast().Should().BeTrue();

    [Fact]
    public void IsFuture_FutureDate_ReturnsTrue()
        => SysDateTime.UtcNow.AddMinutes(1).IsFuture().Should().BeTrue();

    // ── NextWorkday / PreviousWorkday ─────────────────────────────────────────

    [Fact]
    public void NextWorkday_Friday_ReturnsMonday()
    {
        var friday = new SysDateTime(2026, 2, 20); // Friday
        friday.NextWorkday().DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void PreviousWorkday_Monday_ReturnsFriday()
    {
        var monday = new SysDateTime(2026, 2, 23); // Monday
        monday.PreviousWorkday().DayOfWeek.Should().Be(DayOfWeek.Friday);
    }

    // ── ToIso8601String ────────────────────────────────────────────────────────

    [Fact]
    public void ToIso8601String_UtcDate_ReturnsIsoFormat()
    {
        var dt = new SysDateTime(2026, 2, 22, 12, 0, 0, DateTimeKind.Utc);
        dt.ToIso8601String().Should().Be("2026-02-22T12:00:00Z");
    }
}
