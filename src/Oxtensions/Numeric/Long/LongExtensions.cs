// -----------------------------------------------------------------------
// <copyright file="LongExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Numeric;

/// <summary>
/// Extension methods for <see cref="long"/>.
/// </summary>
public static class LongExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Sign checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is greater than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(this long value) => value > 0L;

    /// <summary>Returns <see langword="true"/> if the value is less than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(this long value) => value < 0L;

    /// <summary>Returns <see langword="true"/> if the value equals zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this long value) => value == 0L;

    // ─────────────────────────────────────────────────────────────────────────
    // Parity
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is evenly divisible by 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven(this long value) => (value & 1L) == 0L;

    /// <summary>Returns <see langword="true"/> if the value is not evenly divisible by 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOdd(this long value) => (value & 1L) != 0L;

    // ─────────────────────────────────────────────────────────────────────────
    // Range
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Restricts the value to the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="min"/> &gt; <paramref name="max"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Clamp(this long value, long min, long max)
    {
        if (min > max) throw new ArgumentException("min must be less than or equal to max.");
        return Math.Clamp(value, min, max);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the value falls within [<paramref name="min"/>, <paramref name="max"/>] (both inclusive).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this long value, long min, long max) => value >= min && value <= max;

    // ─────────────────────────────────────────────────────────────────────────
    // Math
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns the absolute value.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Abs(this long value) => Math.Abs(value);

    /// <summary>
    /// Returns the number of digits in the absolute value. Zero returns 1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DigitCount(this long value)
        => value == 0L ? 1 : (int)Math.Floor(Math.Log10(Math.Abs((double)value))) + 1;

    /// <summary>
    /// Decomposes the absolute value into its individual digits (most-significant first).
    /// </summary>
    public static int[] ToDigits(this long value)
    {
        if (value == 0L) return [0];
        value = Math.Abs(value);
        int count = value.DigitCount();
        int[] digits = new int[count];
        for (int i = count - 1; i >= 0; i--)
        {
            digits[i] = (int)(value % 10L);
            value /= 10L;
        }
        return digits;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // IsMultipleOf / Pow
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the value is a multiple of <paramref name="divisor"/>.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns><see langword="true"/> when divisible without remainder; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMultipleOf(this long value, long divisor)
        => divisor != 0L && value % divisor == 0L;

    /// <summary>
    /// Raises the value to the specified integer power.
    /// </summary>
    /// <param name="value">The base.</param>
    /// <param name="exponent">The exponent (must be ≥ 0).</param>
    /// <returns>value ^ exponent.</returns>
    /// <exception cref="ArgumentException">Thrown when exponent is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Pow(this long value, int exponent)
    {
        if (exponent < 0) throw new ArgumentException("Exponent must be non-negative.", nameof(exponent));
        return (long)Math.Pow(value, exponent);
    }
}
