# Oxtensions

![Oxtensions](https://raw.githubusercontent.com/Oxara/Oxtensions/main/assets/oxtensions_banner.png)

> High-performance, production-ready .NET extension methods library.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Target Frameworks](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209%20%7C%2010-purple)](#)

---

## 📦 Installation

```bash
dotnet add package Oxtensions
```

---

## 🗂️ Extension Categories

| Namespace | Class | Description |
|-----------|-------|-------------|
| `Oxtensions.String` | `StringExtensions` | [Slug, masking, casing, validation, safe parsing →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/String/README.md) |
| `Oxtensions.Byte` | `ByteExtensions` | [Hex, Base64, MD5/SHA-256 hashing →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Byte/README.md) |
| `Oxtensions.Date` | `DateTimeExtensions` · `DateOnlyExtensions` · `DateTimeOffsetExtensions` | [Unix timestamps, boundaries, workday navigation, ISO 8601 →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Date/README.md) |
| `Oxtensions.Collections` | `ListExtensions` · `DictionaryExtensions` · `StackExtensions` · `QueueExtensions` | [Chunking, pagination, merge, query string, stack/queue bulk ops →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Collections/README.md) |
| `Oxtensions.Enum` | `EnumExtensions` | [Description/Display attribute, flags, safe parse →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Enum/README.md) |
| `Oxtensions.DataTable` | `DataTableExtensions` | [Typed mapping with reflection cache, JSON export →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/DataTable/README.md) |
| `Oxtensions.Generic` | `GenericExtensions` | [Null guards, deep clone, JSON, IN predicate →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Generic/README.md) |
| `Oxtensions.Numeric` | `IntExtensions` · `LongExtensions` · `DoubleExtensions` · `DecimalExtensions` | [Sign, parity, clamp, prime, factorial, digits, rounding, currency →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Numeric/README.md) |
| `Oxtensions.Char` | `CharExtensions` | [Vowel/consonant, ASCII category, case and whitespace predicates →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Char/README.md) |
| `Oxtensions.Identifiers` | `GuidExtensions` | [IsEmpty, ToShortString, NewComb (time-ordered GUID) →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Guid/README.md) |
| `Oxtensions.Durations` | `TimeSpanExtensions` · `IntDurationExtensions` · `LongDurationExtensions` | [IsPositive/Negative/Zero, Abs, TotalWeeks, fluent builders →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/TimeSpan/README.md) |
| `Oxtensions.Streams` | `StreamExtensions` · `ByteStreamExtensions` | [ToByteArray, Base64, ReadAllText, GZip compress/decompress →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Stream/README.md) |
| `Oxtensions.Async` | `TaskExtensions` | [FireAndForget, WithTimeout, WithCancellation, Retry →](https://github.com/Oxara/Oxtensions/blob/main/src/Oxtensions/Async/README.md) |

---

## ⚡ Quick Examples

```csharp
using Oxtensions.Async;
using Oxtensions.Byte;
using Oxtensions.Char;
using Oxtensions.Collections;
using Oxtensions.DataTable;
using Oxtensions.Date;
using Oxtensions.Durations;
using Oxtensions.Enum;
using Oxtensions.Generic;
using Oxtensions.Identifiers;
using Oxtensions.Numeric;
using Oxtensions.Streams;
using Oxtensions.String;

// String
"Hello Wörld! Naïve".ToSlug();          // "hello-world-naive"
"1234567890123456".Mask(4, 4);          // "1234********3456"
"hello world".ToPascalCase();           // "HelloWorld"
"user@example.com".IsValidEmail();      // true

// Byte
byte[] data = Encoding.UTF8.GetBytes("hello");
data.ToHexString();                      // "68656C6C6F"
data.ComputeSHA256().ToHexString();      // SHA-256 hex

// DateTime
new DateTime(2026, 2, 20).NextWorkday();         // 2026-02-23
946684800L.FromUnixTimestamp();                  // 2000-01-01 00:00:00 UTC

// DateOnly
new DateOnly(2026, 2, 20).NextWorkday();         // 2026-02-23 (Friday → Monday)
new DateOnly(2026, 6, 15).StartOfYear();         // 2026-01-01
new DateOnly(2026, 6, 15).ToDateTime();          // 2026-06-15 00:00:00

// DateTimeOffset
DateTimeOffset.UtcNow.ToUnixTimestamp();         // Unix seconds
946684800L.ToDateTimeOffset();                   // 2000-01-01T00:00:00+00:00
new DateTimeOffset(2026, 2, 22, 15, 0, 0, TimeSpan.FromHours(3)).ToIso8601String();
// "2026-02-22T15:00:00+03:00"

// Collections — List
new[] {1,2,3,4,5}.Chunk(2);             // [[1,2],[3,4],[5]]
items.Paginate(0, 10);                   // first 10 items

// Collections — Dictionary
dict.GetOrDefault("key", "fallback");
dict.Merge(otherDict, overwrite: true);
new Dictionary<string,string>{{"q","hello"}}.ToQueryString(); // "q=hello"

// Enum
MyEnum.Active.GetDescription();          // [Description] attribute value
MyEnum.Active.GetDisplayName();          // [Display(Name=)] attribute value

// DataTable
var users = dataTable.ToList<User>();    // reflection-cached mapping
var json  = dataTable.ToJson();

// Generic
person.ThrowIfNull(nameof(person));
5.In(1, 3, 5, 7);                        // true
original.Clone();                        // deep copy via System.Text.Json

// Numeric (int / long / double)
7.IsPrime();                             // true
5.Factorial();                           // 120
1234.ToDigits();                         // [1, 2, 3, 4]
42.IsBetween(1, 100);                    // true
double.NaN.IsNaN();                      // true
3.14d.RoundTo(1);                        // 3.1

// Numeric (decimal)
1234.56m.ToCurrencyString("en-US");      // "$1,234.56"
150m.Clamp(0m, 100m);                    // 100
25m.Percentage(200m);                    // 12.5
(-42.5m).Abs();                          // 42.5

// Char
'a'.IsVowel();                           // true
'Z'.IsConsonant();                        // true
'5'.IsAsciiDigit();                       // true

// Identifiers (Guid)
Guid.Empty.IsEmpty();                    // true
Guid.NewGuid().ToShortString();          // 22-char Base64URL
Guid.NewComb();                          // time-ordered GUID

// Durations (TimeSpan)
TimeSpan.FromSeconds(-5).Abs();          // 00:00:05
5.Days() + 3.Hours();                    // 5.03:00:00
10.Minutes().ToHumanReadable();          // "10 minutes"

// Streams
byte[] data = File.ReadAllBytes("file.bin");
byte[] compressed = data.CompressGzip();
byte[] back = compressed.DecompressGzip();

// Async
await SomeWork().FireAndForget();
await SomeWork().WithTimeout(TimeSpan.FromSeconds(5));
await SomeWork().Retry(maxAttempts: 3, delay: TimeSpan.FromMilliseconds(500));

// Collections (Stack)
var stack = new Stack<int>();
stack.PushRange(new[] { 1, 2, 3 });   // top = 3
stack.PeekOrDefault(-1);               // 3 (no remove)
stack.PopMany(2);                      // [3, 2]
stack.Clone();                         // independent copy

// Collections (Queue)
var queue = new Queue<string>();
queue.EnqueueRange(new[] { "a", "b", "c" });
queue.PeekOrDefault("(empty)");        // "a" (no remove)
queue.DequeueMany(2);                  // ["a", "b"]
queue.Clone();                         // independent copy
```

---

## 🏗️ Project Structure

```
Oxtensions.sln
├── src/
│   └── Oxtensions/
│       ├── String/      StringExtensions.cs
│       ├── Byte/        ByteExtensions.cs
│       ├── Char/        CharExtensions.cs
│       ├── Date/
│       │   ├── DateTime/       DateTimeExtensions.cs
│       │   ├── DateOnly/       DateOnlyExtensions.cs
│       │   └── DateTimeOffset/ DateTimeOffsetExtensions.cs
│       ├── Collections/
│       │   ├── List/        ListExtensions.cs
│       │   ├── Dictionary/  DictionaryExtensions.cs
│       │   ├── Stack/       StackExtensions.cs
│       │   └── Queue/       QueueExtensions.cs
│       ├── DataTable/   DataTableExtensions.cs
│       ├── Enum/        EnumExtensions.cs
│       ├── Generic/     GenericExtensions.cs
│       ├── Guid/        GuidExtensions.cs
│       ├── TimeSpan/
│       │   ├── TimeSpanExtensions.cs
│       │   ├── IntDurationExtensions.cs
│       │   └── LongDurationExtensions.cs
│       ├── Stream/
│       │   ├── StreamExtensions.cs
│       │   └── ByteStreamExtensions.cs
│       ├── Async/       TaskExtensions.cs
│       └── Numeric/
│           ├── Int/     IntExtensions.cs
│           ├── Long/    LongExtensions.cs
│           ├── Double/  DoubleExtensions.cs
│           └── Decimal/ DecimalExtensions.cs
└── tests/
    └── Oxtensions.Tests/
        ├── String/      StringExtensionsTests.cs
        ├── Byte/        ByteExtensionsTests.cs
        ├── Char/        CharExtensionsTests.cs
        ├── Date/        DateTimeExtensionsTests.cs
        │               DateOnlyExtensionsTests.cs
        │               DateTimeOffsetExtensionsTests.cs
        ├── Collections/ ListExtensionsTests.cs
        │               DictionaryExtensionsTests.cs
        │               StackExtensionsTests.cs
        │               QueueExtensionsTests.cs
        ├── DataTable/   DataTableExtensionsTests.cs
        ├── Enum/        EnumExtensionsTests.cs
        ├── Generic/     GenericExtensionsTests.cs
        ├── Guid/        GuidExtensionsTests.cs
        ├── TimeSpan/    TimeSpanExtensionsTests.cs
        ├── Stream/      StreamExtensionsTests.cs
        ├── Async/       TaskExtensionsTests.cs
        └── Numeric/
            ├── IntExtensionsTests.cs
            ├── LongExtensionsTests.cs
            ├── DoubleExtensionsTests.cs
            └── DecimalExtensionsTests.cs
```

---

## 🧪 Running Tests

```bash
# All frameworks
dotnet test

# Specific framework
dotnet test --framework net10.0

# Specific class
dotnet test --filter "ClassName=StringExtensionsTests"
```

**Test coverage:** 807 tests across 5 frameworks (net6.0 · net7.0 · net8.0 · net9.0 · net10.0)

---

## 🔧 Engineering Highlights

- **Zero-allocation paths** — `ArrayPool<T>`, `stackalloc`, `string.Create`, `Span<T>` throughout
- **Reflection cache** — `ConcurrentDictionary<Type, ...>` in `DataTableExtensions` and `EnumExtensions`
- **Span-based parsing** — all `TryParse` calls use `.AsSpan()` overloads
- **Pre-compiled regex** — all regex patterns are `static readonly` with `RegexOptions.Compiled`
- **Thread-safe** — stateless extension methods; no shared mutable state
- **Nullable-aware** — `#nullable enable` on all files, null-safe public APIs

---

## 🧰 Other Oxara Tools

- **[Local Change Guard](https://github.com/Oxara/Oxara.VsCode.LocalChangeGuard)** — Keep developer-local Git hunks separate from committable changes without hiding either from the worktree. [Install from the VS Code Marketplace](https://marketplace.visualstudio.com/items?itemName=oxara.local-change-guard).

---

## 📄 License

MIT © 2026 [Oxara](https://github.com/oxara)
