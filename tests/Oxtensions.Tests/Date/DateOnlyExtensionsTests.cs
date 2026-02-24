// -----------------------------------------------------------------------
// <copyright file="DateOnlyExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Date;
using Xunit;

namespace Oxtensions.Tests.Date;

public sealed class DateOnlyExtensionsTests
{
    // ── IsWeekend / IsWeekday ─────────────────────────────────────────────────

    [Fact] public void IsWeekend_Saturday_ReturnsTrue()
        => new System.DateOnly(2026, 2, 21).IsWeekend().Should().BeTrue();

    [Fact] public void IsWeekend_Sunday_ReturnsTrue()
        => new System.DateOnly(2026, 2, 22).IsWeekend().Should().BeTrue();

    [Fact] public void IsWeekend_Monday_ReturnsFalse()
        => new System.DateOnly(2026, 2, 23).IsWeekend().Should().BeFalse();

    [Fact] public void IsWeekday_Monday_ReturnsTrue()
        => new System.DateOnly(2026, 2, 23).IsWeekday().Should().BeTrue();

    [Fact] public void IsWeekday_Saturday_ReturnsFalse()
        => new System.DateOnly(2026, 2, 21).IsWeekday().Should().BeFalse();

    // ── StartOfMonth / EndOfMonth ─────────────────────────────────────────────

    [Fact]
    public void StartOfMonth_MidMonth_ReturnsFirstDay()
        => new System.DateOnly(2026, 6, 15).StartOfMonth().Should().Be(new System.DateOnly(2026, 6, 1));

    [Fact]
    public void EndOfMonth_June_Returns30th()
        => new System.DateOnly(2026, 6, 1).EndOfMonth().Day.Should().Be(30);

    [Fact]
    public void EndOfMonth_February2026_Returns28th()
        => new System.DateOnly(2026, 2, 10).EndOfMonth().Day.Should().Be(28);

    [Fact]
    public void EndOfMonth_LeapYear_Returns29th()
        => new System.DateOnly(2024, 2, 1).EndOfMonth().Day.Should().Be(29);

    // ── StartOfYear / EndOfYear ───────────────────────────────────────────────

    [Fact]
    public void StartOfYear_MidYear_ReturnsJan1()
        => new System.DateOnly(2026, 8, 20).StartOfYear().Should().Be(new System.DateOnly(2026, 1, 1));

    [Fact]
    public void EndOfYear_MidYear_ReturnsDec31()
        => new System.DateOnly(2026, 3, 10).EndOfYear().Should().Be(new System.DateOnly(2026, 12, 31));

    // ── StartOfWeek ───────────────────────────────────────────────────────────

    [Fact]
    public void StartOfWeek_Wednesday_ReturnsPreviousMonday()
    {
        var wednesday = new System.DateOnly(2026, 2, 18); // Wednesday
        wednesday.StartOfWeek().DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void StartOfWeek_Sunday_SundayStart_ReturnsSunday()
    {
        var sunday = new System.DateOnly(2026, 2, 22);
        sunday.StartOfWeek(DayOfWeek.Sunday).DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void StartOfWeek_Monday_ReturnsSelf()
    {
        var monday = new System.DateOnly(2026, 2, 23);
        monday.StartOfWeek().Should().Be(monday);
    }

    // ── ToDateTime ────────────────────────────────────────────────────────────

    [Fact]
    public void ToDateTime_DefaultTime_ReturnsMidnight()
    {
        var date = new System.DateOnly(2026, 6, 15);
        var dt = date.ToDateTime();
        dt.Hour.Should().Be(0);
        dt.Minute.Should().Be(0);
        dt.Second.Should().Be(0);
    }

    [Fact]
    public void ToDateTime_WithTime_ReturnsCombined()
    {
        var date = new System.DateOnly(2026, 6, 15);
        var time = new System.TimeOnly(14, 30, 0);
        var dt = date.ToDateTime(time);
        dt.Hour.Should().Be(14);
        dt.Minute.Should().Be(30);
    }

    // ── Age ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Age_BirthDateInPast_ReturnsPositiveAge()
        => new System.DateOnly(1990, 1, 1).Age().Should().BeGreaterThan(30);

    [Fact]
    public void Age_BirthdayToday_ReturnsCorrectAge()
    {
        var today = System.DateOnly.FromDateTime(System.DateTime.Today);
        today.AddYears(-25).Age().Should().Be(25);
    }

    // ── IsToday / IsPast / IsFuture ───────────────────────────────────────────

    [Fact]
    public void IsToday_Today_ReturnsTrue()
        => System.DateOnly.FromDateTime(System.DateTime.Today).IsToday().Should().BeTrue();

    [Fact]
    public void IsToday_Yesterday_ReturnsFalse()
        => System.DateOnly.FromDateTime(System.DateTime.Today).AddDays(-1).IsToday().Should().BeFalse();

    [Fact]
    public void IsPast_Yesterday_ReturnsTrue()
        => System.DateOnly.FromDateTime(System.DateTime.Today).AddDays(-1).IsPast().Should().BeTrue();

    [Fact]
    public void IsFuture_Tomorrow_ReturnsTrue()
        => System.DateOnly.FromDateTime(System.DateTime.Today).AddDays(1).IsFuture().Should().BeTrue();

    // ── NextWorkday / PreviousWorkday ─────────────────────────────────────────

    [Fact]
    public void NextWorkday_Friday_ReturnsMonday()
        => new System.DateOnly(2026, 2, 20).NextWorkday().DayOfWeek.Should().Be(DayOfWeek.Monday);

    [Fact]
    public void NextWorkday_Saturday_ReturnsMonday()
        => new System.DateOnly(2026, 2, 21).NextWorkday().DayOfWeek.Should().Be(DayOfWeek.Monday);

    [Fact]
    public void PreviousWorkday_Monday_ReturnsFriday()
        => new System.DateOnly(2026, 2, 23).PreviousWorkday().DayOfWeek.Should().Be(DayOfWeek.Friday);

    [Fact]
    public void PreviousWorkday_Sunday_ReturnsFriday()
        => new System.DateOnly(2026, 2, 22).PreviousWorkday().DayOfWeek.Should().Be(DayOfWeek.Friday);
}
