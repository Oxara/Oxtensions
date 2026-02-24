// -----------------------------------------------------------------------
// <copyright file="LongDurationExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Durations;

/// <summary>
/// Fluent <see cref="TimeSpan"/> builder extensions for <see cref="long"/>.
/// </summary>
public static class LongDurationExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Fluent builders
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of days.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Days(this long value)
        => TimeSpan.FromDays(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of hours.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Hours(this long value)
        => TimeSpan.FromHours(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of minutes.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Minutes(this long value)
        => TimeSpan.FromMinutes(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of seconds.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Seconds(this long value)
        => TimeSpan.FromSeconds(value);

    /// <summary>Returns a <see cref="TimeSpan"/> of the specified number of milliseconds.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Milliseconds(this long value)
        => TimeSpan.FromMilliseconds(value);
}
