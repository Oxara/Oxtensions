# Durations Extensions (TimeSpan)

> Namespace: `Oxtensions.Durations`

Predicates, arithmetic helpers, and human-readable formatting for `System.TimeSpan`, plus fluent duration builder extensions on `int` and `long`.

---

## Classes

| Class | Extends | Description |
|-------|---------|-------------|
| `TimeSpanExtensions` | `System.TimeSpan` | Predicates, `Abs`, week conversion, formatting |
| `IntDurationExtensions` | `int` | Fluent builders: `3.Days()`, `2.Hours()`, etc. |
| `LongDurationExtensions` | `long` | Same fluent builders for `long` values |

---

## Methods — TimeSpanExtensions

| Method | Description |
|--------|-------------|
| `IsPositive()` | Returns `true` when the duration is greater than zero |
| `IsNegative()` | Returns `true` when the duration is less than zero |
| `IsZero()` | Returns `true` when the duration equals `TimeSpan.Zero` |
| `Abs()` | Returns the absolute (non-negative) duration |
| `TotalWeeks()` | Returns total elapsed weeks as a `double` |
| `ToHumanReadable()` | Returns a compact human string: `"1d 4h 30m 0s"` |
| `ToIso8601Duration()` | Returns an ISO 8601 duration string: `"P1DT4H30M0S"` |

## Methods — Duration builders (int / long)

| Method | Returns |
|--------|---------|
| `n.Days()` | `TimeSpan.FromDays(n)` |
| `n.Hours()` | `TimeSpan.FromHours(n)` |
| `n.Minutes()` | `TimeSpan.FromMinutes(n)` |
| `n.Seconds()` | `TimeSpan.FromSeconds(n)` |
| `n.Milliseconds()` | `TimeSpan.FromMilliseconds(n)` |

---

## Usage Examples

```csharp
using Oxtensions.Durations;

TimeSpan.FromSeconds(-5).IsNegative();     // true
TimeSpan.FromSeconds(-5).Abs();            // 00:00:05
TimeSpan.FromDays(14).TotalWeeks();        // 2.0
TimeSpan.FromMinutes(150).ToHumanReadable();   // "2h 30m 0s"
TimeSpan.FromDays(2).ToIso8601Duration();      // "P2DT0H0M0S"

// Fluent builders
var timeout = 30.Seconds();         // TimeSpan.FromSeconds(30)
var expiry  = 7.Days();             // TimeSpan.FromDays(7)
var lag     = 500L.Milliseconds();  // TimeSpan.FromMilliseconds(500)
```
