// -----------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Collections;

/// <summary>
/// High-performance extension methods for collections and <see cref="IEnumerable{T}"/>.
/// </summary>
public static class ListExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the collection is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The collection to check.</param>
    /// <returns><see langword="true"/> when null or empty.</returns>
    /// <example>
    /// <code>
    /// new List&lt;int&gt;().IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        if (source is null) return true;
        if (source is ICollection<T> col) return col.Count == 0;
        using var enumerator = source.GetEnumerator();
        return !enumerator.MoveNext();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Chunk
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Splits a list into sub-lists of at most <paramref name="size"/> elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="size">Maximum chunk size. Must be positive.</param>
    /// <returns>Sequence of <see cref="IReadOnlyList{T}"/> chunks.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="size"/> is not positive.</exception>
    /// <example>
    /// <code>
    /// new[] { 1, 2, 3, 4, 5 }.Chunk(2); // [[1,2],[3,4],[5]]
    /// </code>
    /// </example>
    /// <remarks>Uses <see cref="ArraySegment{T}"/> to avoid element-level allocation.</remarks>
    public static IEnumerable<IReadOnlyList<T>> Chunk<T>(this IList<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (size <= 0) throw new ArgumentException("size must be positive.", nameof(size));

        int count = source.Count;
        for (int i = 0; i < count; i += size)
        {
            int chunkSize = Math.Min(size, count - i);
            yield return new ArraySegment<T>(source is T[] arr ? arr : [.. source], i, chunkSize);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Shuffle
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Shuffles the list in-place using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The list to shuffle.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// list.Shuffle();
    /// </code>
    /// </example>
    public static void Shuffle<T>(this IList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        int n = source.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (source[i], source[j]) = (source[j], source[i]);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DistinctBy (pre-.NET 6 compatibility)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns distinct elements from a sequence based on a key selector, preserving first occurrence.
    /// </summary>
    /// <typeparam name="T">Source element type.</typeparam>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">Function to extract the key for comparison.</param>
    /// <returns>Sequence with duplicates removed by key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// people.DistinctBy(p => p.Name);
    /// </code>
    /// </example>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var seen = new HashSet<TKey>();
        foreach (var item in source)
            if (seen.Add(keySelector(item)))
                yield return item;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ForEach
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Executes an action for each element in the sequence.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="action">Action to execute per element.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// items.ForEach(item => Console.WriteLine(item));
    /// </code>
    /// </example>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in source)
            action(item);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToHashSet
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a <see cref="HashSet{T}"/> from the sequence.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A new <see cref="HashSet{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// var set = items.ToHashSet();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new HashSet<T>(source);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Paginate
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a single page from the sequence (zero-based page index).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="pageIndex">Zero-based page number.</param>
    /// <param name="pageSize">Number of elements per page.</param>
    /// <returns>Sequence of elements for the requested page.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when page parameters are negative.</exception>
    /// <example>
    /// <code>
    /// items.Paginate(0, 10); // first 10 items
    /// </code>
    /// </example>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (pageIndex < 0) throw new ArgumentException("pageIndex must be non-negative.", nameof(pageIndex));
        if (pageSize <= 0) throw new ArgumentException("pageSize must be positive.", nameof(pageSize));

        int skip = pageIndex * pageSize;
        int returned = 0;
        int index = 0;

        foreach (var item in source)
        {
            if (index++ < skip) continue;
            if (returned >= pageSize) yield break;
            yield return item;
            returned++;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AddRangeIfNotExists
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds items from <paramref name="items"/> to the list only if they are not already present.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The target list.</param>
    /// <param name="items">Items to conditionally add.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// list.AddRangeIfNotExists(new[] { 1, 2, 3 });
    /// </code>
    /// </example>
    public static void AddRangeIfNotExists<T>(this IList<T> source, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);

        foreach (var item in items)
            if (!source.Contains(item))
                source.Add(item);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RemoveWhere
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Removes all elements from the list that satisfy the given predicate.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The target list.</param>
    /// <param name="predicate">Condition for removal.</param>
    /// <returns>Number of elements removed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// list.RemoveWhere(x => x &lt; 0);
    /// </code>
    /// </example>
    public static int RemoveWhere<T>(this IList<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        int removed = 0;
        for (int i = source.Count - 1; i >= 0; i--)
        {
            if (predicate(source[i]))
            {
                source.RemoveAt(i);
                removed++;
            }
        }
        return removed;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MinBy / MaxBy (pre-.NET 6 compat)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the element with the minimum key value, or default when the sequence is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">Function to extract comparison key.</param>
    /// <returns>Element with minimum key, or default.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static T? MinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        T? minItem = default;
        TKey? minKey = default;
        bool hasValue = false;

        foreach (var item in source)
        {
            TKey key = keySelector(item);
            if (!hasValue || key.CompareTo(minKey!) < 0)
            {
                minItem = item;
                minKey = key;
                hasValue = true;
            }
        }

        return minItem;
    }

    /// <summary>
    /// Returns the element with the maximum key value, or default when the sequence is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">Function to extract comparison key.</param>
    /// <returns>Element with maximum key, or default.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static T? MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        T? maxItem = default;
        TKey? maxKey = default;
        bool hasValue = false;

        foreach (var item in source)
        {
            TKey key = keySelector(item);
            if (!hasValue || key.CompareTo(maxKey!) > 0)
            {
                maxItem = item;
                maxKey = key;
                hasValue = true;
            }
        }

        return maxItem;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Flatten / RandomItem / RandomItems / Rotate / Interleave / CountBy / None / HasDuplicates / Duplicates
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Flattens a sequence of sequences into a single sequence.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">Nested sequences to flatten.</param>
    /// <returns>All elements from all inner sequences.</returns>
    /// <example><code>new[] { new[]{1,2}, new[]{3,4} }.Flatten(); // [1,2,3,4]</code></example>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        foreach (var inner in source)
            foreach (var item in inner)
                yield return item;
    }

    /// <summary>
    /// Returns a single random element from the collection.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source collection (must be non-empty).</param>
    /// <returns>A random element.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection is empty.</exception>
    public static T RandomItem<T>(this IList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Count == 0) throw new ArgumentException("Collection must not be empty.", nameof(source));
        return source[Random.Shared.Next(source.Count)];
    }

    /// <summary>
    /// Returns <paramref name="count"/> distinct random elements from the collection (no replacement).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="count">Number of items to pick.</param>
    /// <returns>Array of <paramref name="count"/> randomly selected elements.</returns>
    /// <exception cref="ArgumentException">Thrown when count exceeds collection size.</exception>
    public static T[] RandomItems<T>(this IList<T> source, int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (count < 0 || count > source.Count)
            throw new ArgumentException("count must be between 0 and source.Count.", nameof(count));

        // Partial Fisher-Yates using a copy
        var copy = new System.Collections.Generic.List<T>(source);
        var result = new T[count];
        for (int i = 0; i < count; i++)
        {
            int j = Random.Shared.Next(i, copy.Count);
            (copy[i], copy[j]) = (copy[j], copy[i]);
            result[i] = copy[i];
        }
        return result;
    }

    /// <summary>
    /// Rotates the list in-place to the left by <paramref name="count"/> positions.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The list to rotate.</param>
    /// <param name="count">Number of positions to rotate left (negative = rotate right).</param>
    /// <example><code>[1,2,3,4,5].Rotate(2); // [3,4,5,1,2]</code></example>
    public static void Rotate<T>(this IList<T> source, int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        int n = source.Count;
        if (n <= 1) return;
        count = ((count % n) + n) % n;  // normalize to [0, n)
        if (count == 0) return;

        // Reverse-algorithm: O(n), in-place
        Reverse(source, 0, count - 1);
        Reverse(source, count, n - 1);
        Reverse(source, 0, n - 1);
    }

    /// <summary>
    /// Interleaves two sequences, alternating elements.
    /// If lengths differ, remaining elements from the longer sequence are appended.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="first">First sequence.</param>
    /// <param name="second">Second sequence.</param>
    /// <returns>Interleaved sequence.</returns>
    /// <example><code>[1,2,3].Interleave([a,b,c]) // [1,a,2,b,3,c]</code></example>
    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        bool has1 = e1.MoveNext(), has2 = e2.MoveNext();
        while (has1 || has2)
        {
            if (has1) { yield return e1.Current; has1 = e1.MoveNext(); }
            if (has2) { yield return e2.Current; has2 = e2.MoveNext(); }
        }
    }

    /// <summary>
    /// Groups elements by key and returns a dictionary with the count of each key.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">Function to extract the grouping key.</param>
    /// <returns>Dictionary mapping each key to its occurrence count.</returns>
    /// <example><code>words.CountBy(w => w.Length); // { 3 => 5, 5 => 2, ... }</code></example>
    public static Dictionary<TKey, int> CountBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var result = new Dictionary<TKey, int>();
        foreach (var item in source)
        {
            var key = keySelector(item);
            result[key] = result.TryGetValue(key, out int c) ? c + 1 : 1;
        }
        return result;
    }

    /// <summary>
    /// Returns <see langword="true"/> when no element satisfies <paramref name="predicate"/>.
    /// When predicate is omitted, returns <see langword="true"/> when the sequence is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">Optional predicate.</param>
    /// <returns><see langword="true"/> when none match; otherwise <see langword="false"/>.</returns>
    /// <example><code>numbers.None(x => x &lt; 0); // true when all are non-negative</code></example>
    public static bool None<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        return predicate is null ? !source.Any() : !source.Any(predicate);
    }

    /// <summary>
    /// Returns <see langword="true"/> if any element appears more than once (by default equality).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns><see langword="true"/> when duplicates exist; otherwise <see langword="false"/>.</returns>
    public static bool HasDuplicates<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var seen = new System.Collections.Generic.HashSet<T>();
        foreach (var item in source)
            if (!seen.Add(item)) return true;
        return false;
    }

    /// <summary>
    /// Returns the elements that appear more than once, identified by <paramref name="keySelector"/>.
    /// Each duplicate element is returned once.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <typeparam name="TKey">Key type for duplicate detection.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">Function to extract the comparison key.</param>
    /// <returns>Sequence of one representative per duplicate group.</returns>
    public static IEnumerable<T> Duplicates<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var seen  = new System.Collections.Generic.HashSet<TKey>();
        var dupes = new System.Collections.Generic.HashSet<TKey>();
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (!seen.Add(key) && dupes.Add(key))
                yield return item;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Private helpers
    // ─────────────────────────────────────────────────────────────────────────

    private static void Reverse<T>(IList<T> list, int lo, int hi)
    {
        while (lo < hi) { (list[lo], list[hi]) = (list[hi], list[lo]); lo++; hi--; }
    }
}
