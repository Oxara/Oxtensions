// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Date;

/// <summary>
/// High-performance extension methods for <see cref="System.DateTime"/>.
/// </summary>
public static class DateTimeExtensions
{
    private static readonly System.DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // ─────────────────────────────────────────────────────────────────────────
    // Unix timestamp
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> to a Unix timestamp (seconds since epoch).
    /// </summary>
    /// <param name="value">The date/time value (treated as UTC).</param>
    /// <returns>Seconds since Unix epoch.</returns>
    /// <example>
    /// <code>
    /// new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp(); // 946684800
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToUnixTimestamp(this System.DateTime value)
        => (long)(value.ToUniversalTime() - UnixEpoch).TotalSeconds;

    /// <summary>
    /// Converts a Unix timestamp (seconds) to a UTC <see cref="System.DateTime"/>.
    /// </summary>
    /// <param name="timestamp">Seconds since Unix epoch.</param>
    /// <returns>Corresponding UTC <see cref="System.DateTime"/>.</returns>
    /// <example>
    /// <code>
    /// 946684800L.FromUnixTimestamp(); // 2000-01-01 00:00:00 UTC
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime FromUnixTimestamp(this long timestamp)
        => UnixEpoch.AddSeconds(timestamp);

    // ─────────────────────────────────────────────────────────────────────────
    // Weekend / Weekday
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a Saturday or Sunday.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when Saturday or Sunday; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// new DateTime(2026, 2, 21).IsWeekend(); // true (Saturday)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWeekend(this System.DateTime value)
        => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a weekday (Monday–Friday).
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when Monday–Friday; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWeekday(this System.DateTime value)
        => !value.IsWeekend();

    // ─────────────────────────────────────────────────────────────────────────
    // Start / End of Day / Month / Year / Week
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a <see cref="System.DateTime"/> representing midnight (00:00:00.000) of the same date,
    /// preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>Start of day.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime StartOfDay(this System.DateTime value)
        => value.Date;

    /// <summary>
    /// Returns a <see cref="System.DateTime"/> representing 23:59:59.999 of the same date,
    /// preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>End of day.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime EndOfDay(this System.DateTime value)
        => new(value.Year, value.Month, value.Day, 23, 59, 59, 999, value.Kind);

    /// <summary>
    /// Returns the first day of the month at midnight, preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>First day of the calendar month.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime StartOfMonth(this System.DateTime value)
        => new(value.Year, value.Month, 1, 0, 0, 0, value.Kind);

    /// <summary>
    /// Returns the last day of the month at 23:59:59.999, preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>Last moment of the calendar month.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime EndOfMonth(this System.DateTime value)
    {
        int lastDay = System.DateTime.DaysInMonth(value.Year, value.Month);
        return new System.DateTime(value.Year, value.Month, lastDay, 23, 59, 59, 999, value.Kind);
    }

    /// <summary>
    /// Returns January 1st of the same year at midnight (00:00:00.000), preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>First moment of the calendar year.</returns>
    /// <example>
    /// <code>
    /// new DateTime(2026, 6, 15).StartOfYear(); // 2026-01-01 00:00:00
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime StartOfYear(this System.DateTime value)
        => new(value.Year, 1, 1, 0, 0, 0, value.Kind);

    /// <summary>
    /// Returns December 31st of the same year at 23:59:59.999, preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>Last moment of the calendar year.</returns>
    /// <example>
    /// <code>
    /// new DateTime(2026, 6, 15).EndOfYear(); // 2026-12-31 23:59:59.999
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime EndOfYear(this System.DateTime value)
        => new(value.Year, 12, 31, 23, 59, 59, 999, value.Kind);

    /// <summary>
    /// Returns the start of the week (midnight) for the given date, preserving <see cref="DateTimeKind"/>.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <param name="startDay">First day of the week. Defaults to <see cref="DayOfWeek.Monday"/>.</param>
    /// <returns>Start of the containing week.</returns>
    /// <example>
    /// <code>
    /// someWednesday.StartOfWeek(); // previous Monday at 00:00
    /// </code>
    /// </example>
    public static System.DateTime StartOfWeek(this System.DateTime value, DayOfWeek startDay = DayOfWeek.Monday)
    {
        int diff = ((int)value.DayOfWeek - (int)startDay + 7) % 7;
        return value.Date.AddDays(-diff);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Age
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calculates age in completed years from a birth date to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>Age in full years.</returns>
    /// <example>
    /// <code>
    /// new DateTime(1990, 5, 15).Age(); // years since 1990-05-15
    /// </code>
    /// </example>
    public static int Age(this System.DateTime birthDate)
    {
        var today = System.DateTime.Today;
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Is Today / Past / Future
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the date is today (local date comparison).
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when today; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsToday(this System.DateTime value)
        => value.Date == System.DateTime.Today;

    /// <summary>
    /// Returns <see langword="true"/> if the date is in the past relative to UTC now.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when past; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPast(this System.DateTime value)
        => value.ToUniversalTime() < System.DateTime.UtcNow;

    /// <summary>
    /// Returns <see langword="true"/> if the date is in the future relative to UTC now.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when future; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFuture(this System.DateTime value)
        => value.ToUniversalTime() > System.DateTime.UtcNow;

    // ─────────────────────────────────────────────────────────────────────────
    // Next / Previous Workday
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the next calendar workday (Monday–Friday) after the given date.
    /// </summary>
    /// <param name="value">The reference date.</param>
    /// <returns>Next workday.</returns>
    /// <example>
    /// <code>
    /// new DateTime(2026, 2, 20).NextWorkday(); // 2026-02-23 (skips weekend)
    /// </code>
    /// </example>
    public static System.DateTime NextWorkday(this System.DateTime value)
    {
        var next = value.Date.AddDays(1);
        while (next.IsWeekend()) next = next.AddDays(1);
        return next;
    }

    /// <summary>
    /// Returns the previous calendar workday (Monday–Friday) before the given date.
    /// </summary>
    /// <param name="value">The reference date.</param>
    /// <returns>Previous workday.</returns>
    public static System.DateTime PreviousWorkday(this System.DateTime value)
    {
        var prev = value.Date.AddDays(-1);
        while (prev.IsWeekend()) prev = prev.AddDays(-1);
        return prev;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ISO 8601
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Formats the date/time as an ISO 8601 UTC string (e.g., "2026-02-22T15:30:00Z").
    /// </summary>
    /// <param name="value">The date/time value.</param>
    /// <returns>ISO 8601 formatted string.</returns>
    /// <example>
    /// <code>
    /// DateTime.UtcNow.ToIso8601String(); // "2026-02-22T15:30:00Z"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToIso8601String(this System.DateTime value)
        => value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
}
