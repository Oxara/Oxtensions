// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxtensions.Enum;

/// <summary>
/// High-performance extension methods for <see cref="System.Enum"/> types.
/// </summary>
public static class EnumExtensions
{
    // Reflection caches — ConcurrentDictionary for thread safety
    private static readonly ConcurrentDictionary<(Type, string), string?> DescriptionCache = new();
    private static readonly ConcurrentDictionary<(Type, string), string?> DisplayNameCache = new();

    // ─────────────────────────────────────────────────────────────────────────
    // Description / DisplayName
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the <see cref="DescriptionAttribute.Description"/> of an enum member,
    /// or the member name when no attribute is present.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>Description string.</returns>
    /// <example>
    /// <code>
    /// MyEnum.SomeValue.GetDescription(); // "Friendly description"
    /// </code>
    /// </example>
    public static string GetDescription(this System.Enum value)
    {
        var type = value.GetType();
        string name = value.ToString();

        return DescriptionCache.GetOrAdd((type, name), static key =>
        {
            var field = key.Item1.GetField(key.Item2);
            return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? key.Item2;
        }) ?? name;
    }

    /// <summary>
    /// Returns the <see cref="DisplayAttribute.Name"/> of an enum member,
    /// or the member name when no attribute is present.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>Display name string.</returns>
    /// <example>
    /// <code>
    /// MyEnum.SomeValue.GetDisplayName(); // "Display Name"
    /// </code>
    /// </example>
    public static string GetDisplayName(this System.Enum value)
    {
        var type = value.GetType();
        string name = value.ToString();

        return DisplayNameCache.GetOrAdd((type, name), static key =>
        {
            var field = key.Item1.GetField(key.Item2);
            return field?.GetCustomAttribute<DisplayAttribute>()?.Name ?? key.Item2;
        }) ?? name;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToList
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns all values of the specified enum type as a list.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>List of all enum values.</returns>
    /// <example>
    /// <code>
    /// EnumExtensions.ToList&lt;DayOfWeek&gt;(); // [Sunday, Monday, ...]
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TEnum> ToList<TEnum>() where TEnum : struct, System.Enum
        => [.. System.Enum.GetValues<TEnum>()];

    // ─────────────────────────────────────────────────────────────────────────
    // HasFlag (generic)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the enum value contains the specified flag.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The enum value to check.</param>
    /// <param name="flag">The flag to test for.</param>
    /// <returns><see langword="true"/> when the flag is set.</returns>
    /// <example>
    /// <code>
    /// (FileAccess.ReadWrite).HasFlag(FileAccess.Read); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<T>(this T value, T flag) where T : struct, System.Enum
        => value.HasFlag(flag);

    // ─────────────────────────────────────────────────────────────────────────
    // Parse
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Parses a string representation of an enum, returning the default value on failure.
    /// </summary>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="ignoreCase">Perform case-insensitive parsing. Defaults to <see langword="true"/>.</param>
    /// <returns>Parsed enum value, or <see langword="default"/> when parsing fails.</returns>
    /// <example>
    /// <code>
    /// EnumExtensions.Parse&lt;DayOfWeek&gt;("Monday"); // DayOfWeek.Monday
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Parse<TEnum>(string value, bool ignoreCase = true) where TEnum : struct, System.Enum
        => System.Enum.TryParse<TEnum>(value, ignoreCase, out TEnum result) ? result : default;

    // ─────────────────────────────────────────────────────────────────────────
    // IsValid
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the integer is a defined value of the specified enum.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The integer value to test.</param>
    /// <returns><see langword="true"/> when defined; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// EnumExtensions.IsValid&lt;DayOfWeek&gt;(1); // true (Monday)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid<TEnum>(int value) where TEnum : struct, System.Enum
        => System.Enum.IsDefined(typeof(TEnum), value);
}
