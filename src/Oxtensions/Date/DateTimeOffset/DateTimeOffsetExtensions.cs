// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Date;

/// <summary>
/// High-performance extension methods for <see cref="System.DateTimeOffset"/>.
/// </summary>
public static class DateTimeOffsetExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Unix timestamp
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="System.DateTimeOffset"/> to a Unix timestamp (seconds since epoch).
    /// </summary>
    /// <param name="value">The date/time offset value.</param>
    /// <returns>Seconds since Unix epoch.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToUnixTimestamp(this System.DateTimeOffset value)
        => value.ToUnixTimeSeconds();

    /// <summary>
    /// Converts a Unix timestamp (seconds) to a UTC <see cref="System.DateTimeOffset"/>.
    /// </summary>
    /// <param name="timestamp">Seconds since Unix epoch.</param>
    /// <returns>Corresponding UTC <see cref="System.DateTimeOffset"/>.</returns>
    /// <example>
    /// <code>
    /// 946684800L.ToDateTimeOffset(); // 2000-01-01T00:00:00+00:00
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset ToDateTimeOffset(this long timestamp)
        => System.DateTimeOffset.FromUnixTimeSeconds(timestamp);

    // ─────────────────────────────────────────────────────────────────────────
    // Weekend / Weekday
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a Saturday or Sunday.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when Saturday or Sunday; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWeekend(this System.DateTimeOffset value)
        => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a weekday (Monday–Friday).
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when Monday–Friday; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWeekday(this System.DateTimeOffset value)
        => !value.IsWeekend();

    // ─────────────────────────────────────────────────────────────────────────
    // Start / End boundaries (offset-preserving)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns midnight (00:00:00.000) of the same date, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>Start of day with the same offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset StartOfDay(this System.DateTimeOffset value)
        => new(value.Year, value.Month, value.Day, 0, 0, 0, 0, value.Offset);

    /// <summary>
    /// Returns 23:59:59.999 of the same date, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>End of day with the same offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset EndOfDay(this System.DateTimeOffset value)
        => new(value.Year, value.Month, value.Day, 23, 59, 59, 999, value.Offset);

    /// <summary>
    /// Returns the first day of the month at midnight, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>First moment of the calendar month with the same offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset StartOfMonth(this System.DateTimeOffset value)
        => new(value.Year, value.Month, 1, 0, 0, 0, 0, value.Offset);

    /// <summary>
    /// Returns the last day of the month at 23:59:59.999, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>Last moment of the calendar month with the same offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset EndOfMonth(this System.DateTimeOffset value)
    {
        int lastDay = System.DateTime.DaysInMonth(value.Year, value.Month);
        return new System.DateTimeOffset(value.Year, value.Month, lastDay, 23, 59, 59, 999, value.Offset);
    }

    /// <summary>
    /// Returns January 1st of the same year at midnight, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>First moment of the calendar year with the same offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset StartOfYear(this System.DateTimeOffset value)
        => new(value.Year, 1, 1, 0, 0, 0, 0, value.Offset);

    /// <summary>
    /// Returns December 31st of the same year at 23:59:59.999, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>Last moment of the calendar year with the same offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTimeOffset EndOfYear(this System.DateTimeOffset value)
        => new(value.Year, 12, 31, 23, 59, 59, 999, value.Offset);

    /// <summary>
    /// Returns the start of the week (midnight) for the given value, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <param name="startDay">First day of the week. Defaults to <see cref="DayOfWeek.Monday"/>.</param>
    /// <returns>Start of the containing week with the same offset.</returns>
    public static System.DateTimeOffset StartOfWeek(this System.DateTimeOffset value, DayOfWeek startDay = DayOfWeek.Monday)
    {
        int diff = ((int)value.DayOfWeek - (int)startDay + 7) % 7;
        return value.StartOfDay().AddDays(-diff);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Age
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calculates age in completed years from a birth date to today (local date).
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>Age in full years.</returns>
    public static int Age(this System.DateTimeOffset birthDate)
    {
        var today = System.DateTimeOffset.Now.Date;
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Is Today / Past / Future
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the local date of this value is today.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><see langword="true"/> when today; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsToday(this System.DateTimeOffset value)
        => value.Date == System.DateTime.Today;

    /// <summary>
    /// Returns <see langword="true"/> if the value is in the past relative to <see cref="System.DateTimeOffset.UtcNow"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><see langword="true"/> when past; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPast(this System.DateTimeOffset value)
        => value < System.DateTimeOffset.UtcNow;

    /// <summary>
    /// Returns <see langword="true"/> if the value is in the future relative to <see cref="System.DateTimeOffset.UtcNow"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><see langword="true"/> when future; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFuture(this System.DateTimeOffset value)
        => value > System.DateTimeOffset.UtcNow;

    // ─────────────────────────────────────────────────────────────────────────
    // Next / Previous Workday
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the next calendar workday (Monday–Friday) after the given value, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The reference value.</param>
    /// <returns>Next workday with the same offset.</returns>
    public static System.DateTimeOffset NextWorkday(this System.DateTimeOffset value)
    {
        var next = value.StartOfDay().AddDays(1);
        while (next.IsWeekend()) next = next.AddDays(1);
        return next;
    }

    /// <summary>
    /// Returns the previous calendar workday (Monday–Friday) before the given value, preserving the UTC offset.
    /// </summary>
    /// <param name="value">The reference value.</param>
    /// <returns>Previous workday with the same offset.</returns>
    public static System.DateTimeOffset PreviousWorkday(this System.DateTimeOffset value)
    {
        var prev = value.StartOfDay().AddDays(-1);
        while (prev.IsWeekend()) prev = prev.AddDays(-1);
        return prev;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ISO 8601
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Formats the value as an ISO 8601 string with UTC offset (e.g., "2026-02-22T15:30:00+03:00").
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>ISO 8601 formatted string.</returns>
    /// <example>
    /// <code>
    /// new DateTimeOffset(2026, 2, 22, 15, 0, 0, TimeSpan.FromHours(3)).ToIso8601String();
    /// // "2026-02-22T15:00:00+03:00"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToIso8601String(this System.DateTimeOffset value)
        => value.ToString("yyyy-MM-ddTHH:mm:sszzz");
}
