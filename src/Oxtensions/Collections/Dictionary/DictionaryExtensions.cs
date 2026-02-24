// -----------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace Oxtensions.Collections;

/// <summary>
/// High-performance extension methods for <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the dictionary is <see langword="null"/> or contains no entries.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The dictionary to evaluate.</param>
    /// <returns><see langword="true"/> when null or empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue>? source)
        => source is null || source.Count == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // GetOrDefault
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the value associated with <paramref name="key"/>, or <paramref name="defaultValue"/> when not found.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <param name="key">The key to look up.</param>
    /// <param name="defaultValue">Fallback value when key is absent.</param>
    /// <returns>Found value or default.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// dict.GetOrDefault("missing", "fallback");
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue? GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        TKey key,
        TValue? defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetOrAdd
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the value for <paramref name="key"/>, or adds and returns the result of <paramref name="valueFactory"/> when absent.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The target dictionary.</param>
    /// <param name="key">The key to look up or add.</param>
    /// <param name="valueFactory">Factory called with the key to produce the new value.</param>
    /// <returns>Existing or newly added value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// dict.GetOrAdd("key", k => new List&lt;int&gt;());
    /// </code>
    /// </example>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        TKey key,
        Func<TKey, TValue> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(valueFactory);

        if (source.TryGetValue(key, out TValue? existing))
            return existing;

        TValue newValue = valueFactory(key);
        source[key] = newValue;
        return newValue;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Merge
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Merges entries from <paramref name="other"/> into the current dictionary.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The target dictionary.</param>
    /// <param name="other">The dictionary to merge from.</param>
    /// <param name="overwrite">When <see langword="true"/>, existing keys are overwritten. Defaults to <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// dict.Merge(otherDict, overwrite: true);
    /// </code>
    /// </example>
    public static void Merge<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        IDictionary<TKey, TValue> other,
        bool overwrite = false)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(other);

        foreach (var kvp in other)
        {
            if (overwrite || !source.ContainsKey(kvp.Key))
                source[kvp.Key] = kvp.Value;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToQueryString
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a dictionary to a URL query string (e.g., "key1=val1&amp;key2=val2").
    /// Keys and values are URL-encoded.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <returns>URL-encoded query string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// new Dictionary&lt;string, string&gt; { ["q"] = "hello world" }.ToQueryString();
    /// // "q=hello+world"
    /// </code>
    /// </example>
    public static string ToQueryString<TKey, TValue>(this IDictionary<TKey, TValue> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Count == 0) return string.Empty;

        var sb = new StringBuilder(source.Count * 16);
        bool first = true;

        foreach (var kvp in source)
        {
            if (!first) sb.Append('&');
            sb.Append(Uri.EscapeDataString(kvp.Key?.ToString() ?? string.Empty));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kvp.Value?.ToString() ?? string.Empty));
            first = false;
        }

        return sb.ToString();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Invert
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a new dictionary with keys and values swapped.
    /// </summary>
    /// <typeparam name="TKey">Original key type (becomes new value).</typeparam>
    /// <typeparam name="TValue">Original value type (becomes new key).</typeparam>
    /// <param name="source">The source dictionary.</param>
    /// <returns>Inverted dictionary.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when duplicate values exist (would create duplicate keys).</exception>
    /// <example>
    /// <code>
    /// new Dictionary&lt;string, int&gt; { ["a"] = 1 }.Invert(); // { 1 => "a" }
    /// </code>
    /// </example>
    public static IDictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> source)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new Dictionary<TValue, TKey>(source.Count);
        foreach (var kvp in source)
            result.Add(kvp.Value, kvp.Key);  // throws on duplicate values intentionally

        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AddOrUpdate / RemoveWhere / ToJson
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds the key-value pair if the key does not exist, or updates the value if it does.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The target dictionary.</param>
    /// <param name="key">The key to add or update.</param>
    /// <param name="value">The value to set.</param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        source[key] = value;
    }

    /// <summary>
    /// Removes all entries from the dictionary that satisfy <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The target dictionary.</param>
    /// <param name="predicate">Function that returns <see langword="true"/> for entries to remove.</param>
    /// <returns>Number of entries removed.</returns>
    /// <example>
    /// <code>
    /// dict.RemoveWhere((k, v) => v == null);
    /// </code>
    /// </example>
    public static int RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> source, Func<TKey, TValue, bool> predicate)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var keysToRemove = new System.Collections.Generic.List<TKey>();
        foreach (var kvp in source)
            if (predicate(kvp.Key, kvp.Value)) keysToRemove.Add(kvp.Key);

        foreach (var key in keysToRemove)
            source.Remove(key);

        return keysToRemove.Count;
    }

    /// <summary>
    /// Serializes the dictionary to a JSON string.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="source">The dictionary to serialize.</param>
    /// <returns>JSON representation.</returns>
    public static string ToJson<TKey, TValue>(this IDictionary<TKey, TValue> source)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        return System.Text.Json.JsonSerializer.Serialize(source);
    }
}
