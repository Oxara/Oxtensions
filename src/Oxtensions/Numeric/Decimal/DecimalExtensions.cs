// -----------------------------------------------------------------------
// <copyright file="DecimalExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Oxtensions.Numeric;

/// <summary>
/// Extension methods for <see cref="decimal"/>.
/// </summary>
public static class DecimalExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Rounding
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Rounds a decimal value to the specified number of decimal places using MidpointRounding.AwayFromZero.
    /// </summary>
    /// <param name="value">The value to round.</param>
    /// <param name="decimals">Number of decimal places.</param>
    /// <returns>Rounded value.</returns>
    /// <example>
    /// <code>
    /// 2.345m.RoundTo(2); // 2.35
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal RoundTo(this decimal value, int decimals)
        => Math.Round(value, decimals, MidpointRounding.AwayFromZero);

    // ─────────────────────────────────────────────────────────────────────────
    // Percentage
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calculates what percentage <paramref name="value"/> is of <paramref name="total"/>.
    /// </summary>
    /// <param name="value">The part value.</param>
    /// <param name="total">The total value.</param>
    /// <returns>Percentage (0–100), or 0 when total is zero.</returns>
    /// <example>
    /// <code>
    /// 25m.Percentage(200m); // 12.5
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Percentage(this decimal value, decimal total)
        => total == 0m ? 0m : value / total * 100m;

    // ─────────────────────────────────────────────────────────────────────────
    // Sign checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is greater than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(this decimal value) => value > 0m;

    /// <summary>Returns <see langword="true"/> if the value is less than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(this decimal value) => value < 0m;

    /// <summary>Returns <see langword="true"/> if the value equals zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this decimal value) => value == 0m;

    // ─────────────────────────────────────────────────────────────────────────
    // Clamp
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Clamps the value to the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>Clamped value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="min"/> &gt; <paramref name="max"/>.</exception>
    /// <example>
    /// <code>
    /// 150m.Clamp(0m, 100m); // 100
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Clamp(this decimal value, decimal min, decimal max)
    {
        if (min > max) throw new ArgumentException("min must be less than or equal to max.");
        return value < min ? min : value > max ? max : value;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Currency string
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Formats the decimal as a currency string using the specified culture.
    /// When no culture is provided, <see cref="CultureInfo.CurrentCulture"/> is used.
    /// </summary>
    /// <param name="value">The monetary value.</param>
    /// <param name="culture">Culture name (e.g., "en-US", "de-DE", "en-GB"). Defaults to <see langword="null"/> (current culture).</param>
    /// <returns>Formatted currency string.</returns>
    /// <example>
    /// <code>
    /// 1234.56m.ToCurrencyString("en-US"); // "$1,234.56"
    /// 1234.56m.ToCurrencyString("de-DE"); // "1.234,56 €"
    /// 1234.56m.ToCurrencyString("en-GB"); // "£1,234.56"
    /// </code>
    /// </example>
    public static string ToCurrencyString(this decimal value, string? culture = null)
    {
        var cultureInfo = culture is null
            ? CultureInfo.CurrentCulture
            : CultureInfo.GetCultureInfo(culture);
        return value.ToString("C", cultureInfo);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Abs / IsBetween / ToNearest
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the absolute value of the decimal.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <returns>Non-negative absolute value.</returns>
    /// <example>
    /// <code>
    /// (-42.5m).Abs(); // 42.5
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Abs(this decimal value) => Math.Abs(value);

    /// <summary>
    /// Returns <see langword="true"/> if the value is within the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <param name="min">Lower bound (inclusive).</param>
    /// <param name="max">Upper bound (inclusive).</param>
    /// <returns><see langword="true"/> when within range; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this decimal value, decimal min, decimal max)
        => value >= min && value <= max;

    /// <summary>
    /// Rounds the value to the nearest multiple of <paramref name="step"/>.
    /// Useful for price rounding (e.g. nearest 0.05, nearest 0.25).
    /// </summary>
    /// <param name="value">The decimal to round.</param>
    /// <param name="step">The rounding granularity (e.g. 0.25m).</param>
    /// <returns>Nearest multiple of <paramref name="step"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="step"/> is zero or negative.</exception>
    /// <example>
    /// <code>
    /// 1.37m.ToNearest(0.25m); // 1.25
    /// 1.63m.ToNearest(0.25m); // 1.75
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToNearest(this decimal value, decimal step)
    {
        if (step <= 0m) throw new ArgumentException("step must be positive.", nameof(step));
        return Math.Round(value / step, MidpointRounding.AwayFromZero) * step;
    }
}
