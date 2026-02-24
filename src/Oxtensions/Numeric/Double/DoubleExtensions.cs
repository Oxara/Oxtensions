// -----------------------------------------------------------------------
// <copyright file="DoubleExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Numeric;

/// <summary>
/// Extension methods for <see cref="double"/>.
/// </summary>
public static class DoubleExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Special value checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is <see cref="double.NaN"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN(this double value) => double.IsNaN(value);

    /// <summary>Returns <see langword="true"/> if the value is positive or negative infinity.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinity(this double value) => double.IsInfinity(value);

    /// <summary>Returns <see langword="true"/> if the value is a finite, non-NaN number.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(this double value) => double.IsFinite(value);

    // ─────────────────────────────────────────────────────────────────────────
    // Sign checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is greater than zero (and finite).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(this double value) => value > 0d && double.IsFinite(value);

    /// <summary>Returns <see langword="true"/> if the value is less than zero (and finite).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(this double value) => value < 0d && double.IsFinite(value);

    /// <summary>Returns <see langword="true"/> if the value equals zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this double value) => value == 0d;

    // ─────────────────────────────────────────────────────────────────────────
    // Range
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Restricts the value to the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="min"/> &gt; <paramref name="max"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(this double value, double min, double max)
    {
        if (min > max) throw new ArgumentException("min must be less than or equal to max.");
        return Math.Clamp(value, min, max);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the value falls within [<paramref name="min"/>, <paramref name="max"/>] (both inclusive).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this double value, double min, double max) => value >= min && value <= max;

    // ─────────────────────────────────────────────────────────────────────────
    // Math
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns the absolute value.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Abs(this double value) => Math.Abs(value);

    /// <summary>
    /// Rounds the value to the specified number of decimal places using MidpointRounding.AwayFromZero.
    /// </summary>
    /// <example><code>2.345.RoundTo(2); // 2.35</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RoundTo(this double value, int decimals)
        => Math.Round(value, decimals, MidpointRounding.AwayFromZero);

    // ─────────────────────────────────────────────────────────────────────────
    // Lerp / Normalize / ToRadians / ToDegrees
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Linearly interpolates between <paramref name="a"/> and <paramref name="b"/> using this value as the factor t.
    /// When t = 0 returns a; when t = 1 returns b.
    /// </summary>
    /// <param name="t">Interpolation factor (0 = start, 1 = end; unclamped).</param>
    /// <param name="a">Start value.</param>
    /// <param name="b">End value.</param>
    /// <returns>Interpolated value.</returns>
    /// <example><code>0.5.Lerp(0, 10); // 5.0</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Lerp(this double t, double a, double b)
        => a + (b - a) * t;

    /// <summary>
    /// Normalizes this value from the range [<paramref name="min"/>, <paramref name="max"/>] to [0, 1].
    /// Returns 0 when min equals max.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <param name="min">Range minimum.</param>
    /// <param name="max">Range maximum.</param>
    /// <returns>Normalized value in [0, 1].</returns>
    /// <example><code>5.0.Normalize(0, 10); // 0.5</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Normalize(this double value, double min, double max)
        => max == min ? 0d : (value - min) / (max - min);

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Angle in degrees.</param>
    /// <returns>Angle in radians.</returns>
    /// <example><code>180.0.ToRadians(); // Math.PI</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToRadians(this double degrees)
        => degrees * (Math.PI / 180.0);

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="radians">Angle in radians.</param>
    /// <returns>Angle in degrees.</returns>
    /// <example><code>Math.PI.ToDegrees(); // 180.0</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDegrees(this double radians)
        => radians * (180.0 / Math.PI);
}
