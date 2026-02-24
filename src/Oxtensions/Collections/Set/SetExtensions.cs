// -----------------------------------------------------------------------
// <copyright file="SetExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Collections;

/// <summary>
/// Extension methods for <see cref="ISet{T}"/>.
/// </summary>
public static class SetExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the set is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The set to check.</param>
    /// <returns><see langword="true"/> when null or empty.</returns>
    /// <example>
    /// <code>
    /// new HashSet&lt;int&gt;().IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this ISet<T>? source)
        => source is null || source.Count == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // AddRange
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds all elements from <paramref name="items"/> to the set and returns the count of
    /// newly inserted elements (duplicates are silently skipped).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The target set.</param>
    /// <param name="items">Elements to add.</param>
    /// <returns>Number of elements that were actually added.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// var set = new HashSet&lt;int&gt; { 1, 2 };
    /// set.AddRange(new[] { 2, 3, 4 }); // returns 2 (3 and 4 are new)
    /// </code>
    /// </example>
    public static int AddRange<T>(this ISet<T> source, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);

        int added = 0;
        foreach (var item in items)
            if (source.Add(item)) added++;
        return added;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RemoveWhere
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Removes all elements from the set that satisfy <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The target set.</param>
    /// <param name="predicate">Condition for removal.</param>
    /// <returns>Number of elements removed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// var set = new HashSet&lt;int&gt; { 1, 2, 3, 4 };
    /// set.RemoveWhere(x => x % 2 == 0); // returns 2, set = { 1, 3 }
    /// </code>
    /// </example>
    public static int RemoveWhere<T>(this ISet<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var toRemove = new List<T>();
        foreach (var item in source)
            if (predicate(item)) toRemove.Add(item);

        foreach (var item in toRemove)
            source.Remove(item);

        return toRemove.Count;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToSortedSet
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a new <see cref="SortedSet{T}"/> containing all elements from the set,
    /// ordered using the default comparer for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="source">The source set.</param>
    /// <returns>A new <see cref="SortedSet{T}"/> with the same elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// new HashSet&lt;int&gt; { 3, 1, 2 }.ToSortedSet(); // SortedSet { 1, 2, 3 }
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SortedSet<T> ToSortedSet<T>(this ISet<T> source)
        where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(source);
        return new SortedSet<T>(source);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Overlaps (safe null-guard)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the set shares at least one element with <paramref name="other"/>.
    /// Returns <see langword="false"/> if either collection is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source set.</param>
    /// <param name="other">The sequence to compare.</param>
    /// <returns><see langword="true"/> when at least one element is shared.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// new HashSet&lt;int&gt; { 1, 2, 3 }.OverlapsWith(new[] { 3, 4, 5 }); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OverlapsWith<T>(this ISet<T> source, IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(other);
        return source.Overlaps(other);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SymmetricExcept
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a new <see cref="HashSet{T}"/> containing only the elements that are in
    /// exactly one of the two collections (symmetric difference), without modifying
    /// <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The first set.</param>
    /// <param name="other">The second sequence.</param>
    /// <returns>A new set containing the symmetric difference.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// new HashSet&lt;int&gt; { 1, 2, 3 }.SymmetricDifference(new[] { 2, 3, 4 });
    /// // HashSet { 1, 4 }
    /// </code>
    /// </example>
    public static HashSet<T> SymmetricDifference<T>(this ISet<T> source, IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(other);

        var result = new HashSet<T>(source);
        result.SymmetricExceptWith(other);
        return result;
    }
}
