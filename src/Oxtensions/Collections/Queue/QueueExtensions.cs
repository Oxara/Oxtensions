// -----------------------------------------------------------------------
// <copyright file="QueueExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxtensions.Collections;

/// <summary>
/// Extension methods for <see cref="Queue{T}"/>.
/// </summary>
public static class QueueExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the queue is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="queue">The queue to check.</param>
    /// <returns><see langword="true"/> when null or empty.</returns>
    /// <example>
    /// <code>
    /// new Queue&lt;int&gt;().IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this Queue<T>? queue)
        => queue is null || queue.Count == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Safe peek / dequeue
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the element at the front of the queue without removing it, or
    /// <paramref name="defaultValue"/> when the queue is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="queue">The queue to peek.</param>
    /// <param name="defaultValue">Value returned when the queue is empty.</param>
    /// <returns>The front element, or <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// new Queue&lt;int&gt;().PeekOrDefault(-1); // -1
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? PeekOrDefault<T>(this Queue<T> queue, T? defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(queue);
        return queue.Count > 0 ? queue.Peek() : defaultValue;
    }

    /// <summary>
    /// Removes and returns the front element of the queue, or
    /// <paramref name="defaultValue"/> when the queue is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="queue">The queue to dequeue from.</param>
    /// <param name="defaultValue">Value returned when the queue is empty.</param>
    /// <returns>The dequeued element, or <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// new Queue&lt;int&gt;().DequeueOrDefault(-1); // -1
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? DequeueOrDefault<T>(this Queue<T> queue, T? defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(queue);
        return queue.Count > 0 ? queue.Dequeue() : defaultValue;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Bulk operations
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Enqueues all elements from <paramref name="items"/> in enumeration order.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="queue">The target queue.</param>
    /// <param name="items">Elements to enqueue.</param>
    /// <example>
    /// <code>
    /// var q = new Queue&lt;int&gt;();
    /// q.EnqueueRange(new[] { 1, 2, 3 }); // front = 1
    /// </code>
    /// </example>
    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(items);
        foreach (var item in items)
            queue.Enqueue(item);
    }

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the front of the queue.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="queue">The source queue.</param>
    /// <param name="count">Maximum number of elements to dequeue.</param>
    /// <returns>
    /// An array of dequeued elements in dequeue order (index 0 = was at front).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="count"/> is negative.
    /// </exception>
    /// <example>
    /// <code>
    /// var q = new Queue&lt;int&gt;(new[] { 1, 2, 3, 4 });
    /// q.DequeueMany(2); // [1, 2], queue now has [3, 4]
    /// </code>
    /// </example>
    public static T[] DequeueMany<T>(this Queue<T> queue, int count)
    {
        ArgumentNullException.ThrowIfNull(queue);
        if (count < 0) throw new ArgumentException("count must be non-negative.", nameof(count));

        int take = Math.Min(count, queue.Count);
        var result = new T[take];
        for (int i = 0; i < take; i++)
            result[i] = queue.Dequeue();
        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Clone
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a shallow copy of the queue, preserving element order.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="queue">The queue to clone.</param>
    /// <returns>A new <see cref="Queue{T}"/> with the same elements in the same order.</returns>
    /// <example>
    /// <code>
    /// var original = new Queue&lt;int&gt;(new[] { 1, 2, 3 });
    /// var copy = original.Clone(); // independent copy, same front
    /// </code>
    /// </example>
    public static Queue<T> Clone<T>(this Queue<T> queue)
    {
        ArgumentNullException.ThrowIfNull(queue);
        return new Queue<T>(queue);
    }
}
