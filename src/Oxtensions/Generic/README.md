# GenericExtensions

> `using Oxtensions.Generic;`

Universal helpers for any type `T` — null checks, guard clauses, membership tests, deep cloning via JSON, and JSON serialization/deserialization.

---

## Methods at a Glance

| Method | Description |
|--------|-------------|
| `IsNull<T>()` | `true` if the value is `null` |
| `IsNotNull<T>()` | `true` if the value is not `null` |
| `ThrowIfNull<T>(paramName)` | ArgumentNullException guard |
| `In<T>(values)` | SQL-style `IN (...)` membership check |
| `Clone<T>()` | Deep clone via JSON round-trip |
| `ToJson<T>()` | Serialize to JSON string |
| `FromJson<T>(json)` | Deserialize from JSON string |

---

## Usage Examples

### Null Checks

```csharp
string? name = null;
name.IsNull();     // true
name.IsNotNull();  // false

object obj = new();
obj.IsNull();     // false
obj.IsNotNull();  // true
```

### ThrowIfNull

```csharp
public void ProcessOrder(Order? order)
{
    order.ThrowIfNull(nameof(order));
    // order is guaranteed non-null from here
    Console.WriteLine(order.Id);
}

ProcessOrder(null);
// ArgumentNullException: Value cannot be null. (Parameter 'order')
```

> On .NET 6+ this is similar to `ArgumentNullException.ThrowIfNull`, but available as an extension method usable on any type.

### In — SQL-style Membership

```csharp
int status = 2;
status.In(1, 2, 3);      // true
status.In(4, 5, 6);      // false

string role = "admin";
role.In("admin", "superuser");  // true
role.In("guest", "viewer");     // false
```

Useful for replacing long `if` chains:

```csharp
// Before
if (status == 1 || status == 2 || status == 5) { ... }

// After
if (status.In(1, 2, 5)) { ... }
```

Works with any type that implements `IEquatable<T>` (uses `EqualityComparer<T>.Default`):

```csharp
DayOfWeek today = DateTime.Today.DayOfWeek;
bool isWeekend = today.In(DayOfWeek.Saturday, DayOfWeek.Sunday);
```

### Clone — Deep Copy

```csharp
public class Address
{
    public string Street { get; set; } = "";
    public string City   { get; set; } = "";
}

var original = new Address { Street = "123 Main St", City = "Istanbul" };
var copy = original.Clone();

copy.City = "Ankara";

Console.WriteLine(original.City);  // "Istanbul"  — unaffected
Console.WriteLine(copy.City);      // "Ankara"
```

Deep copy of a complex graph:

```csharp
var order = new Order
{
    Id    = 42,
    Lines = new List<OrderLine>
    {
        new() { ProductId = 1, Qty = 2 },
        new() { ProductId = 3, Qty = 1 },
    }
};

var orderCopy = order.Clone<Order>()!;
orderCopy.Lines[0].Qty = 99;

Console.WriteLine(order.Lines[0].Qty);     // 2  — original unchanged
Console.WriteLine(orderCopy.Lines[0].Qty); // 99
```

> Uses `System.Text.Json.JsonSerializer` serialize → deserialize round-trip. All properties must be public and JSON-serializable.

### ToJson / FromJson

```csharp
var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com" };

string json = user.ToJson();
// {"Id":1,"Name":"Alice","Email":"alice@example.com"}

User? restored = json.FromJson<User>();
// restored.Name == "Alice"
```

```csharp
// FromJson gracefully handles invalid JSON
User? bad = "not json at all".FromJson<User>();
// bad == null  (returns default, no exception)
```

Logging example:

```csharp
logger.LogDebug("Request payload: {Payload}", request.ToJson());
```

---

## Performance Notes

- `IsNull` / `IsNotNull` are pure null equality checks decorated with `[MethodImpl(AggressiveInlining)]` — zero overhead.
- `In` uses `EqualityComparer<T>.Default.Equals` in a `params T[]` loop — no LINQ, no boxing for value types.
- `Clone` / `ToJson` / `FromJson` use `System.Text.Json.JsonSerializer` (Microsoft's built-in, high-performance serializer) — no Newtonsoft.Json dependency.
- `FromJson` catches `JsonException` and returns `default` rather than propagating, making it suitable for untrusted input without try/catch at the call site.
