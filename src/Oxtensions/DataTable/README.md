# DataTableExtensions

> `using Oxtensions.DataTable;`

Extensions for `System.Data.DataTable` — predicate checks, reflection-backed object mapping with caching, JSON serialization, key/value projection, and side-effect iteration.

---

## Methods at a Glance

| Method | Description |
|--------|-------------|
| `HasRows()` | Returns `true` if the table has at least one row |
| `ToList<T>()` | Map rows to POCO objects (cached reflection) |
| `ToJson()` | Serialize the table to a JSON array string |
| `ToDictionary<TKey, TValue>(keyCol, valCol)` | Build a dictionary from two columns |
| `ForEach(action)` | Iterate rows with a side-effect action |

---

## Usage Examples

### HasRows

```csharp
DataTable table = GetSomeTable();

if (!table.HasRows())
{
    Console.WriteLine("No data returned.");
    return;
}
```

### ToList — Mapping rows to POCOs

Define a class whose property names match column names (case-insensitive):

```csharp
public class Customer
{
    public int    Id       { get; set; }
    public string Name     { get; set; } = "";
    public string Email    { get; set; } = "";
    public bool   IsActive { get; set; }
}
```

```csharp
DataTable dt = ExecuteQuery("SELECT Id, Name, Email, IsActive FROM Customers");

List<Customer> customers = dt.ToList<Customer>();

foreach (var c in customers)
    Console.WriteLine($"{c.Id}: {c.Name} ({c.Email})");
```

- Column lookup is **case-insensitive** (`id`, `ID`, `Id` all map correctly).
- `DBNull` values are silently skipped — the property retains its default value.
- Property/column mapping is cached in a `ConcurrentDictionary<Type, ...>` — reflection runs only once per type.

### ToJson

```csharp
DataTable dt = GetProductsTable();
string json = dt.ToJson();

// [{"Id":1,"Name":"Widget","Price":9.99},{"Id":2,"Name":"Gadget","Price":19.99}]
```

Useful for API responses or logging the contents of a `DataTable`:

```csharp
logger.LogDebug("Query result: {Json}", resultsTable.ToJson());
```

### ToDictionary

```csharp
// Build a lookup of Id → Name from a DataTable
DataTable dt = ExecuteQuery("SELECT Id, Name FROM Countries");

Dictionary<int, string> lookup = dt.ToDictionary<int, string>("Id", "Name");

lookup[90]; // "Turkey"
lookup[49]; // "Germany"
```

```csharp
// Config table with string keys and values
DataTable config = ExecuteQuery("SELECT [Key], [Value] FROM AppConfig");
var settings = config.ToDictionary<string, string>("Key", "Value");

settings["ConnectionTimeout"]; // "30"
```

### ForEach

```csharp
DataTable dt = GetOrdersTable();

dt.ForEach(row =>
{
    var orderId = (int)row["OrderId"];
    var status  = row["Status"]?.ToString();
    Console.WriteLine($"Order #{orderId} — {status}");
});
```

---

## Performance Notes

- `ToList<T>` builds a `(PropertyInfo[], Dictionary<string, PropertyInfo>)` tuple per type and stores it in a `ConcurrentDictionary<Type, ...>`. Subsequent calls for the same type reuse the cached mapping — no `GetProperties()` calls after the first invocation.
- Column-to-property matching uses a pre-built case-insensitive `Dictionary<string, PropertyInfo>` constructed with `StringComparer.OrdinalIgnoreCase`.
- `ToJson` uses `System.Text.Json.JsonSerializer` with the built-in `DataTable` converter — no third-party JSON library required.
- `DBNull` handling uses `row[col] is DBNull` pattern — no exception-based control flow.
