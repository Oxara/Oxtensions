// -----------------------------------------------------------------------
// <copyright file="CharExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Char;

/// <summary>
/// High-performance extension methods for <see cref="char"/>.
/// </summary>
public static class CharExtensions
{
    private static readonly System.Collections.Generic.HashSet<char> EnglishVowels =
        new() { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };

    // ─────────────────────────────────────────────────────────────────────────
    // Vowel / Consonant
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the character is an English vowel (a, e, i, o, u — case-insensitive).
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when a vowel; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// 'a'.IsVowel(); // true
    /// 'B'.IsVowel(); // false
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVowel(this char value)
        => EnglishVowels.Contains(value);

    /// <summary>
    /// Returns <see langword="true"/> if the character is an ASCII letter that is not a vowel.
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when a consonant; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsConsonant(this char value)
        => char.IsLetter(value) && !value.IsVowel();

    // ─────────────────────────────────────────────────────────────────────────
    // ASCII helpers
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the character is an ASCII letter (a–z, A–Z).
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when an ASCII letter; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetter(this char value)
        => (value >= 'a' && value <= 'z') || (value >= 'A' && value <= 'Z');

    /// <summary>
    /// Returns <see langword="true"/> if the character is an ASCII decimal digit (0–9).
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when 0–9; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(this char value)
        => value >= '0' && value <= '9';

    /// <summary>
    /// Returns <see langword="true"/> if the character is an ASCII letter or decimal digit.
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when ASCII alphanumeric; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterOrDigit(this char value)
        => value.IsAsciiLetter() || value.IsAsciiDigit();

    // ─────────────────────────────────────────────────────────────────────────
    // Whitespace / Newline
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the character is a whitespace character
    /// (space, tab, newline, carriage return, etc.).
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when whitespace; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhitespace(this char value)
        => char.IsWhiteSpace(value);

    /// <summary>
    /// Returns <see langword="true"/> if the character is a newline character
    /// (<c>'\n'</c> or <c>'\r'</c>).
    /// </summary>
    /// <param name="value">The character to check.</param>
    /// <returns><see langword="true"/> when newline; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNewLine(this char value)
        => value is '\n' or '\r';

    // ─────────────────────────────────────────────────────────────────────────
    // Case
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the character is an uppercase letter.
    /// </summary>
    /// <param name="value">The character to check.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUppercase(this char value)
        => char.IsUpper(value);

    /// <summary>
    /// Returns <see langword="true"/> if the character is a lowercase letter.
    /// </summary>
    /// <param name="value">The character to check.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLowercase(this char value)
        => char.IsLower(value);
}
