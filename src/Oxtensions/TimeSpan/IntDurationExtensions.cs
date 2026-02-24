// -----------------------------------------------------------------------
// <copyright file="IntDurationExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Durations;

/// <summary>
/// Fluent <see cref="TimeSpan"/> builder extensions for <see cref="int"/>.
/// </summary>
public static class IntDurationExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Fluent builders
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of days.</summary>
    /// <example><code>7.Days() // TimeSpan.FromDays(7)</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Days(this int value)
        => TimeSpan.FromDays(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of hours.</summary>
    /// <example><code>2.Hours() // TimeSpan.FromHours(2)</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Hours(this int value)
        => TimeSpan.FromHours(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of minutes.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Minutes(this int value)
        => TimeSpan.FromMinutes(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of seconds.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Seconds(this int value)
        => TimeSpan.FromSeconds(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of milliseconds.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Milliseconds(this int value)
        => TimeSpan.FromMilliseconds(value);
}
