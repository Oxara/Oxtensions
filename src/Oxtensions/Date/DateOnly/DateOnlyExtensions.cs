// -----------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Date;

/// <summary>
/// High-performance extension methods for <see cref="System.DateOnly"/>.
/// </summary>
public static class DateOnlyExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Weekend / Weekday
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a Saturday or Sunday.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when Saturday or Sunday; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWeekend(this System.DateOnly value)
        => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a weekday (Monday–Friday).
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when Monday–Friday; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWeekday(this System.DateOnly value)
        => !value.IsWeekend();

    // ─────────────────────────────────────────────────────────────────────────
    // Start / End boundaries
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the first day of the month.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>First day of the calendar month.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateOnly StartOfMonth(this System.DateOnly value)
        => new(value.Year, value.Month, 1);

    /// <summary>
    /// Returns the last day of the month.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>Last day of the calendar month.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateOnly EndOfMonth(this System.DateOnly value)
        => new(value.Year, value.Month, System.DateTime.DaysInMonth(value.Year, value.Month));

    /// <summary>
    /// Returns January 1st of the same year.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>First day of the calendar year.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateOnly StartOfYear(this System.DateOnly value)
        => new(value.Year, 1, 1);

    /// <summary>
    /// Returns December 31st of the same year.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <returns>Last day of the calendar year.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateOnly EndOfYear(this System.DateOnly value)
        => new(value.Year, 12, 31);

    /// <summary>
    /// Returns the first day of the week containing this date.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <param name="startDay">First day of the week. Defaults to <see cref="DayOfWeek.Monday"/>.</param>
    /// <returns>Start of the containing week.</returns>
    public static System.DateOnly StartOfWeek(this System.DateOnly value, DayOfWeek startDay = DayOfWeek.Monday)
    {
        int diff = ((int)value.DayOfWeek - (int)startDay + 7) % 7;
        return value.AddDays(-diff);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Conversion
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts this <see cref="System.DateOnly"/> to a <see cref="System.DateTime"/> using the given time component.
    /// </summary>
    /// <param name="value">The source date.</param>
    /// <param name="time">Time of day. Defaults to midnight.</param>
    /// <returns>A <see cref="System.DateTime"/> with <see cref="DateTimeKind.Unspecified"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.DateTime ToDateTime(this System.DateOnly value, System.TimeOnly time = default)
        => value.ToDateTime(time);

    // ─────────────────────────────────────────────────────────────────────────
    // Age
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calculates age in completed years from a birth date to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>Age in full years.</returns>
    public static int Age(this System.DateOnly birthDate)
    {
        var today = System.DateOnly.FromDateTime(System.DateTime.Today);
        int age = today.Year - birthDate.Year;
        if (birthDate.AddYears(age) > today) age--;
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
    public static bool IsToday(this System.DateOnly value)
        => value == System.DateOnly.FromDateTime(System.DateTime.Today);

    /// <summary>
    /// Returns <see langword="true"/> if the date is before today.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when in the past; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPast(this System.DateOnly value)
        => value < System.DateOnly.FromDateTime(System.DateTime.Today);

    /// <summary>
    /// Returns <see langword="true"/> if the date is after today.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <returns><see langword="true"/> when in the future; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFuture(this System.DateOnly value)
        => value > System.DateOnly.FromDateTime(System.DateTime.Today);

    // ─────────────────────────────────────────────────────────────────────────
    // Next / Previous Workday
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the next calendar workday (Monday–Friday) after the given date.
    /// </summary>
    /// <param name="value">The reference date.</param>
    /// <returns>Next workday.</returns>
    public static System.DateOnly NextWorkday(this System.DateOnly value)
    {
        var next = value.AddDays(1);
        while (next.IsWeekend()) next = next.AddDays(1);
        return next;
    }

    /// <summary>
    /// Returns the previous calendar workday (Monday–Friday) before the given date.
    /// </summary>
    /// <param name="value">The reference date.</param>
    /// <returns>Previous workday.</returns>
    public static System.DateOnly PreviousWorkday(this System.DateOnly value)
    {
        var prev = value.AddDays(-1);
        while (prev.IsWeekend()) prev = prev.AddDays(-1);
        return prev;
    }
}
