# Changelog

All notable changes to **Oxtensions** are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

---

## [1.1.0] — 2026-02-24

### Added

- `Oxtensions.Collections.SetExtensions.IsNullOrEmpty()` — null-safe empty check for `ISet<T>`.
- `Oxtensions.Collections.SetExtensions.AddRange(items)` — bulk add; returns the count of newly inserted elements.
- `Oxtensions.Collections.SetExtensions.RemoveWhere(predicate)` — removes all elements matching the predicate; returns the count removed.
- `Oxtensions.Collections.SetExtensions.ToSortedSet()` — creates a new `SortedSet<T>` from the set elements in ascending order.
- `Oxtensions.Collections.SetExtensions.OverlapsWith(other)` — returns `true` when the set shares at least one element with the given sequence.
- `Oxtensions.Collections.SetExtensions.SymmetricDifference(other)` — returns a new `HashSet<T>` containing elements exclusive to exactly one collection, without mutating the source.
- `Oxtensions.Collections.ReadOnlyListExtensions.IsNullOrEmpty()` — null-safe empty check for `IReadOnlyList<T>` using `Count`.
- `Oxtensions.Collections.ReadOnlyListExtensions.IndexOf(item)` — returns the zero-based index of the first matching element, or `-1`.
- `Oxtensions.Collections.ReadOnlyListExtensions.LastIndexOf(item)` — returns the zero-based index of the last matching element, or `-1`.
- `Oxtensions.Collections.ReadOnlyListExtensions.BinarySearch(value)` — binary search on a sorted `IReadOnlyList<T>`; returns the element index or the bitwise complement of the insertion point.
- `Oxtensions.Collections.ReadOnlyListExtensions.Slice(start, length)` — returns a read-only sub-list covering the specified range.
- `Oxtensions.Collections.ReadOnlyDictionaryExtensions.ContainsAllKeys(keys)` — returns `true` only when every supplied key is present.
- `Oxtensions.Collections.ReadOnlyDictionaryExtensions.ContainsAnyKey(keys)` — returns `true` when at least one supplied key is present.
- `Oxtensions.Collections.ReadOnlyDictionaryExtensions.ToDictionary()` — creates a mutable `Dictionary<TKey, TValue>` copy of the read-only dictionary.
- `Oxtensions.Collections.ReadOnlyDictionaryExtensions.FilterByKeys(keys)` — returns a new dictionary containing only entries whose keys appear in the supplied key sequence.

---

## [1.0.0] — 2026-02-24

Initial release of Oxtensions — a high-performance, production-ready .NET extension methods library targeting net6.0 through net10.0.

### Added

#### `Oxtensions.String` — `StringExtensions`

- `ToSlug()` — URL-safe slug with Unicode transliteration.
- `Mask(int, int)` — mask middle characters (e.g. credit cards).
- `ToPascalCase()` / `ToCamelCase()` / `ToSnakeCase()` / `ToKebabCase()` — case converters.
- `IsValidEmail()` / `IsValidUrl()` / `IsValidIpAddress()` — format validators.
- `TryParseInt()` / `TryParseDouble()` / `TryParseDecimal()` / `TryParseBool()` / `TryParseGuid()` / `TryParseDateTime()` — safe parsers returning `bool`.
- `Truncate(int, string)` — truncate with optional suffix.
- `RemoveWhitespace()` — strip all whitespace characters.
- `Reverse()` — reverse a string respecting Unicode surrogate pairs.
- `CountOccurrences(string)` — count non-overlapping substring occurrences.
- `SplitAndTrim(char)` — split, trim each part, and remove empty entries.
- `ContainsAny(params string[])` — returns `true` if any of the given substrings is found.
- `IfNullOrEmpty(string)` — returns fallback when null or empty.
- `IfNullOrWhiteSpace(string)` — returns fallback when null or whitespace.
- `CombineWith(params string[])` — fluent `Path.Combine` wrapper.

#### `Oxtensions.Byte` — `ByteExtensions`

- `ToHexString()` — encode byte array to uppercase hex string.
- `FromHexString()` — decode hex string to byte array.
- `ToBase64String()` / `FromBase64String()` — Base64 encode/decode.
- `ComputeMD5()` / `ComputeSHA256()` / `ComputeSHA512()` — hash a byte array.

#### `Oxtensions.Char` — `CharExtensions`

