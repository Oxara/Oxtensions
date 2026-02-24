// -----------------------------------------------------------------------
// <copyright file="StackExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxtensions.Collections;

/// <summary>
/// Extension methods for <see cref="Stack{T}"/>.
/// </summary>
public static class StackExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the stack is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="stack">The stack to check.</param>
    /// <returns><see langword="true"/> when null or empty.</returns>
    /// <example>
    /// <code>
    /// new Stack&lt;int&gt;().IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this Stack<T>? stack)
        => stack is null || stack.Count == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Safe peek / pop
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the top element of the stack without removing it, or
    /// <paramref name="defaultValue"/> when the stack is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="stack">The stack to peek.</param>
    /// <param name="defaultValue">Value returned when the stack is empty.</param>
    /// <returns>The top element, or <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// new Stack&lt;int&gt;().PeekOrDefault(-1); // -1
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? PeekOrDefault<T>(this Stack<T> stack, T? defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(stack);
        return stack.Count > 0 ? stack.Peek() : defaultValue;
    }

    /// <summary>
    /// Removes and returns the top element of the stack, or
    /// <paramref name="defaultValue"/> when the stack is empty.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="stack">The stack to pop from.</param>
    /// <param name="defaultValue">Value returned when the stack is empty.</param>
    /// <returns>The popped element, or <paramref name="defaultValue"/>.</returns>
    /// <example>
    /// <code>
    /// new Stack&lt;int&gt;().PopOrDefault(-1); // -1
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? PopOrDefault<T>(this Stack<T> stack, T? defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(stack);
        return stack.Count > 0 ? stack.Pop() : defaultValue;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Bulk operations
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Pushes all elements from <paramref name="items"/> onto the stack
    /// in enumeration order (last item ends up on top).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="stack">The target stack.</param>
    /// <param name="items">Elements to push.</param>
    /// <example>
    /// <code>
    /// var s = new Stack&lt;int&gt;();
    /// s.PushRange(new[] { 1, 2, 3 }); // top = 3
    /// </code>
    /// </example>
    public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(stack);
        ArgumentNullException.ThrowIfNull(items);
        foreach (var item in items)
            stack.Push(item);
    }

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the top of the stack.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="stack">The source stack.</param>
    /// <param name="count">Maximum number of elements to pop.</param>
    /// <returns>
    /// An array of popped elements in pop order (index 0 = first popped / was on top).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="count"/> is negative.
    /// </exception>
    /// <example>
    /// <code>
    /// var s = new Stack&lt;int&gt;(new[] { 1, 2, 3 }); // top = 1
    /// s.PopMany(2); // [1, 2]
    /// </code>
    /// </example>
    public static T[] PopMany<T>(this Stack<T> stack, int count)
    {
        ArgumentNullException.ThrowIfNull(stack);
        if (count < 0) throw new ArgumentException("count must be non-negative.", nameof(count));

        int take = Math.Min(count, stack.Count);
        var result = new T[take];
        for (int i = 0; i < take; i++)
            result[i] = stack.Pop();
        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Clone
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a shallow copy of the stack, preserving element order
    /// (same element will be on top of the returned stack).
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="stack">The stack to clone.</param>
    /// <returns>A new <see cref="Stack{T}"/> with the same elements in the same order.</returns>
    /// <example>
    /// <code>
    /// var original = new Stack&lt;int&gt;(new[] { 1, 2, 3 });
    /// var copy = original.Clone(); // independent copy, same top
    /// </code>
    /// </example>
    public static Stack<T> Clone<T>(this Stack<T> stack)
    {
        ArgumentNullException.ThrowIfNull(stack);
        // Stack(IEnumerable<T>) reverses the order, so we reverse first to restore it.
        var arr = stack.ToArray(); // ToArray preserves pop-order (top = index 0)
        System.Array.Reverse(arr);
        return new Stack<T>(arr);
    }
}
