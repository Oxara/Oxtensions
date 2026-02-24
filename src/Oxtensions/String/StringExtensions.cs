// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxtensions.String;

/// <summary>
/// High-performance extension methods for <see cref="System.String"/>.
/// </summary>
public static class StringExtensions
{
    // Pre-compiled regex patterns (thread-safe, allocation-free on repeated calls)
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex UrlRegex = new(
        @"^https?://[^\s/$.?#].[^\s]*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex PhoneRegex = new(
        @"^\+?[0-9\s\-\(\)]{7,20}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex HtmlTagRegex = new(
        @"<[^>]*>",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    // Supplemental character map for code points that Unicode NFD cannot decompose
    // (e.g. ı U+0131, ð U+00F0, ø U+00F8, þ U+00FE, ß U+00DF, æ U+00E6)
    private static readonly (char Source, char Target)[] SupplementalCharMap =
    [
        ('ı', 'i'), ('I', 'I'),   // dotless i has no NFD decomposition
        ('ø', 'o'), ('Ø', 'O'),   // Nordic
        ('ð', 'd'), ('Ð', 'D'),   // Icelandic eth
        ('þ', 't'), ('Þ', 'T'),   // Icelandic thorn
        ('ß', 's'),               // German sharp-s
        ('æ', 'e'), ('Æ', 'E'),   // Digraph
    ];

    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the string is <see langword="null"/> or has zero length.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <returns><see langword="true"/> when null or empty; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// bool result = "".IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    /// <remarks>Uses <see cref="MemoryExtensions"/> for zero-allocation span check.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string? value)
        => value is null || value.Length == 0;

    /// <summary>
    /// Returns <see langword="true"/> if the string is <see langword="null"/>, empty, or consists only of whitespace.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <returns><see langword="true"/> when null, empty, or whitespace; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// bool result = "  ".IsNullOrWhiteSpace(); // true
    /// </code>
    /// </example>
    /// <remarks>Uses <see cref="MemoryExtensions.IsWhiteSpace"/> for span-based check without allocation.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace(this string? value)
        => value is null || value.AsSpan().IsWhiteSpace();

    // ─────────────────────────────────────────────────────────────────────────
    // Slug / Casing
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a string to a SEO-friendly URL slug, removing diacritics and replacing spaces/symbols with hyphens.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>Lower-case hyphen-separated slug, or <see cref="System.String.Empty"/> when input is null/whitespace.</returns>
    /// <example>
    /// <code>
    /// "Hello Wörld! Naïve".ToSlug(); // "hello-world-naive"
    /// </code>
    /// </example>
    public static string ToSlug(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;

        var normalized = value!.RemoveAccents();

        var sb = new StringBuilder(normalized.Length);
        bool lastWasHyphen = false;

        foreach (char c in normalized)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToLowerInvariant(c));
                lastWasHyphen = false;
            }
            else if (!lastWasHyphen && sb.Length > 0)
            {
                sb.Append('-');
                lastWasHyphen = true;
            }
        }

        // Trim trailing hyphen
        if (sb.Length > 0 && sb[^1] == '-')
            sb.Remove(sb.Length - 1, 1);

        return sb.ToString();
    }

    /// <summary>
    /// Truncates a string to <paramref name="maxLength"/> characters, appending <paramref name="suffix"/> when truncated.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="maxLength">Maximum allowed character count (including suffix).</param>
    /// <param name="suffix">Suffix appended on truncation. Defaults to "...".</param>
    /// <returns>Original string if within limit; otherwise truncated string with suffix.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="maxLength"/> is negative.</exception>
    /// <example>
    /// <code>
    /// "Hello World".Truncate(7); // "Hell..."
    /// </code>
    /// </example>
    public static string Truncate(this string? value, int maxLength, string suffix = "...")
    {
        if (maxLength < 0) throw new ArgumentException("maxLength must be non-negative.", nameof(maxLength));
        if (value.IsNullOrEmpty()) return string.Empty;
        if (value!.Length <= maxLength) return value;

        int cutLength = maxLength - suffix.Length;
        if (cutLength <= 0) return suffix[..Math.Min(suffix.Length, maxLength)];

        return string.Concat(value.AsSpan(0, cutLength), suffix);
    }

    /// <summary>
    /// Converts a string to Title Case using the current culture.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>Title-cased string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// "hello world".ToTitleCase(); // "Hello World"
    /// </code>
    /// </example>
    public static string ToTitleCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value!.ToLower());
    }

    /// <summary>
    /// Converts a string to PascalCase (UpperCamelCase).
    /// </summary>
    /// <param name="value">The source string (space/underscore/hyphen separated words).</param>
    /// <returns>PascalCase string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// "hello world".ToPascalCase(); // "HelloWorld"
    /// </code>
    /// </example>
    public static string ToPascalCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;
        return BuildWordCase(value!, capitalizeFirst: true, separator: null);
    }

    /// <summary>
    /// Converts a string to camelCase (lowerCamelCase).
    /// </summary>
    /// <param name="value">The source string (space/underscore/hyphen separated words).</param>
    /// <returns>camelCase string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// "hello world".ToCamelCase(); // "helloWorld"
    /// </code>
    /// </example>
    public static string ToCamelCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;
        return BuildWordCase(value!, capitalizeFirst: false, separator: null);
    }

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>snake_case string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// "Hello World".ToSnakeCase(); // "hello_world"
    /// </code>
    /// </example>
    public static string ToSnakeCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;
        return BuildDelimitedCase(value!, '_');
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>kebab-case string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// "Hello World".ToKebabCase(); // "hello-world"
    /// </code>
    /// </example>
    public static string ToKebabCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;
        return BuildDelimitedCase(value!, '-');
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Accent / Character manipulation
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Removes accent marks and diacritics from a string using Unicode NFD normalization,
    /// with supplemental handling for code points that do not decompose (e.g. ø, ð, þ, ß, ı).
    /// Supports all Latin-based scripts.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>ASCII-safe string with accents removed.</returns>
    /// <example>
    /// <code>
    /// "café résumé".RemoveAccents();  // "cafe resume"
    /// "naïve façade".RemoveAccents(); // "naive facade"
    /// </code>
    /// </example>
    public static string RemoveAccents(this string? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;

        // Apply supplemental map for characters without NFD decompositions first
        var sb = new StringBuilder(value!.Length);
        foreach (char c in value)
        {
            char mapped = c;
            foreach (var (src, tgt) in SupplementalCharMap)
                if (c == src) { mapped = tgt; break; }
            sb.Append(mapped);
        }

        // NFD decomposition: splits base letters from their combining marks
        string normalized = sb.ToString().Normalize(NormalizationForm.FormD);
        sb.Clear();
        sb.EnsureCapacity(normalized.Length);

        // Strip all combining (diacritic) marks, keep everything else
        foreach (char c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Determines whether the string contains a specified substring using case-insensitive ordinal comparison.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="substring">The substring to locate.</param>
    /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// "Hello World".ContainsIgnoreCase("WORLD"); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsIgnoreCase(this string? value, string substring)
    {
        if (value is null || substring is null) return false;
        return value.Contains(substring, StringComparison.OrdinalIgnoreCase);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Safe parse
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Parses the string as <see cref="int"/>, returning <paramref name="defaultValue"/> on failure.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="defaultValue">Value returned when parsing fails. Defaults to 0.</param>
    /// <returns>Parsed integer or <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// "42".ToInt32OrDefault();       // 42
    /// "abc".ToInt32OrDefault(-1);    // -1
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32OrDefault(this string? value, int defaultValue = 0)
    {
        if (value.IsNullOrWhiteSpace()) return defaultValue;
        return int.TryParse(value.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Parses the string as <see cref="decimal"/>, returning <paramref name="defaultValue"/> on failure.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="defaultValue">Value returned when parsing fails. Defaults to 0.</param>
    /// <returns>Parsed decimal or <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// "3.14".ToDecimalOrDefault(); // 3.14
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimalOrDefault(this string? value, decimal defaultValue = 0m)
    {
        if (value.IsNullOrWhiteSpace()) return defaultValue;
        return decimal.TryParse(value.AsSpan(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Parses the string as <see cref="Guid"/>, returning <see cref="Guid.Empty"/> on failure.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>Parsed <see cref="Guid"/> or <see cref="Guid.Empty"/>.</returns>
    /// <example>
    /// <code>
    /// "not-a-guid".ToGuidOrDefault(); // Guid.Empty
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid ToGuidOrDefault(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return Guid.Empty;
        return Guid.TryParse(value.AsSpan(), out Guid result) ? result : Guid.Empty;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Masking
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Masks characters between visible regions, useful for credit card or ID number display.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="visibleStart">Number of characters to show at the start.</param>
    /// <param name="visibleEnd">Number of characters to show at the end.</param>
    /// <param name="maskChar">Character used for masking. Defaults to '*'.</param>
    /// <returns>Masked string.</returns>
    /// <exception cref="ArgumentException">Thrown when visible regions overlap or are negative.</exception>
    /// <example>
    /// <code>
    /// "1234567890".Mask(2, 2); // "12******90"
    /// </code>
    /// </example>
    public static string Mask(this string? value, int visibleStart, int visibleEnd, char maskChar = '*')
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        if (visibleStart < 0) throw new ArgumentException("visibleStart must be non-negative.", nameof(visibleStart));
        if (visibleEnd < 0) throw new ArgumentException("visibleEnd must be non-negative.", nameof(visibleEnd));

        int length = value!.Length;
        if (visibleStart + visibleEnd >= length)
            return value;

        int maskLength = length - visibleStart - visibleEnd;
        char[] buffer = ArrayPool<char>.Shared.Rent(length);
        try
        {
            value.AsSpan(0, visibleStart).CopyTo(buffer);
            buffer.AsSpan(visibleStart, maskLength).Fill(maskChar);
            value.AsSpan(length - visibleEnd, visibleEnd).CopyTo(buffer.AsSpan(visibleStart + maskLength));
            return new string(buffer, 0, length);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Validation
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the string is a valid e-mail address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns><see langword="true"/> when valid email format; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// "user@example.com".IsValidEmail(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidEmail(this string? value)
        => !value.IsNullOrWhiteSpace() && EmailRegex.IsMatch(value!);

    /// <summary>
    /// Returns <see langword="true"/> if the string is a valid HTTP/HTTPS URL.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns><see langword="true"/> when valid URL; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// "https://example.com".IsValidUrl(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidUrl(this string? value)
        => !value.IsNullOrWhiteSpace() && UrlRegex.IsMatch(value!);

    /// <summary>
    /// Returns <see langword="true"/> if the string is a valid phone number (7-20 digits, optional +/spaces/dashes).
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns><see langword="true"/> when valid phone; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// "+90 532 000 0000".IsValidPhoneNumber(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidPhoneNumber(this string? value)
        => !value.IsNullOrWhiteSpace() && PhoneRegex.IsMatch(value!);

    // ─────────────────────────────────────────────────────────────────────────
    // Repeat / Reverse
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repeats the string <paramref name="count"/> times.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="count">Number of repetitions. Must be non-negative.</param>
    /// <returns>Concatenated string repeated <paramref name="count"/> times.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="count"/> is negative.</exception>
    /// <example>
    /// <code>
    /// "ab".Repeat(3); // "ababab"
    /// </code>
    /// </example>
    public static string Repeat(this string? value, int count)
    {
        if (count < 0) throw new ArgumentException("count must be non-negative.", nameof(count));
        if (value.IsNullOrEmpty() || count == 0) return string.Empty;
        if (count == 1) return value!;

        return string.Create(value!.Length * count, (value!, count), static (span, state) =>
        {
            var (src, n) = state;
            ReadOnlySpan<char> source = src.AsSpan();
            for (int i = 0; i < n; i++)
                source.CopyTo(span[(i * source.Length)..]);
        });
    }

    /// <summary>
    /// Reverses the order of words in the string.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>String with word order reversed.</returns>
    /// <example>
    /// <code>
    /// "Hello World Foo".ReverseWords(); // "Foo World Hello"
    /// </code>
    /// </example>
    public static string ReverseWords(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;

        var words = value!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Array.Reverse(words);
        return string.Join(' ', words);
    }

    /// <summary>
    /// Reverses all characters in the string.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>Reversed string.</returns>
    /// <example>
    /// <code>
    /// "hello".Reverse(); // "olleh"
    /// </code>
    /// </example>
    public static string Reverse(this string? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        if (value!.Length == 1) return value;

        char[] chars = ArrayPool<char>.Shared.Rent(value.Length);
        try
        {
            value.AsSpan().CopyTo(chars);
            chars.AsSpan(0, value.Length).Reverse();
            return new string(chars, 0, value.Length);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Count / Left / Right
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Counts non-overlapping occurrences of <paramref name="substring"/> within the string.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="substring">The substring to count.</param>
    /// <returns>Number of occurrences, or 0 when not found.</returns>
    /// <example>
    /// <code>
    /// "banana".CountOccurrences("an"); // 2
    /// </code>
    /// </example>
    public static int CountOccurrences(this string? value, string substring)
    {
        if (value.IsNullOrEmpty() || substring.IsNullOrEmpty()) return 0;

        int count = 0;
        int index = 0;
        ReadOnlySpan<char> source = value.AsSpan();
        ReadOnlySpan<char> target = substring.AsSpan();

        while (true)
        {
            int found = source[index..].IndexOf(target, StringComparison.Ordinal);
            if (found < 0) break;
            count++;
            index += found + target.Length;
            if (index >= source.Length) break;
        }

        return count;
    }

    /// <summary>
    /// Returns the leftmost <paramref name="length"/> characters of the string.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="length">Number of characters to take from the left.</param>
    /// <returns>Substring from the left, or the original string if shorter than <paramref name="length"/>.</returns>
    /// <example>
    /// <code>
    /// "Hello World".Left(5); // "Hello"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Left(this string? value, int length)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return length >= value!.Length ? value : value[..length];
    }

    /// <summary>
    /// Returns the rightmost <paramref name="length"/> characters of the string.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="length">Number of characters to take from the right.</param>
    /// <returns>Substring from the right, or the original string if shorter than <paramref name="length"/>.</returns>
    /// <example>
    /// <code>
    /// "Hello World".Right(5); // "World"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Right(this string? value, int length)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return length >= value!.Length ? value : value[^length..];
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HTML / Base64
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Removes all HTML tags from the string, returning plain text content.
    /// </summary>
    /// <param name="value">The HTML string.</param>
    /// <returns>Plain text without HTML tags.</returns>
    /// <example>
    /// <code>
    /// "<b>Hello</b> World".RemoveHtmlTags(); // "Hello World"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RemoveHtmlTags(this string? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return HtmlTagRegex.Replace(value!, string.Empty);
    }

    /// <summary>
    /// Encodes the string to Base64 using UTF-8 encoding.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <returns>Base64-encoded string.</returns>
    /// <example>
    /// <code>
    /// "Hello".ToBase64(); // "SGVsbG8="
    /// </code>
    /// </example>
    public static string ToBase64(this string? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        int maxBytes = Encoding.UTF8.GetMaxByteCount(value!.Length);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(maxBytes);
        try
        {
            int written = Encoding.UTF8.GetBytes(value, buffer);
            return Convert.ToBase64String(buffer, 0, written);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Decodes a Base64 string back to UTF-8 plain text.
    /// </summary>
    /// <param name="value">The Base64-encoded string.</param>
    /// <returns>Decoded UTF-8 string, or <see cref="System.String.Empty"/> on invalid input.</returns>
    /// <example>
    /// <code>
    /// "SGVsbG8=".FromBase64(); // "Hello"
    /// </code>
    /// </example>
    public static string FromBase64(this string? value)
    {
        if (value.IsNullOrWhiteSpace()) return string.Empty;
        try
        {
            byte[] data = Convert.FromBase64String(value!);
            return Encoding.UTF8.GetString(data);
        }
        catch (FormatException)
        {
            return string.Empty;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SplitAndTrim / ContainsAny / IfNullOrEmpty / IfNullOrWhiteSpace / CombineWith
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Splits the string by <paramref name="separator"/> and trims each segment, removing empty entries.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="separator">The character to split on.</param>
    /// <returns>Array of trimmed, non-empty segments.</returns>
    /// <example>
    /// <code>
    /// " a , b , c ".SplitAndTrim(','); // ["a", "b", "c"]
    /// </code>
    /// </example>
    public static string[] SplitAndTrim(this string? value, char separator)
    {
        if (value.IsNullOrWhiteSpace()) return [];
        return value!.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the string contains any of the specified substrings
    /// using the default ordinal string comparison.
    /// </summary>
    /// <param name="value">The string to search in.</param>
    /// <param name="values">Substrings to look for.</param>
    /// <returns><see langword="true"/> when at least one substring is found; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// "hello world".ContainsAny("foo", "world", "bar"); // true
    /// </code>
    /// </example>
    public static bool ContainsAny(this string? value, params string[] values)
    {
        if (value.IsNullOrEmpty() || values is not { Length: > 0 }) return false;
        foreach (var v in values)
            if (value!.Contains(v, StringComparison.Ordinal)) return true;
        return false;
    }

    /// <summary>
    /// Returns <paramref name="fallback"/> when the string is <see langword="null"/> or empty;
    /// otherwise returns the original string.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <param name="fallback">Value to return on null or empty input.</param>
    /// <returns>Original string or fallback.</returns>
    /// <example>
    /// <code>
    /// string? name = null;
    /// name.IfNullOrEmpty("Anonymous"); // "Anonymous"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IfNullOrEmpty(this string? value, string fallback)
        => string.IsNullOrEmpty(value) ? fallback : value;

    /// <summary>
    /// Returns <paramref name="fallback"/> when the string is <see langword="null"/>, empty, or whitespace;
    /// otherwise returns the original string.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <param name="fallback">Value to return on null, empty, or whitespace input.</param>
    /// <returns>Original string or fallback.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IfNullOrWhiteSpace(this string? value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value;

    /// <summary>
    /// Combines the string with additional path segments using <see cref="System.IO.Path.Combine(string[])"/>.
    /// </summary>
    /// <param name="value">The base path.</param>
    /// <param name="parts">Additional path segments.</param>
    /// <returns>Combined path string.</returns>
    /// <example>
    /// <code>
    /// "C:\\base".CombineWith("sub", "file.txt"); // "C:\\base\\sub\\file.txt"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CombineWith(this string value, params string[] parts)
    {
        var all = new string[parts.Length + 1];
        all[0] = value;
        parts.CopyTo(all, 1);
        return System.IO.Path.Combine(all);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Private helpers
    // ─────────────────────────────────────────────────────────────────────────

    private static string BuildWordCase(string value, bool capitalizeFirst, char? separator)
    {
        ReadOnlySpan<char> span = value.AsSpan();
        var sb = new StringBuilder(value.Length);
        bool newWord = true;
        bool isFirst = true;

        foreach (char c in span)
        {
            if (c == ' ' || c == '_' || c == '-')
            {
                newWord = true;
                continue;
            }

            if (newWord)
            {
                if (separator.HasValue && sb.Length > 0) sb.Append(separator.Value);
                bool cap = capitalizeFirst || !isFirst;
                sb.Append(cap ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
                newWord = false;
                isFirst = false;
            }
            else
            {
                sb.Append(char.ToLowerInvariant(c));
            }
        }

        return sb.ToString();
    }

    private static string BuildDelimitedCase(string value, char delimiter)
    {
        var sb = new StringBuilder(value.Length + 4);
        ReadOnlySpan<char> span = value.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];

            if (c == ' ' || c == '_' || c == '-')
            {
                if (sb.Length > 0 && sb[^1] != delimiter)
                    sb.Append(delimiter);
                continue;
            }

            if (char.IsUpper(c) && sb.Length > 0 && sb[^1] != delimiter)
                sb.Append(delimiter);

            sb.Append(char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }
}
