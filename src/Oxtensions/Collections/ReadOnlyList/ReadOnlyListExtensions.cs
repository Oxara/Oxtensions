// -----------------------------------------------------------------------
// <copyright file="ReadOnlyListExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Collections;

/// <summary>
/// Extension methods for <see cref="IReadOnlyList{T}"/>.
/// </summary>
public static class ReadOnlyListExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the list is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The list to check.</param>
    /// <returns><see langword="true"/> when null or empty.</returns>
    /// <example>
    /// <code>
    /// Array.Empty&lt;int&gt;().IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IReadOnlyList<T>? source)
        => source is null || source.Count == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Index search
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the zero-based index of the first occurrence of <paramref name="item"/>,
    /// or <c>-1</c> when not found.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The list to search.</param>
    /// <param name="item">The value to find.</param>
    /// <returns>Zero-based index of the first match, or <c>-1</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// new int[] { 10, 20, 30 }.IndexOf(20); // 1
    /// </code>
    /// </example>
    public static int IndexOf<T>(this IReadOnlyList<T> source, T item)
    {
        ArgumentNullException.ThrowIfNull(source);

        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < source.Count; i++)
            if (comparer.Equals(source[i], item)) return i;
        return -1;
    }

    /// <summary>
    /// Returns the zero-based index of the last occurrence of <paramref name="item"/>,
    /// or <c>-1</c> when not found.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The list to search.</param>
    /// <param name="item">The value to find.</param>
    /// <returns>Zero-based index of the last match, or <c>-1</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// new int[] { 10, 20, 10 }.LastIndexOf(10); // 2
    /// </code>
    /// </example>
    public static int LastIndexOf<T>(this IReadOnlyList<T> source, T item)
    {
        ArgumentNullException.ThrowIfNull(source);

        var comparer = EqualityComparer<T>.Default;
        for (int i = source.Count - 1; i >= 0; i--)
            if (comparer.Equals(source[i], item)) return i;
        return -1;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BinarySearch
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Performs a binary search on a <em>sorted</em> list for <paramref name="value"/>.
    /// Returns the zero-based index of the match, or the bitwise complement
    /// (<c>~insertionPoint</c>) of the index where the value would be inserted if not found.
    /// </summary>
    /// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="source">The sorted list to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>Non-negative index on match; negative bitwise complement otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// new int[] { 1, 3, 5, 7, 9 }.BinarySearch(5);  // 2
    /// new int[] { 1, 3, 5, 7, 9 }.BinarySearch(4);  // ~2 = -3  (would insert at index 2)
    /// </code>
    /// </example>
    public static int BinarySearch<T>(this IReadOnlyList<T> source, T value)
        where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(source);

        int lo = 0, hi = source.Count - 1;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            int cmp = source[mid].CompareTo(value);
            if (cmp == 0) return mid;
            if (cmp < 0) lo = mid + 1;
            else hi = mid - 1;
        }
        return ~lo;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Slice
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a read-only view of a contiguous range of elements starting at
    /// <paramref name="start"/> with length <paramref name="length"/>.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="start">Zero-based starting index.</param>
    /// <param name="length">Number of elements to include.</param>
    /// <returns>A new <see cref="IReadOnlyList{T}"/> covering the requested range.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="start"/> or <paramref name="length"/> are out of range.</exception>
    /// <example>
    /// <code>
    /// new int[] { 1, 2, 3, 4, 5 }.Slice(1, 3); // [2, 3, 4]
    /// </code>
    /// </example>
    public static IReadOnlyList<T> Slice<T>(this IReadOnlyList<T> source, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (start < 0 || start > source.Count) throw new ArgumentException("start is out of range.", nameof(start));
        if (length < 0 || start + length > source.Count) throw new ArgumentException("length is out of range.", nameof(length));

        var result = new T[length];
        for (int i = 0; i < length; i++)
            result[i] = source[start + i];
        return result;
    }

}
