// -----------------------------------------------------------------------
// <copyright file="ReadOnlyDictionaryExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

namespace Oxtensions.Collections;

/// <summary>
/// Extension methods for <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
/// </summary>
public static class ReadOnlyDictionaryExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Key presence
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if every key in <paramref name="keys"/> is present.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <param name="keys">Keys that must all be present.</param>
    /// <returns><see langword="true"/> when all keys exist; <see langword="false"/> when any key is missing.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// dict.ContainsAllKeys(new[] { "a", "b" }); // true only when both keys exist
    /// </code>
    /// </example>
    public static bool ContainsAllKeys<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> source,
        IEnumerable<TKey> keys)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keys);

        foreach (var key in keys)
            if (!source.ContainsKey(key)) return false;
        return true;
    }

    /// <summary>
    /// Returns <see langword="true"/> if at least one key from <paramref name="keys"/> is present.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <param name="keys">Keys where at least one must be present.</param>
    /// <returns><see langword="true"/> when any key exists; <see langword="false"/> when none exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// dict.ContainsAnyKey(new[] { "x", "y" }); // true when at least one key exists
    /// </code>
    /// </example>
    public static bool ContainsAnyKey<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> source,
        IEnumerable<TKey> keys)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keys);

        foreach (var key in keys)
            if (source.ContainsKey(key)) return true;
        return false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Conversion
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a mutable <see cref="Dictionary{TKey,TValue}"/> copy of the read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <returns>A new mutable <see cref="Dictionary{TKey,TValue}"/> with the same entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// IReadOnlyDictionary&lt;string, int&gt; ro = GetReadOnlyData();
    /// var mutable = ro.ToDictionary();
    /// </code>
    /// </example>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> source)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        return new Dictionary<TKey, TValue>(source);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FilterByKeys
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a new <see cref="Dictionary{TKey,TValue}"/> containing only the entries
    /// whose keys appear in <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <param name="keys">The keys to include.</param>
    /// <returns>Filtered dictionary containing only the requested keys.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// dict.FilterByKeys(new[] { "a", "c" }); // only entries for "a" and "c"
    /// </code>
    /// </example>
    public static Dictionary<TKey, TValue> FilterByKeys<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> source,
        IEnumerable<TKey> keys)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keys);

        var result = new Dictionary<TKey, TValue>();
        foreach (var key in keys)
            if (source.TryGetValue(key, out TValue? value))
                result[key] = value;
        return result;
    }
}
