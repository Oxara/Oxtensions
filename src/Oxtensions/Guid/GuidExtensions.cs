// -----------------------------------------------------------------------
// <copyright file="GuidExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Identifiers;

/// <summary>
/// High-performance extension methods for <see cref="Guid"/> and <see cref="System.Nullable{Guid}"/>.
/// </summary>
public static class GuidExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the GUID equals <see cref="Guid.Empty"/>.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <returns><see langword="true"/> when empty; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// Guid.Empty.IsEmpty(); // true
    /// Guid.NewGuid().IsEmpty(); // false
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Guid value)
        => value == Guid.Empty;

    /// <summary>
    /// Returns <see langword="true"/> if the nullable GUID is <see langword="null"/> or equals <see cref="Guid.Empty"/>.
    /// </summary>
    /// <param name="value">The nullable GUID to check.</param>
    /// <returns><see langword="true"/> when null or empty; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this Guid? value)
        => value is null || value.Value == Guid.Empty;

    // ─────────────────────────────────────────────────────────────────────────
    // Formatting
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts the GUID to a 22-character URL-safe Base64 string (no padding, no dashes).
    /// Useful for compact ID representation in URLs and databases.
    /// </summary>
    /// <param name="value">The source GUID.</param>
    /// <returns>22-character Base64URL string.</returns>
    /// <example>
    /// <code>
    /// var id = Guid.NewGuid().ToShortString(); // e.g. "3q2-7wEjNkiprFZARm8Gyg"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToShortString(this Guid value)
        => Convert.ToBase64String(value.ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .TrimEnd('=');

    // ─────────────────────────────────────────────────────────────────────────
    // Factory
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new time-ordered GUID (COMB) that sorts chronologically.
    /// The high bytes are replaced with the current UTC timestamp, making it
    /// friendly for clustered database indexes.
    /// </summary>
    /// <returns>A time-ordered <see cref="Guid"/>.</returns>
    /// <example>
    /// <code>
    /// var id = GuidExtensions.NewComb(); // always increasing, good for SQL Server clustered index
    /// </code>
    /// </example>
    public static Guid NewComb()
    {
        var guidBytes = Guid.NewGuid().ToByteArray();
        var now = System.DateTime.UtcNow;
        var timeBytes = BitConverter.GetBytes(now.Ticks);

        // Place the 6 most-significant time bytes at positions 10–15 (SQL Server NEWSEQUENTIALID pattern)
        guidBytes[10] = timeBytes[7];
        guidBytes[11] = timeBytes[6];
        guidBytes[12] = timeBytes[5];
        guidBytes[13] = timeBytes[4];
        guidBytes[14] = timeBytes[3];
        guidBytes[15] = timeBytes[2];

        return new Guid(guidBytes);
    }
}
