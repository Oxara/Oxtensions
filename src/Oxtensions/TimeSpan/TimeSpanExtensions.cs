// -----------------------------------------------------------------------
// <copyright file="TimeSpanExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;
using System.Text;

namespace Oxtensions.Durations;

/// <summary>
/// High-performance extension methods for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Sign checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the duration is greater than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(this TimeSpan value)
        => value > TimeSpan.Zero;

    /// <summary>Returns <see langword="true"/> if the duration is less than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(this TimeSpan value)
        => value < TimeSpan.Zero;

    /// <summary>Returns <see langword="true"/> if the duration equals zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this TimeSpan value)
        => value == TimeSpan.Zero;

    // ─────────────────────────────────────────────────────────────────────────
    // Math
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the absolute (non-negative) value of the duration.
    /// </summary>
    /// <param name="value">The duration.</param>
    /// <returns>Absolute duration.</returns>
    /// <example>
    /// <code>
    /// TimeSpan.FromHours(-2).Abs(); // 02:00:00
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Abs(this TimeSpan value)
        => value < TimeSpan.Zero ? value.Negate() : value;

    /// <summary>
    /// Returns the total number of complete weeks in the duration.
    /// </summary>
    /// <param name="value">The duration.</param>
    /// <returns>Total weeks as a double.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double TotalWeeks(this TimeSpan value)
        => value.TotalDays / 7.0;

    // ─────────────────────────────────────────────────────────────────────────
    // Formatting
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Formats the duration as a human-readable string, e.g. "2h 30m 15s" or "45m 0s".
    /// Days are included only when non-zero.
    /// </summary>
    /// <param name="value">The duration to format.</param>
    /// <returns>Human-readable duration string.</returns>
    /// <example>
    /// <code>
    /// TimeSpan.FromMinutes(150).ToHumanReadable(); // "2h 30m 0s"
    /// TimeSpan.FromDays(1.5).ToHumanReadable();    // "1d 12h 0m 0s"
    /// </code>
    /// </example>
    public static string ToHumanReadable(this TimeSpan value)
    {
        var abs = value.Abs();
        var sb = new StringBuilder();
        if (value.IsNegative()) sb.Append('-');
        if (abs.Days > 0) sb.Append(abs.Days).Append("d ");
        if (abs.Days > 0 || abs.Hours > 0) sb.Append(abs.Hours).Append("h ");
        if (abs.Days > 0 || abs.Hours > 0 || abs.Minutes > 0) sb.Append(abs.Minutes).Append("m ");
        sb.Append(abs.Seconds).Append('s');
        return sb.ToString();
    }

    /// <summary>
    /// Formats the duration as an ISO 8601 duration string (e.g. "PT2H30M15S").
    /// </summary>
    /// <param name="value">The duration to format.</param>
    /// <returns>ISO 8601 duration string.</returns>
    /// <example>
    /// <code>
    /// TimeSpan.FromMinutes(150).ToIso8601Duration(); // "PT2H30M0S"
    /// TimeSpan.FromDays(2).ToIso8601Duration();      // "P2DT0H0M0S"
    /// </code>
    /// </example>
    public static string ToIso8601Duration(this TimeSpan value)
    {
        var abs = value.Abs();
        var sb = new StringBuilder();
        if (value.IsNegative()) sb.Append('-');
        sb.Append('P');
        if (abs.Days > 0) sb.Append(abs.Days).Append('D');
        sb.Append('T');
        sb.Append(abs.Hours).Append('H');
        sb.Append(abs.Minutes).Append('M');
        sb.Append(abs.Seconds).Append('S');
        return sb.ToString();
    }
}
