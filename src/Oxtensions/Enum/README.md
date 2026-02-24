# EnumExtensions

> `using Oxtensions.Enum;`

Reflection-based helpers for C# enums — description/display-name reading with caching, value listing, flag operations, safe parsing, and validation.

---

## Methods at a Glance

| Method | Description |
|--------|-------------|
| `GetDescription()` | Read `[Description("...")]` attribute |
| `GetDisplayName()` | Read `[Display(Name="...")]` attribute |
| `ToList<TEnum>()` | All values of an enum as a `List<TEnum>` |
| `HasFlag<T>(flag)` | Type-safe flag check |
| `Parse<TEnum>(value)` | Safe string-to-enum parse |
| `IsValid<TEnum>(value)` | Check if a value is defined |

---

## Attribute Setup

The description/display-name methods require standard .NET attributes:

```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public enum OrderStatus
{
    [Description("Pending Payment")]
    [Display(Name = "Awaiting Payment")]
    Pending = 0,

    [Description("Order Confirmed")]
    [Display(Name = "Confirmed")]
    Confirmed = 1,

    [Description("Shipped to Customer")]
    [Display(Name = "Shipped")]
    Shipped = 2,

    [Description("Delivered Successfully")]
    [Display(Name = "Delivered")]
    Delivered = 3,

    Cancelled = 4,  // no attributes — falls back to member name
}
```

---

## Usage Examples

### GetDescription

```csharp
OrderStatus.Pending.GetDescription();    // "Pending Payment"
OrderStatus.Shipped.GetDescription();    // "Shipped to Customer"
OrderStatus.Cancelled.GetDescription();  // "Cancelled"  (no attribute → member name)
```

### GetDisplayName

```csharp
OrderStatus.Confirmed.GetDisplayName();  // "Confirmed"
OrderStatus.Pending.GetDisplayName();    // "Awaiting Payment"
OrderStatus.Cancelled.GetDisplayName();  // "Cancelled"  (no attribute → member name)
```

> Both methods use a `ConcurrentDictionary<(Type, string), string>` cache so reflection is only performed once per member.

### ToList

```csharp
var statuses = EnumExtensions.ToList<OrderStatus>();
// [ Pending, Confirmed, Shipped, Delivered, Cancelled ]

foreach (var s in statuses)
    Console.WriteLine($"{(int)s}: {s.GetDescription()}");
// 0: Pending Payment
// 1: Order Confirmed
// ...
```

Populating a UI dropdown:

```csharp
var items = EnumExtensions.ToList<OrderStatus>()
    .Select(s => new SelectListItem
    {
        Value = ((int)s).ToString(),
        Text  = s.GetDisplayName()
    });
```

### HasFlag

```csharp
[Flags]
public enum Permission
{
    None    = 0,
    Read    = 1,
    Write   = 2,
    Delete  = 4,
    Admin   = Read | Write | Delete
}

Permission user = Permission.Read | Permission.Write;

user.HasFlag(Permission.Read);    // true
user.HasFlag(Permission.Delete);  // false
user.HasFlag(Permission.Admin);   // false  (Delete bit is missing)
```

### Parse

```csharp
EnumExtensions.Parse<OrderStatus>("Confirmed");  // OrderStatus.Confirmed
EnumExtensions.Parse<OrderStatus>("confirmed");  // OrderStatus.Confirmed (case-insensitive)
EnumExtensions.Parse<OrderStatus>("Unknown");    // null
EnumExtensions.Parse<OrderStatus>("");           // null
```

### IsValid

```csharp
EnumExtensions.IsValid<OrderStatus>(1);          // true   (Confirmed)
EnumExtensions.IsValid<OrderStatus>(99);         // false
EnumExtensions.IsValid<OrderStatus>("Shipped");  // true
EnumExtensions.IsValid<OrderStatus>("Ghost");    // false
```

---

## Performance Notes

- `GetDescription` and `GetDisplayName` cache results in a `ConcurrentDictionary<(Type, string), string>` — reflection is invoked only once per enum member per application lifetime.
- `ToList<TEnum>` calls `Enum.GetValues(typeof(TEnum))` which is itself cached by the runtime on .NET 6+.
- `Parse<TEnum>` uses `Enum.TryParse<TEnum>` with `ignoreCase: true` — no exception overhead on missing values.
