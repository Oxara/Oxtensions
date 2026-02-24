# StringExtensions

> `using Oxtensions.String;`

High-performance extension methods for `System.String` — Span-based null checks, slug generation with Turkish character support, safe parsing, masking, casing conversions, validation, and more.

---

## Methods at a Glance

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | Span-based null/empty check |
| `IsNullOrWhiteSpace()` | Span-based whitespace check |
| `ToSlug()` | SEO-friendly URL slug with Turkish char support |
| `Truncate(maxLength, suffix)` | Truncate with ellipsis |
| `ToTitleCase()` | Hello World |
| `ToPascalCase()` | HelloWorld |
| `ToCamelCase()` | helloWorld |
| `ToSnakeCase()` | hello_world |
| `ToKebabCase()` | hello-world |
| `RemoveAccents()` | Strip diacritics — café → cafe, naïve → naive |
| `ContainsIgnoreCase(value)` | Case-insensitive contains |
| `ToInt32OrDefault(default)` | Safe int parse |
| `ToDecimalOrDefault(default)` | Safe decimal parse |
| `ToGuidOrDefault()` | Safe GUID parse |
| `Mask(start, end, char)` | Card / ID masking |
| `IsValidEmail()` | Email format validation |
| `IsValidUrl()` | HTTP/HTTPS URL validation |
| `IsValidPhoneNumber()` | Phone number validation |
| `Repeat(count)` | Repeat string N times |
| `ReverseWords()` | Reverse word order |
| `Reverse()` | Reverse characters |
| `CountOccurrences(substring)` | Count non-overlapping occurrences |
| `Left(length)` | Leftmost N characters |
| `Right(length)` | Rightmost N characters |
| `RemoveHtmlTags()` | Strip HTML tags |
| `ToBase64()` | UTF-8 → Base64 |
| `FromBase64()` | Base64 → UTF-8 |

---

## Usage Examples

### Null / Empty Checks

```csharp
string? value = null;
value.IsNullOrEmpty();       // true
"".IsNullOrEmpty();          // true
"hello".IsNullOrEmpty();     // false

"   ".IsNullOrWhiteSpace();  // true
"hi".IsNullOrWhiteSpace();   // false
```

### Slug

```csharp
"Hello Wörld! Naïve".ToSlug();          // "hello-world-naive"
"SEO Friendly URL 2026".ToSlug();    // "seo-friendly-url-2026"
"foo  --  bar".ToSlug();             // "foo-bar"
```

### Truncate

```csharp
"Hello World".Truncate(7);           // "Hell..."
"Hello World".Truncate(7, "---");    // "Hell---"
"Hi".Truncate(10);                   // "Hi"  (unchanged)
```

### Casing Conversions

```csharp
"hello world".ToTitleCase();   // "Hello World"
"hello world".ToPascalCase();  // "HelloWorld"
"hello world".ToCamelCase();   // "helloWorld"
"Hello World".ToSnakeCase();   // "hello_world"
"Hello World".ToKebabCase();   // "hello-world"
"foo_bar_baz".ToPascalCase();  // "FooBarBaz"
```

### Accent Removal

```csharp
"café résumé".RemoveAccents();              // "cafe resume"
"naïve façade pêche".RemoveAccents();        // "naive facade peche"
Über-kühl".RemoveAccents();                // "Uber-kuhl"
```

Also handles characters without NFD decompositions:

```csharp
"Türkçe Şehir İstanbul".RemoveAccents();  // "Turkce Sehir Istanbul"
"ırmak".RemoveAccents();                   // "irmak"  (dotless-i)
"øre".RemoveAccents();                     // "ore"    (Nordic o-slash)
"Straße".RemoveAccents();                 // "Strase" (German sharp-s)
```

### Safe Parsing

```csharp
"42".ToInt32OrDefault();          // 42
"abc".ToInt32OrDefault(-1);       // -1
((string?)null).ToInt32OrDefault(); // 0

"3.14".ToDecimalOrDefault();      // 3.14m
"bad".ToDecimalOrDefault(0m);     // 0m

Guid.NewGuid().ToString()
    .ToGuidOrDefault();           // valid Guid
"not-a-guid".ToGuidOrDefault();   // Guid.Empty
```

### Masking

```csharp
// Credit card
"1234567890123456".Mask(4, 4);          // "1234********3456"

// Turkish ID (TCKN)
"12345678901".Mask(3, 2);               // "123******01"

// Custom mask character
"JOHNDOE@EXAMPLE.COM".Mask(4, 7, '#'); // "JOHN########.COM"
```

### Validation

```csharp
"user@example.com".IsValidEmail();   // true
"not-an-email".IsValidEmail();       // false

"https://github.com".IsValidUrl();   // true
"ftp://bad".IsValidUrl();            // false

"+90 532 000 0000".IsValidPhoneNumber(); // true
"123".IsValidPhoneNumber();              // false
```

### Repeat / Reverse

```csharp
"ab".Repeat(3);                  // "ababab"
"Hello World".ReverseWords();    // "World Hello"
"hello".Reverse();               // "olleh"
```

### Count / Left / Right

```csharp
"banana".CountOccurrences("an"); // 2
"Hello World".Left(5);           // "Hello"
"Hello World".Right(5);          // "World"
```

### HTML & Base64

```csharp
"<b>Hello</b> <i>World</i>".RemoveHtmlTags(); // "Hello World"

"Hello, World!".ToBase64();       // "SGVsbG8sIFdvcmxkIQ=="
"SGVsbG8=".FromBase64();          // "Hello"
"!!!invalid!!!".FromBase64();     // ""  (graceful fallback)
```

---

## Performance Notes

- `IsNullOrEmpty` / `IsNullOrWhiteSpace` → `MemoryExtensions.IsWhiteSpace` (zero allocation)
- `Mask` → `ArrayPool<char>` (avoids heap allocation for the char buffer)
- `Repeat` → `string.Create` with a state lambda (single allocation)
- `Reverse` → `ArrayPool<char>` + span reverse (no LINQ)
- All `TryParse` calls use `.AsSpan()` overloads to skip intermediate string allocation
- All regex patterns are `static readonly` with `RegexOptions.Compiled`
- `RemoveAccents` uses a small supplemental map for code points that NFD cannot decompose (ı, ø, ð, þ, ß, æ), then strips all `NonSpacingMark` categories via NFD — supports all Latin-based scripts
