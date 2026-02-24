# Date Extensions

> High-performance extension methods for `DateTime`, `DateOnly`, and `DateTimeOffset`, all unified under the `Oxtensions.Date` namespace.

```csharp
using Oxtensions.Date;
```

---

## DateTime Extensions

| Method | Description |
|--------|-------------|
| `ToUnixTimestamp()` | Converts to Unix timestamp in seconds (UTC) |
| `FromUnixTimestamp(this long)` | Converts Unix seconds to UTC `DateTime` |
| `IsWeekend()` | `true` if Saturday or Sunday |
| `IsWeekday()` | `true` if Monday–Friday |
| `StartOfDay()` | Returns midnight of the same date, preserving `DateTimeKind` |
| `EndOfDay()` | Returns 23:59:59.999 of the same date, preserving `DateTimeKind` |
| `StartOfMonth()` | First day of the month at midnight, preserving `DateTimeKind` |
| `EndOfMonth()` | Last day of the month at 23:59:59.999, preserving `DateTimeKind` |
| `StartOfYear()` | January 1st at midnight, preserving `DateTimeKind` |
| `EndOfYear()` | December 31st at 23:59:59.999, preserving `DateTimeKind` |
| `StartOfWeek(DayOfWeek)` | Start of the week (default: Monday), preserving `DateTimeKind` |
| `Age()` | Age in completed years from birth date to today |
| `IsToday()` | `true` if local date equals today |
| `IsPast()` | `true` if value (UTC) is before `DateTime.UtcNow` |
| `IsFuture()` | `true` if value (UTC) is after `DateTime.UtcNow` |
| `NextWorkday()` | Next Monday–Friday after the date |
| `PreviousWorkday()` | Previous Monday–Friday before the date |
| `ToIso8601String()` | ISO 8601 UTC string (e.g. `"2026-02-22T12:00:00Z"`) |

### Usage

```csharp
using Oxtensions.Date;

var dt = new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc); // Friday

dt.NextWorkday();                          // 2026-02-23 (Monday)
dt.StartOfMonth();                         // 2026-02-01 00:00:00 UTC
dt.EndOfYear();                            // 2026-12-31 23:59:59.999 UTC
dt.ToUnixTimestamp();                      // 1740009600
946684800L.FromUnixTimestamp();            // 2000-01-01 00:00:00 UTC
dt.ToIso8601String();                      // "2026-02-20T00:00:00Z"
new DateTime(1990, 5, 15).Age();           // years since 1990-05-15
```

---

## DateOnly Extensions

| Method | Description |
|--------|-------------|
| `IsWeekend()` | `true` if Saturday or Sunday |
| `IsWeekday()` | `true` if Monday–Friday |
| `StartOfMonth()` | First day of the month |
| `EndOfMonth()` | Last day of the month |
| `StartOfYear()` | January 1st of the same year |
| `EndOfYear()` | December 31st of the same year |
| `StartOfWeek(DayOfWeek)` | Start of the week (default: Monday) |
| `ToDateTime(TimeOnly?)` | Converts to `DateTime`; defaults to midnight |
| `Age()` | Age in completed years from birth date to today |
| `IsToday()` | `true` if equals today's local date |
| `IsPast()` | `true` if before today |
| `IsFuture()` | `true` if after today |
| `NextWorkday()` | Next Monday–Friday after the date |
| `PreviousWorkday()` | Previous Monday–Friday before the date |

### Usage

```csharp
using Oxtensions.Date;

var date = new DateOnly(2026, 2, 20); // Friday

date.NextWorkday();                   // 2026-02-23 (Monday)
date.StartOfYear();                   // 2026-01-01
date.EndOfMonth();                    // 2026-02-28
date.ToDateTime();                    // 2026-02-20 00:00:00
date.ToDateTime(new TimeOnly(9, 0));  // 2026-02-20 09:00:00
new DateOnly(2024, 2, 1).EndOfMonth(); // 2024-02-29 (leap year)
```

---

## DateTimeOffset Extensions

| Method | Description |
|--------|-------------|
| `ToUnixTimestamp()` | Converts to Unix timestamp in seconds |
| `ToDateTimeOffset(this long)` | Converts Unix seconds to UTC `DateTimeOffset` |
| `IsWeekend()` | `true` if Saturday or Sunday |
| `IsWeekday()` | `true` if Monday–Friday |
| `StartOfDay()` | Midnight of the same date, **preserving UTC offset** |
| `EndOfDay()` | 23:59:59.999 of the same date, **preserving UTC offset** |
| `StartOfMonth()` | First day of the month at midnight, **preserving UTC offset** |
| `EndOfMonth()` | Last day of the month at 23:59:59.999, **preserving UTC offset** |
| `StartOfYear()` | January 1st at midnight, **preserving UTC offset** |
| `EndOfYear()` | December 31st at 23:59:59.999, **preserving UTC offset** |
| `StartOfWeek(DayOfWeek)` | Start of the week (default: Monday), **preserving UTC offset** |
| `Age()` | Age in completed years from birth date to local today |
| `IsToday()` | `true` if local date equals today |
| `IsPast()` | `true` if value is before `DateTimeOffset.UtcNow` |
| `IsFuture()` | `true` if value is after `DateTimeOffset.UtcNow` |
| `NextWorkday()` | Next Monday–Friday, **preserving UTC offset** |
| `PreviousWorkday()` | Previous Monday–Friday, **preserving UTC offset** |
| `ToIso8601String()` | ISO 8601 string with offset (e.g. `"2026-02-22T15:00:00+03:00"`) |

### Usage

```csharp
using Oxtensions.Date;

var offset = TimeSpan.FromHours(3);
var dto = new DateTimeOffset(2026, 2, 20, 15, 0, 0, offset); // Friday, UTC+3

dto.NextWorkday();                      // 2026-02-23 00:00:00 +03:00 (Monday)
dto.StartOfDay();                       // 2026-02-20 00:00:00 +03:00
dto.EndOfMonth();                       // 2026-02-28 23:59:59.999 +03:00
dto.ToIso8601String();                  // "2026-02-20T15:00:00+03:00"
dto.ToUnixTimestamp();                  // Unix seconds
946684800L.ToDateTimeOffset();          // 2000-01-01T00:00:00+00:00
```

> **Key difference from `DateTime`:** All boundary methods on `DateTimeOffset` preserve the original UTC offset. A value at `+03:00` stays at `+03:00` after calling `StartOfDay()`, `EndOfYear()`, etc.
