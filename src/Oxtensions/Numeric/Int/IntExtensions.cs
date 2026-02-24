// -----------------------------------------------------------------------
// <copyright file="IntExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Numeric;

/// <summary>
/// Extension methods for <see cref="int"/>.
/// </summary>
public static class IntExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Sign checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is greater than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(this int value) => value > 0;

    /// <summary>Returns <see langword="true"/> if the value is less than zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(this int value) => value < 0;

    /// <summary>Returns <see langword="true"/> if the value equals zero.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this int value) => value == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Parity
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> if the value is evenly divisible by 2.</summary>
    /// <example><code>4.IsEven(); // true</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven(this int value) => (value & 1) == 0;

    /// <summary>Returns <see langword="true"/> if the value is not evenly divisible by 2.</summary>
    /// <example><code>3.IsOdd(); // true</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOdd(this int value) => (value & 1) != 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Range
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Restricts the value to the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="min"/> &gt; <paramref name="max"/>.</exception>
    /// <example><code>150.Clamp(0, 100); // 100</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min, int max)
    {
        if (min > max) throw new ArgumentException("min must be less than or equal to max.");
        return Math.Clamp(value, min, max);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the value falls within [<paramref name="min"/>, <paramref name="max"/>] (both inclusive).
    /// </summary>
    /// <example><code>5.IsBetween(1, 10); // true</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this int value, int min, int max) => value >= min && value <= max;

    // ─────────────────────────────────────────────────────────────────────────
    // Math
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns the absolute value.</summary>
    /// <example><code>(-7).Abs(); // 7</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(this int value) => Math.Abs(value);

    /// <summary>
    /// Returns <see langword="true"/> if the value is a prime number.
    /// Values less than 2 are not prime.
    /// </summary>
    /// <example><code>7.IsPrime(); // true</code></example>
    public static bool IsPrime(this int value)
    {
        if (value < 2) return false;
        if (value == 2) return true;
        if ((value & 1) == 0) return false;
        int limit = (int)Math.Sqrt(value);
        for (int i = 3; i <= limit; i += 2)
            if (value % i == 0) return false;
        return true;
    }

    /// <summary>
    /// Returns the factorial of the value as a <see cref="long"/> (n!).
    /// </summary>
    /// <param name="value">Non-negative integer ≤ 20.</param>
    /// <returns>n!</returns>
    /// <exception cref="ArgumentException">Thrown when value is negative or greater than 20.</exception>
    /// <example><code>5.Factorial(); // 120</code></example>
    public static long Factorial(this int value)
    {
        if (value < 0 || value > 20)
            throw new ArgumentException("Value must be in range [0, 20].", nameof(value));
        long result = 1;
        for (int i = 2; i <= value; i++)
            result *= i;
        return result;
    }

    /// <summary>
    /// Returns the number of digits in the absolute value of the integer.
    /// Zero returns 1.
    /// </summary>
    /// <example><code>(-1234).DigitCount(); // 4</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DigitCount(this int value)
        => value == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs((double)value))) + 1;

    /// <summary>
    /// Decomposes the absolute value of the integer into its individual digits (most-significant first).
    /// </summary>
    /// <example><code>1234.ToDigits(); // [1, 2, 3, 4]</code></example>
    public static int[] ToDigits(this int value)
    {
        if (value == 0) return [0];
        value = Math.Abs(value);
        int count = value.DigitCount();
        int[] digits = new int[count];
        for (int i = count - 1; i >= 0; i--)
        {
            digits[i] = value % 10;
            value /= 10;
        }
        return digits;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToOrdinal / ToRoman / IsMultipleOf / Pow
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the ordinal string representation of the integer (e.g. 1st, 2nd, 3rd).
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <returns>Ordinal string.</returns>
    /// <example><code>1.ToOrdinal(); // "1st"  //  12.ToOrdinal(); // "12th"</code></example>
    public static string ToOrdinal(this int value)
    {
        int abs = Math.Abs(value);
        string suffix = (abs % 100) switch
        {
            11 or 12 or 13 => "th",
            _ => (abs % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            }
        };
        return $"{value}{suffix}";
    }

    /// <summary>
    /// Converts a positive integer (1–3999) to its Roman numeral representation.
    /// </summary>
    /// <param name="value">The integer to convert. Must be between 1 and 3999.</param>
    /// <returns>Roman numeral string (e.g. "XIV").</returns>
    /// <exception cref="ArgumentException">Thrown when value is outside 1–3999.</exception>
    /// <example><code>14.ToRoman(); // "XIV"  //  1994.ToRoman(); // "MCMXCIV"</code></example>
    public static string ToRoman(this int value)
    {
        if (value < 1 || value > 3999)
            throw new ArgumentException("Value must be between 1 and 3999.", nameof(value));

        ReadOnlySpan<int>    numerals = [1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1];
        ReadOnlySpan<string> symbols  = ["M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"];

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < numerals.Length; i++)
            while (value >= numerals[i]) { sb.Append(symbols[i]); value -= numerals[i]; }
        return sb.ToString();
    }

    /// <summary>
    /// Returns <see langword="true"/> if the value is a multiple of <paramref name="divisor"/>.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns><see langword="true"/> when divisible without remainder; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMultipleOf(this int value, int divisor)
        => divisor != 0 && value % divisor == 0;

    /// <summary>
    /// Raises the value to the specified integer power.
    /// </summary>
    /// <param name="value">The base.</param>
    /// <param name="exponent">The exponent (must be ≥ 0).</param>
    /// <returns>value ^ exponent.</returns>
    /// <exception cref="ArgumentException">Thrown when exponent is negative.</exception>
    /// <example><code>3.Pow(4); // 81</code></example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Pow(this int value, int exponent)
    {
        if (exponent < 0) throw new ArgumentException("Exponent must be non-negative.", nameof(exponent));
        return (int)Math.Pow(value, exponent);
    }
}