- `IsVowel()` / `IsConsonant()` — English vowel/consonant detection.
- `IsAsciiLetter()` / `IsAsciiDigit()` / `IsAsciiAlphanumeric()` — ASCII category checks.
- `IsUpperCase()` / `IsLowerCase()` — case predicates.
- `IsWhiteSpaceOrControl()` — extended whitespace check.

#### `Oxtensions.Date` — `DateTimeExtensions` · `DateOnlyExtensions` · `DateTimeOffsetExtensions`

- `ToUnixTimestamp()` / `FromUnixTimestamp()` — Unix epoch conversion.
- `StartOfDay()` / `EndOfDay()` / `StartOfWeek()` / `EndOfWeek()` / `StartOfMonth()` / `EndOfMonth()` / `StartOfYear()` / `EndOfYear()` — boundary helpers.
- `NextWorkday()` / `PreviousWorkday()` — workday navigation (Mon–Fri).
- `IsWeekend()` / `IsWeekday()` — day-type predicates.
- `IsToday()` / `IsPast()` / `IsFuture()` — relative-time predicates.
- `ToIso8601String()` — ISO 8601 formatted string.
- `Age()` — calculate age in years from a birth date.
- `ToDateTime()` (DateOnly) — convert to `DateTime` at midnight.
- `ToDateTimeOffset()` (DateOnly) — convert to `DateTimeOffset` at midnight UTC.

#### `Oxtensions.Collections` — `ListExtensions` · `DictionaryExtensions` · `StackExtensions` · `QueueExtensions` · `SetExtensions` · `ReadOnlyListExtensions` · `ReadOnlyDictionaryExtensions`

**ListExtensions** (IEnumerable<T> / IList<T>):
- `IsNullOrEmpty()` — null/empty check.
- `Chunk<T>(int)` — split into fixed-size chunks.
- `Paginate<T>(int, int)` — page by index and size.
- `Shuffle<T>()` — Fisher-Yates in-place shuffle.
- `DistinctBy<T, TKey>(Func<T, TKey>)` — distinct by key selector.
- `ForEach<T>(Action<T>)` — side-effect iteration.
- `ToHashSet<T>()` — project to `HashSet<T>`.
- `AddRangeIfNotExists<T>(IEnumerable<T>)` — add only missing items.
- `RemoveWhere<T>(Func<T, bool>)` — conditional remove; returns count.
- `Flatten<T>()` — flatten nested sequences.
- `RandomItem<T>()` — single uniformly random element.
- `RandomItems<T>(int)` — multiple distinct random elements without replacement.
- `Rotate<T>(int)` — in-place left/right rotation.
- `Interleave<T>(IEnumerable<T>)` — alternating merge of two sequences.
- `CountBy<T, TKey>(Func<T, TKey>)` — group-and-count into a dictionary.
- `None<T>(Func<T, bool>?)` — inverse of `Any`.
- `HasDuplicates<T>()` — true when any element appears more than once.
- `Duplicates<T, TKey>(Func<T, TKey>)` — returns elements that appear more than once.
- `MinBy<T, TKey>(Func<T, TKey>)` / `MaxBy<T, TKey>(Func<T, TKey>)` — element with min/max projected value.

**DictionaryExtensions** (IDictionary<TKey, TValue>):
- `IsNullOrEmpty()` — null/empty check.
- `GetOrDefault(TKey, TValue?)` — safe get with fallback.
- `GetOrAdd(TKey, Func<TKey, TValue>)` — get or lazily insert.
- `Merge(IDictionary, bool)` — merge two dictionaries with optional overwrite.
- `Invert()` — swap keys and values.
- `ToQueryString()` — URL query string serialisation.
- `AddOrUpdate(TKey, TValue)` — upsert an entry.
- `RemoveWhere(Func<TKey, TValue, bool>)` — conditional remove; returns count.
- `ToJson()` — JSON serialisation via `System.Text.Json`.

**StackExtensions** (Stack<T>):
- `IsNullOrEmpty()` / `PeekOrDefault(T?)` / `PopOrDefault(T?)` / `PushRange(IEnumerable<T>)` / `PopMany(int)` / `Clone()`.

**QueueExtensions** (Queue<T>):
- `IsNullOrEmpty()` / `PeekOrDefault(T?)` / `DequeueOrDefault(T?)` / `EnqueueRange(IEnumerable<T>)` / `DequeueMany(int)` / `Clone()`.

