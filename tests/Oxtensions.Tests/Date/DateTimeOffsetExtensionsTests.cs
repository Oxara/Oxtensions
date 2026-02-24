// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Date;
using Xunit;

namespace Oxtensions.Tests.Date;

public sealed class DateTimeOffsetExtensionsTests
{
    private static readonly System.TimeSpan Utc3 = System.TimeSpan.FromHours(3);

    // ── ToUnixTimestamp / ToDateTimeOffset ────────────────────────────────────

    [Fact]
    public void ToUnixTimestamp_KnownDate_ReturnsCorrectTimestamp()
    {
        var dto = new System.DateTimeOffset(2000, 1, 1, 0, 0, 0, System.TimeSpan.Zero);
        dto.ToUnixTimestamp().Should().Be(946684800L);
    }

    [Fact]
    public void ToDateTimeOffset_KnownTimestamp_ReturnsUtcDate()
    {
        var result = 946684800L.ToDateTimeOffset();
        result.UtcDateTime.Should().Be(new System.DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ToUnixTimestamp_ThenToDateTimeOffset_RoundTrips()
    {
        var original = new System.DateTimeOffset(2026, 6, 15, 12, 0, 0, System.TimeSpan.Zero);
        original.ToUnixTimestamp().ToDateTimeOffset().ToUnixTimeSeconds().Should().Be(original.ToUnixTimeSeconds());
    }

    // ── IsWeekend / IsWeekday ─────────────────────────────────────────────────

    [Fact] public void IsWeekend_Saturday_ReturnsTrue()
        => new System.DateTimeOffset(2026, 2, 21, 0, 0, 0, System.TimeSpan.Zero).IsWeekend().Should().BeTrue();

    [Fact] public void IsWeekend_Monday_ReturnsFalse()
        => new System.DateTimeOffset(2026, 2, 23, 0, 0, 0, System.TimeSpan.Zero).IsWeekend().Should().BeFalse();

    [Fact] public void IsWeekday_Monday_ReturnsTrue()
        => new System.DateTimeOffset(2026, 2, 23, 0, 0, 0, System.TimeSpan.Zero).IsWeekday().Should().BeTrue();

    // ── StartOfDay / EndOfDay ─────────────────────────────────────────────────

    [Fact]
    public void StartOfDay_MidDay_ReturnsMidnight()
    {
        var dto = new System.DateTimeOffset(2026, 6, 15, 14, 30, 0, Utc3);
        var result = dto.StartOfDay();
        result.Hour.Should().Be(0);
        result.Minute.Should().Be(0);
        result.Offset.Should().Be(Utc3);
    }

    [Fact]
    public void EndOfDay_MidDay_Returns235959()
    {
        var dto = new System.DateTimeOffset(2026, 6, 15, 8, 0, 0, Utc3);
        var result = dto.EndOfDay();
        result.Hour.Should().Be(23);
        result.Minute.Should().Be(59);
        result.Second.Should().Be(59);
        result.Offset.Should().Be(Utc3);
    }

    // ── StartOfMonth / EndOfMonth ─────────────────────────────────────────────

    [Fact]
    public void StartOfMonth_MidMonth_ReturnsFirstDay()
    {
        var dto = new System.DateTimeOffset(2026, 6, 15, 0, 0, 0, Utc3);
        dto.StartOfMonth().Day.Should().Be(1);
        dto.StartOfMonth().Offset.Should().Be(Utc3);
    }

    [Fact]
    public void EndOfMonth_June_Returns30th()
    {
        var dto = new System.DateTimeOffset(2026, 6, 1, 0, 0, 0, Utc3);
        dto.EndOfMonth().Day.Should().Be(30);
    }

    [Fact]
    public void EndOfMonth_LeapYear_Returns29th()
    {
        var dto = new System.DateTimeOffset(2024, 2, 1, 0, 0, 0, System.TimeSpan.Zero);
        dto.EndOfMonth().Day.Should().Be(29);
    }

    // ── StartOfYear / EndOfYear ───────────────────────────────────────────────

    [Fact]
    public void StartOfYear_MidYear_ReturnsJan1()
    {
        var dto = new System.DateTimeOffset(2026, 8, 20, 0, 0, 0, Utc3);
        var result = dto.StartOfYear();
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
        result.Offset.Should().Be(Utc3);
    }

    [Fact]
    public void EndOfYear_MidYear_ReturnsDec31()
    {
        var dto = new System.DateTimeOffset(2026, 3, 10, 0, 0, 0, Utc3);
        var result = dto.EndOfYear();
        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
        result.Offset.Should().Be(Utc3);
    }

    // ── StartOfWeek ───────────────────────────────────────────────────────────

    [Fact]
    public void StartOfWeek_Wednesday_ReturnsPreviousMonday()
    {
        var wednesday = new System.DateTimeOffset(2026, 2, 18, 12, 0, 0, Utc3);
        wednesday.StartOfWeek().DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void StartOfWeek_PreservesOffset()
    {
        var dto = new System.DateTimeOffset(2026, 2, 18, 12, 0, 0, Utc3);
        dto.StartOfWeek().Offset.Should().Be(Utc3);
    }

    // ── Age ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Age_BirthDateInPast_ReturnsPositiveAge()
    {
        var birthDate = new System.DateTimeOffset(1990, 1, 1, 0, 0, 0, System.TimeSpan.Zero);
        birthDate.Age().Should().BeGreaterThan(30);
    }

    // ── IsToday / IsPast / IsFuture ───────────────────────────────────────────

    [Fact]
    public void IsToday_Today_ReturnsTrue()
        => System.DateTimeOffset.Now.IsToday().Should().BeTrue();

    [Fact]
    public void IsToday_Yesterday_ReturnsFalse()
        => System.DateTimeOffset.UtcNow.AddDays(-1).IsToday().Should().BeFalse();

    [Fact]
    public void IsPast_PastValue_ReturnsTrue()
        => System.DateTimeOffset.UtcNow.AddMinutes(-1).IsPast().Should().BeTrue();

    [Fact]
    public void IsFuture_FutureValue_ReturnsTrue()
        => System.DateTimeOffset.UtcNow.AddMinutes(1).IsFuture().Should().BeTrue();

    // ── NextWorkday / PreviousWorkday ─────────────────────────────────────────

    [Fact]
    public void NextWorkday_Friday_ReturnsMonday()
    {
        var friday = new System.DateTimeOffset(2026, 2, 20, 12, 0, 0, Utc3);
        friday.NextWorkday().DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void NextWorkday_PreservesOffset()
    {
        var friday = new System.DateTimeOffset(2026, 2, 20, 12, 0, 0, Utc3);
        friday.NextWorkday().Offset.Should().Be(Utc3);
    }

    [Fact]
    public void PreviousWorkday_Monday_ReturnsFriday()
    {
        var monday = new System.DateTimeOffset(2026, 2, 23, 12, 0, 0, Utc3);
        monday.PreviousWorkday().DayOfWeek.Should().Be(DayOfWeek.Friday);
    }

    // ── ToIso8601String ───────────────────────────────────────────────────────

    [Fact]
    public void ToIso8601String_Utc_ReturnsCorrectFormat()
    {
        var dto = new System.DateTimeOffset(2026, 2, 22, 12, 0, 0, System.TimeSpan.Zero);
        dto.ToIso8601String().Should().Be("2026-02-22T12:00:00+00:00");
    }

    [Fact]
    public void ToIso8601String_WithOffset_IncludesOffset()
    {
        var dto = new System.DateTimeOffset(2026, 2, 22, 15, 0, 0, Utc3);
        dto.ToIso8601String().Should().Be("2026-02-22T15:00:00+03:00");
    }
}
