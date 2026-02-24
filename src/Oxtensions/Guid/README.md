# Identifiers Extensions (Guid)

> Namespace: `Oxtensions.Identifiers`

Utility extensions for `System.Guid` — emptiness checks, URL-safe short-string encoding, and COMB-style time-ordered GUID generation.

---

## Methods

| Method | Description |
|--------|-------------|
| `IsEmpty()` | Returns `true` if the GUID equals `Guid.Empty` |
| `IsNullOrEmpty()` | Returns `true` if the nullable GUID is `null` or `Guid.Empty` |
| `ToShortString()` | Encodes the GUID as a 22-character URL-safe Base64 string |
| `GuidExtensions.NewComb()` | Generates a time-ordered GUID with the high bytes set to current UTC time (ideal for clustered index performance) |

---

## Usage Examples

```csharp
using Oxtensions.Identifiers;

Guid.Empty.IsEmpty();                     // true
Guid.NewGuid().IsEmpty();                 // false

Guid? nullable = null;
nullable.IsNullOrEmpty();                 // true

var id = Guid.NewGuid().ToShortString();  // e.g. "3q2-7wEjNkiprFZARm8Gyg" (22 chars, URL-safe)

var ordered = GuidExtensions.NewComb();   // time-ordered GUID for DB insertion order
```

---

## Notes

- `ToShortString()` uses Base64URL encoding (replaces `+`→`-`, `/`→`_`, strips `==` padding). The result is always exactly 22 characters and safe to use in URLs and file names.
- `NewComb()` copies the last 6 bytes of the GUID with the current UTC millisecond timestamp, preserving randomness in the first 10 bytes. This reduces page splits in clustered SQL Server / PostgreSQL indexes compared to `Guid.NewGuid()`.
