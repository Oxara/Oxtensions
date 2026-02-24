// -----------------------------------------------------------------------
// <copyright file="DataTableExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace Oxtensions.DataTable;

/// <summary>
/// High-performance extension methods for <see cref="System.Data.DataTable"/>.
/// </summary>
public static class DataTableExtensions
{
    // Reflection cache: Type -> (PropertyInfo[], propertyNameLower -> PropertyInfo)
    private static readonly ConcurrentDictionary<Type, (PropertyInfo[] Props, Dictionary<string, PropertyInfo> Map)>
        ReflectionCache = new();

    // ─────────────────────────────────────────────────────────────────────────
    // HasRows
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the table is not null and contains at least one row.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns><see langword="true"/> when rows exist.</returns>
    /// <example>
    /// <code>
    /// dataTable.HasRows(); // true / false
    /// </code>
    /// </example>
    public static bool HasRows(this System.Data.DataTable? table)
        => table is not null && table.Rows.Count > 0;

    // ─────────────────────────────────────────────────────────────────────────
    // ToList<T>
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Maps each <see cref="DataRow"/> to an instance of <typeparamref name="T"/> using cached reflection.
    /// Column names are matched to property names case-insensitively.
    /// </summary>
    /// <typeparam name="T">Target type with a parameterless constructor.</typeparam>
    /// <param name="table">The source table.</param>
    /// <returns>List of mapped objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="table"/> is null.</exception>
    /// <example>
    /// <code>
    /// var users = dataTable.ToList&lt;User&gt;();
    /// </code>
    /// </example>
    /// <remarks>Reflection metadata is cached in a <see cref="ConcurrentDictionary{TKey,TValue}"/> for performance.</remarks>
    public static List<T> ToList<T>(this System.Data.DataTable table) where T : new()
    {
        ArgumentNullException.ThrowIfNull(table);

        var (_, map) = GetOrAddCache<T>();
        var result = new List<T>(table.Rows.Count);

        foreach (DataRow row in table.Rows)
        {
            var item = new T();
            foreach (DataColumn col in table.Columns)
            {
                if (map.TryGetValue(col.ColumnName.ToLowerInvariant(), out var prop))
                {
                    object? cellValue = row[col] == DBNull.Value ? null : row[col];
                    if (cellValue is not null)
                    {
                        Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(item, Convert.ChangeType(cellValue, targetType));
                    }
                }
            }
            result.Add(item);
        }

        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToJson
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Serializes the <see cref="System.Data.DataTable"/> to a JSON array string.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>JSON string representation of all rows.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="table"/> is null.</exception>
    /// <example>
    /// <code>
    /// string json = dataTable.ToJson();
    /// </code>
    /// </example>
    public static string ToJson(this System.Data.DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        var rows = new List<Dictionary<string, object?>>(table.Rows.Count);

        foreach (DataRow row in table.Rows)
        {
            var dict = new Dictionary<string, object?>(table.Columns.Count);
            foreach (DataColumn col in table.Columns)
                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            rows.Add(dict);
        }

        return JsonSerializer.Serialize(rows, JsonOptions);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ToDictionary
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="System.Data.DataTable"/> to a dictionary using specified key and value columns.
    /// </summary>
    /// <typeparam name="TKey">Type of the key column values.</typeparam>
    /// <typeparam name="TValue">Type of the value column values.</typeparam>
    /// <param name="table">The source table.</param>
    /// <param name="keyColumn">Column name to use as dictionary key.</param>
    /// <param name="valueColumn">Column name to use as dictionary value.</param>
    /// <returns>Dictionary of key-value pairs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// var dict = dataTable.ToDictionary&lt;int, string&gt;("Id", "Name");
    /// </code>
    /// </example>
    public static Dictionary<TKey, TValue?> ToDictionary<TKey, TValue>(
        this System.Data.DataTable table,
        string keyColumn,
        string valueColumn)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(keyColumn);
        ArgumentNullException.ThrowIfNull(valueColumn);

        var result = new Dictionary<TKey, TValue?>(table.Rows.Count);

        foreach (DataRow row in table.Rows)
        {
            var keyObj = row[keyColumn];
            var valObj = row[valueColumn];

            if (keyObj is DBNull) continue;

            TKey key = (TKey)Convert.ChangeType(keyObj, typeof(TKey));
            TValue? value = valObj is DBNull ? default : (TValue?)Convert.ChangeType(valObj, typeof(TValue));
            result[key] = value;
        }

        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ForEach
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Executes <paramref name="action"/> for each <see cref="DataRow"/> in the table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="action">Action to perform on each row.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>
    /// <code>
    /// dataTable.ForEach(row => Console.WriteLine(row["Name"]));
    /// </code>
    /// </example>
    public static void ForEach(this System.Data.DataTable table, Action<DataRow> action)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(action);

        foreach (DataRow row in table.Rows)
            action(row);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Private helpers
    // ─────────────────────────────────────────────────────────────────────────

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static (PropertyInfo[] Props, Dictionary<string, PropertyInfo> Map) GetOrAddCache<T>()
    {
        return ReflectionCache.GetOrAdd(typeof(T), static type =>
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite)
                            .ToArray();

            var map = new Dictionary<string, PropertyInfo>(props.Length, StringComparer.OrdinalIgnoreCase);
            foreach (var prop in props)
                map[prop.Name.ToLowerInvariant()] = prop;

            return (props, map);
        });
    }
}