**SetExtensions** (ISet<T>):
- `IsNullOrEmpty()` — null/empty check.
- `AddRange(IEnumerable<T>)` — bulk add; returns newly-inserted count.
- `RemoveWhere(Func<T, bool>)` — conditional remove; returns count.
- `ToSortedSet()` — new `SortedSet<T>` in ascending order.
- `OverlapsWith(IEnumerable<T>)` — true when at least one element is shared.
- `SymmetricDifference(IEnumerable<T>)` — returns new `HashSet<T>` without mutating source.

**ReadOnlyListExtensions** (IReadOnlyList<T>):
- `IsNullOrEmpty()` / `IndexOf(T)` / `LastIndexOf(T)` / `BinarySearch(T)` / `Slice(int, int)`.

**ReadOnlyDictionaryExtensions** (IReadOnlyDictionary<TKey, TValue>):
- `ContainsAllKeys(IEnumerable<TKey>)` / `ContainsAnyKey(IEnumerable<TKey>)` / `ToDictionary()` / `FilterByKeys(IEnumerable<TKey>)`.

#### `Oxtensions.Enum` — `EnumExtensions`

- `GetDescription()` / `GetDisplayName()` / `HasFlag<T>(T)` / `TryParse<T>(string, out T)` / `GetValues<T>()`.

#### `Oxtensions.DataTable` — `DataTableExtensions`

- `ToList<T>()` / `ToJson()` / `ToDictionary(string, string)`.

#### `Oxtensions.Generic` — `GenericExtensions`

- `ThrowIfNull(string)` / `In(params T[])` / `Clone<T>()` / `Pipe<T, TResult>(Func<T, TResult>)` / `Also<T>(Action<T>)` / `IfNotNull<T>(Action<T>)`.

#### `Oxtensions.Numeric` — `IntExtensions` · `LongExtensions` · `DoubleExtensions` · `DecimalExtensions`

- **Int/Long**: `IsEven()`, `IsOdd()`, `IsPositive()`, `IsNegative()`, `IsZero()`, `Abs()`, `Clamp(min, max)`, `IsPrime()`, `Factorial()`, `Fibonacci(n)`, `ToDigits()`, `IsBetween(min, max)`, `ToOrdinal()`, `ToRoman()`, `IsMultipleOf(n)`, `Pow(exp)`.
- **Double**: `IsNaN()`, `IsInfinity()`, `RoundTo(digits)`, `Clamp(min, max)`, `IsBetween(min, max)`, `Abs()`, `Lerp(a, b)`, `Normalize(min, max)`, `ToRadians()`, `ToDegrees()`.
- **Decimal**: `ToCurrencyString(culture)`, `Clamp(min, max)`, `Percentage(of)`, `Abs()`, `RoundTo(digits)`, `IsBetween(min, max)`, `ToNearest(step)`.

#### `Oxtensions.Identifiers` — `GuidExtensions`

- `IsEmpty()` / `IsNullOrEmpty()` / `ToShortString()` / `NewComb()`.

#### `Oxtensions.Durations` — `TimeSpanExtensions` · `IntDurationExtensions` · `LongDurationExtensions`

- **TimeSpanExtensions**: `IsPositive()`, `IsNegative()`, `IsZero()`, `Abs()`, `TotalWeeks()`, `ToHumanReadable()`, `ToIso8601Duration()`.
- **Int/LongDurationExtensions**: `Milliseconds()`, `Seconds()`, `Minutes()`, `Hours()`, `Days()`, `Weeks()`.

#### `Oxtensions.Streams` — `StreamExtensions` · `ByteStreamExtensions`

- **StreamExtensions**: `ToByteArray()`, `ToBase64String()`, `ReadAllText(encoding?)`, `CompressGzip()`, `DecompressGzip()`.
- **ByteStreamExtensions**: `ToStream()`, `CompressGzip()`, `DecompressGzip()`.

#### `Oxtensions.Async` — `TaskExtensions`

- `FireAndForget(Action<Exception>?)` — run without awaiting; optional error handler.
- `WithTimeout(TimeSpan)` / `WithTimeout<T>(TimeSpan)` — throws `TimeoutException` if not completed in time.
- `WithCancellation(CancellationToken)` / `WithCancellation<T>(CancellationToken)` — wraps task with cancellation support.
- `Retry(int, TimeSpan?)` / `Retry<T>(int, TimeSpan?)` — extension method on `Func<Task>`; retries on exception with configurable delay.

### Tests

- **744 tests** covering all 15 namespaces across net6.0 · net7.0 · net8.0 · net9.0 · net10.0.

---

[Unreleased]: https://github.com/oxara/oxtensions/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/oxara/oxtensions/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/oxara/oxtensions/releases/tag/v1.0.0