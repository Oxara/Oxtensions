// -----------------------------------------------------------------------
// <copyright file="GenericExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Oxtensions.Generic;

/// <summary>
/// High-performance extension methods for generic types.
/// </summary>
public static class GenericExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    // ─────────────────────────────────────────────────────────────────────────
    // Null checks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the object is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="obj">The object to test.</param>
    /// <returns><see langword="true"/> when null.</returns>
    /// <example>
    /// <code>
    /// ((string?)null).IsNull(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<T>(this T? obj) => obj is null;

    /// <summary>
    /// Returns <see langword="true"/> if the object is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="obj">The object to test.</param>
    /// <returns><see langword="true"/> when not null.</returns>
    /// <example>
    /// <code>
    /// "hello".IsNotNull(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull<T>(this T? obj) => obj is not null;

    // ─────────────────────────────────────────────────────────────────────────
    // ThrowIfNull
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> if the object is null; otherwise returns it.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="obj">The object to guard.</param>
    /// <param name="paramName">Parameter name for the exception message.</param>
    /// <returns>The non-null object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <example>
    /// <code>
    /// var name = input.ThrowIfNull(nameof(input));
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T>(this T? obj, string paramName)
    {
        ArgumentNullException.ThrowIfNull(obj, paramName);
        return obj;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // In
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the value is equal to any of the given candidates (SQL IN() equivalent).
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="values">Candidate values to match against.</param>
    /// <returns><see langword="true"/> when any candidate matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    /// <example>
    /// <code>
    /// 5.In(1, 3, 5, 7); // true
    /// "a".In("b", "c"); // false
    /// </code>
    /// </example>
    public static bool In<T>(this T value, params T[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var comparer = EqualityComparer<T>.Default;
        foreach (var candidate in values)
            if (comparer.Equals(value, candidate)) return true;

        return false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Clone
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a deep copy of the object using JSON serialization/deserialization.
    /// </summary>
    /// <typeparam name="T">Object type (must be JSON-serializable).</typeparam>
    /// <param name="obj">The source object.</param>
    /// <returns>Deep-cloned instance, or <see langword="null"/> when input is null.</returns>
    /// <example>
    /// <code>
    /// var copy = original.Clone();
    /// </code>
    /// </example>
    /// <remarks>Uses <see cref="System.Text.Json.JsonSerializer"/> for deep copy. Not suitable for types with circular references.</remarks>
    public static T? Clone<T>(this T? obj)
    {
        if (obj is null) return default;
        string json = JsonSerializer.Serialize(obj, SerializerOptions);
        return JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToJson / FromJson
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Serializes the object to a JSON string.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>JSON string, or "null" when input is null.</returns>
    /// <example>
    /// <code>
    /// var json = person.ToJson();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToJson<T>(this T? obj)
        => JsonSerializer.Serialize(obj, SerializerOptions);

    /// <summary>
    /// Deserializes a JSON string to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>Deserialized object, or <see langword="default"/> on failure.</returns>
    /// <example>
    /// <code>
    /// var person = "{\"name\":\"Alice\"}".FromJson&lt;Person&gt;();
    /// </code>
    /// </example>
    public static T? FromJson<T>(this string? json)
    {
        if (json is null or { Length: 0 }) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json, SerializerOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Pipe / Also / IfNotNull
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Passes the value through <paramref name="func"/> and returns the result.
    /// Enables fluent pipeline transformations without intermediate variables.
    /// </summary>
    /// <typeparam name="T">Input type.</typeparam>
    /// <typeparam name="TResult">Output type.</typeparam>
    /// <param name="value">The value to transform.</param>
    /// <param name="func">The transformation function.</param>
    /// <returns>Result of applying <paramref name="func"/> to <paramref name="value"/>.</returns>
    /// <example>
    /// <code>
    /// var length = "hello".Pipe(s => s.Length); // 5
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Pipe<T, TResult>(this T value, Func<T, TResult> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        return func(value);
    }

    /// <summary>
    /// Executes <paramref name="action"/> as a side effect and returns the original value unchanged.
    /// Useful for logging or debugging within a fluent chain.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="value">The value to pass through.</param>
    /// <param name="action">The side-effect action.</param>
    /// <returns>The original <paramref name="value"/> unchanged.</returns>
    /// <example>
    /// <code>
    /// var result = GetUser().Also(u => logger.Log(u.Name));
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Also<T>(this T value, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action(value);
        return value;
    }

    /// <summary>
    /// Executes <paramref name="action"/> only when <paramref name="value"/> is not <see langword="null"/>.
    /// Returns the original value to allow chaining.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <param name="action">Action to perform when non-null.</param>
    /// <returns>The original <paramref name="value"/>.</returns>
    /// <example>
    /// <code>
    /// user?.IfNotNull(u => Send(u));
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? IfNotNull<T>(this T? value, Action<T> action) where T : class
    {
        ArgumentNullException.ThrowIfNull(action);
        if (value is not null) action(value);
        return value;
    }
}
