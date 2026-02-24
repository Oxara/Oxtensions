# Oxtensions – Development Rules & Conventions

> This document is the authoritative reference for all coding rules, naming conventions, and structural requirements that govern the Oxtensions library. Every pull request and code change must comply with these rules before merging.

---

## Table of Contents

1. [Project Structure](#1-project-structure)
2. [Namespace Conventions](#2-namespace-conventions)
3. [File Conventions](#3-file-conventions)
4. [Extension Class Rules](#4-extension-class-rules)
   - [4.3 Class independence — no intra-library dependencies](#43-class-independence--no-intra-library-dependencies)
   - [4.4 No third-party dependencies](#44-no-third-party-dependencies)
5. [Method Rules](#5-method-rules)
6. [Performance Annotations](#6-performance-annotations)
7. [XML Documentation](#7-xml-documentation)
8. [Type-Specific Rules](#8-type-specific-rules)
9. [Argument Validation](#9-argument-validation)
10. [Test Conventions](#10-test-conventions)
11. [README Requirements](#11-readme-requirements)
12. [Code Style](#12-code-style)
13. [Changelog Maintenance](#13-changelog-maintenance)
14. [Release & Publishing](#14-release--publishing)

---

## ⚡ Quick Reference — Sık Kullanılan Komutlar

### Testleri çalıştır
```powershell
dotnet test --configuration Release                        # tüm framework'ler (net6–net10)
dotnet test --configuration Release --framework net10.0   # tek framework
dotnet test --filter "ClassName=StringExtensionsTests"    # tek sınıf
```

### NuGet paketi oluştur
```powershell
dotnet pack src/Oxtensions/Oxtensions.csproj -c Release -o ./nupkg
```

### NuGet paketini yayınla (manuel)
```powershell
dotnet nuget push nupkg\Oxtensions.*.nupkg `
  --api-key <KEY> `
  --source https://api.nuget.org/v3/index.json

dotnet nuget push nupkg\Oxtensions.*.snupkg `
  --api-key <KEY> `
  --source https://api.nuget.org/v3/index.json
```

### Yeni sürüm çıkar (CI otomatik yayınlar)
```powershell
# 1. Oxtensions.csproj içindeki <Version> değerini güncelle
# 2. CHANGELOG.md [Unreleased] → [x.y.z] - YYYY-MM-DD olarak güncelle
git add src/Oxtensions/Oxtensions.csproj CHANGELOG.md
git commit -m "chore: release vX.Y.Z"
git push

git tag vX.Y.Z
git push origin vX.Y.Z   # ← bu tag CI'yi tetikler, gerisini CI halleder
```

### Git — ilk kurulum (sadece bir kere)
```powershell
git init
git remote add origin https://github.com/Oxara/Oxtensions.git
git branch -M main
git push -u origin main
```

---

## 1. Project Structure

### 1.1 Source layout

```
src/
  Oxtensions/
    {Category}/                  ← single-type category
      {Type}Extensions.cs
      README.md
    {Group}/                     ← multi-type group (e.g. Numeric, Date)
      {SubType}/
        {SubType}Extensions.cs
      README.md                  ← one README covers all subtypes in the group
```

**Rules:**
- One extension class per file. File name = `{TypeName}Extensions.cs`.
- Single-type categories (e.g. `String`, `List`, `Enum`) are top-level folders directly under `src/Oxtensions/`.
- Related types that share a namespace and logical domain must be grouped under a **parent folder** with one subfolder per type:
  - `Numeric/` → `Int/`, `Long/`, `Double/`, `Decimal/`
  - `Date/` → `DateTime/`, `DateOnly/`, `DateTimeOffset/`
- Never merge two different extension classes into a single `.cs` file.
- All source folders must contain a `README.md`.

### 1.2 Test layout

```
tests/
  Oxtensions.Tests/
    {Category}/
      {Type}ExtensionsTests.cs
    {Group}/
      {SubType}ExtensionsTests.cs   ← flat inside group folder (no sub-subfolders)
```

---

## 2. Namespace Conventions

| Category | Namespace |
|----------|-----------|
| String | `Oxtensions.String` |
| Byte | `Oxtensions.Byte` |
| Char | `Oxtensions.Char` |
| Enum | `Oxtensions.Enum` |
| DataTable | `Oxtensions.DataTable` |
| Generic | `Oxtensions.Generic` |
| Guid (Identifiers) | `Oxtensions.Identifiers` |
| Stream / IO helpers | `Oxtensions.Streams` |
| TimeSpan / Duration builders | `Oxtensions.Durations` |
| Task / async helpers | `Oxtensions.Async` |
| Numeric (Int/Long/Double/Decimal) | `Oxtensions.Numeric` |
| Date (DateTime/DateOnly/DateTimeOffset) | `Oxtensions.Date` |
| Collections (List/Dictionary/Stack/Queue) | `Oxtensions.Collections` |

**Rules:**
- Namespace = `Oxtensions.{Category}` — always **PascalCase**, singular form.
- All types within a grouped folder (e.g. `Numeric/`, `Date/`) share **one** namespace. No sub-namespaces like `Oxtensions.Numeric.Int`.
- Test namespaces mirror the category: `Oxtensions.Tests.{Category}` (e.g. `Oxtensions.Tests.Date`).
- Never use the physical subfolder name as a namespace qualifier.
- **No namespace may shadow or conflict with a well-known .NET BCL or commonly used framework namespace.** For example, `Oxtensions.Guid`, `Oxtensions.TimeSpan`, `Oxtensions.Stream`, and `Oxtensions.Task` are **forbidden** because they shadow `System.Guid`, `System.TimeSpan`, `System.IO.Stream`, and `System.Threading.Tasks.Task` respectively inside any file that also declares `namespace Oxtensions.*`. Choose a category name that does not collide: e.g. use `Oxtensions.Async` instead of `Oxtensions.Task`, `Oxtensions.Streams` instead of `Oxtensions.Stream`, `Oxtensions.Identifiers` instead of `Oxtensions.Guid`, `Oxtensions.Durations` instead of `Oxtensions.TimeSpan`.
- When choosing a new namespace, verify that the chosen name does not appear as a type or namespace in any of the following BCL root namespaces: `System`, `System.IO`, `System.Text`, `System.Linq`, `System.Collections`, `System.Threading`, `System.Threading.Tasks`, `System.Reflection`.  If a collision exists, append a qualifying adjective or use an alternative noun (e.g. `Oxtensions.GuidHelpers`, `Oxtensions.Identifiers`).

---

## 3. File Conventions

### 3.1 File header

Every source and test file must begin with the following copyright block:

```csharp
// -----------------------------------------------------------------------
// <copyright file="{FileName}.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------
```

### 3.2 Nullable enable

Every `.cs` file must include `#nullable enable` immediately after the copyright block and before any `using` directives.

```csharp
#nullable enable
```

### 3.3 Using directives

- Place `using` directives after `#nullable enable`.
- Use `using System.Runtime.CompilerServices;` when `[MethodImpl]` is used.
- Avoid unnecessary `using` directives.
- Source files must **not** use `using` to import their own namespace.

### 3.4 Type aliases

When an extension method's `this` parameter type name conflicts with a commonly used identifier in test code, declare a file-scoped type alias:

```csharp
using SysDateTime = System.DateTime;   // avoids conflict with DateTimeExtensions class name
```

Use the prefix `Sys` for all system type aliases.

---

## 4. Extension Class Rules

### 4.1 Declaration

```csharp
public static class {TypeName}Extensions
```

- Always `public static class`.
- Class name = `{TypeName}Extensions`, where `{TypeName}` is the exact BCL type name (e.g. `DateTime`, `DateOnly`, `DateTimeOffset`, `Int32`, `String`).
- No inheritance, no constructors, no instance state — pure static members only.

### 4.2 Uniqueness within a namespace

No two extension methods within the same namespace may have the **same name** and the **same `this` parameter type**. This applies across all classes in the namespace, not just within one class.

> **Example:** `DateTimeExtensions.FromUnixTimestamp(this long)` returns `DateTime`. If `DateTimeOffsetExtensions` also needs a `long` → `DateTimeOffset` conversion in the same `Oxtensions.Date` namespace, the method must have a **distinct name** (e.g. `ToDateTimeOffset(this long)`) to prevent ambiguity at the call site.

### 4.3 Class independence — no intra-library dependencies

Each extension class must be **fully self-contained**. The following are strictly forbidden:

- Calling a method from another `*Extensions` class anywhere in the same `Oxtensions.*` library.
- `using` a namespace from within this library in a source file of this library (e.g. `using Oxtensions.Numeric;` inside `StringExtensions.cs`).
- Sharing helper utilities through a common base class, shared static class, or any other coupling mechanism between extension classes.

This rule ensures that any single extension class can be extracted, published, or tested in complete isolation without pulling in any other part of the library.

> **Rationale:** Should a consumer ever need only `StringExtensions`, they must be able to take that one file without any transitive dependency on `NumericExtensions`, `DateExtensions`, or any other class in the solution.

When the same logic is needed in two classes, **duplicate it** rather than introduce a coupling. Private helper methods inside the same class are always permitted.

### 4.4 No third-party dependencies

The `Oxtensions` library (the `src/Oxtensions/` project) must depend **only on the .NET BCL and the .NET SDK**. No NuGet packages, no third-party assemblies, and no project-to-project references to non-BCL code are permitted in `Oxtensions.csproj`.

**Forbidden in production source:**
- Any `<PackageReference>` that is not a Microsoft-owned, SDK-shipped package (e.g. `System.*`, `Microsoft.Extensions.*` from the official SDK are acceptable; anything else is not).
- Packages from NuGet that are not part of the .NET platform itself (e.g. `Newtonsoft.Json`, `Polly`, `AutoMapper`, `MediatR` or any other independently versioned library).

**Permitted in test projects only:**
- `xUnit` / `xunit.runner.*` — test framework.
- `FluentAssertions` — assertion library.
- `coverlet.*` — code coverage tooling.

> **Rationale:** A consumer installing this library must not be forced to pull in additional transitive packages. All functionality must be implemented using types available in the .NET runtime that the consumer already targets.

---

## 5. Method Rules

### 5.1 Section dividers

Group related methods with a full-width divider comment:

```csharp
// ─────────────────────────────────────────────────────────────────────────
// Section Title
// ─────────────────────────────────────────────────────────────────────────
```

Use consistent section titles across all extension classes (e.g. `Unix timestamp`, `Weekend / Weekday`, `Start / End boundaries`, `Age`, `Is Today / Past / Future`, `Next / Previous Workday`, `ISO 8601`).

### 5.2 Method visibility

All extension methods must be `public static`. No `internal`, `private`, or `protected` extension methods.

### 5.3 Private helpers

Permissible private static members:

- `private static readonly {Type} {Name}` — for shared constants/epoch values.
- `private static {Type} {Name}(...)` — for non-extension helper methods.

Private helpers must not be extension methods (`this` parameter not allowed on private methods).

### 5.4 Preferred implementation style

- **Zero-allocation first:** Use BCL constructors over string manipulation or intermediate allocations.
  - ✅ `new DateTime(y, m, 1, 0, 0, 0, kind)` 
  - ❌ `DateTime.Parse($"{y}-{m}-01")`
- **Preserve state on boundary methods:**
  - `DateTime` boundary methods must preserve `DateTimeKind` by passing `value.Kind` to constructors.
  - `DateTimeOffset` boundary methods must preserve `value.Offset` by passing it to constructors.

---

## 6. Performance Annotations

### 6.1 `[MethodImpl(MethodImplOptions.AggressiveInlining)]`

**Required on:** Methods whose entire body is a single expression or a simple constructor call.

```csharp
// ✅ Single expression → must have [AggressiveInlining]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool IsWeekend(this DateTime value)
    => value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
```

**Forbidden on:** Methods that contain any of the following:
- Loop (`for`, `foreach`, `while`, `do`)
- Multi-branch conditionals (`if/else`, `switch`)
- Local variable declarations (beyond trivial `var x = SomeSingleCall()` one-liners)
- Multi-line method bodies (more than one logical statement)

```csharp
// ✅ Loop → must NOT have [AggressiveInlining]
public static DateTime NextWorkday(this DateTime value)
{
    var next = value.Date.AddDays(1);
    while (next.IsWeekend()) next = next.AddDays(1);
    return next;
}
```

---

## 7. XML Documentation

### 7.1 Required tags

Every `public` method must have:

| Tag | When required |
|-----|--------------|
| `<summary>` | Always |
| `<param name="...">` | For every parameter, including `this` (omit for `this` if self-evident) |
| `<returns>` | Always when the return type is not `void` |

### 7.2 `<example>` tag

Required for:
- Any method whose usage is non-obvious from its signature.
- All `FromXxx` / `ToXxx` conversion methods.
- All boundary methods (StartOf*, EndOf*) when the behavior has an edge case.

Format:

```csharp
/// <example>
/// <code>
/// new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp(); // 946684800
/// </code>
/// </example>
```

### 7.3 Language quality

- Use en-US English for all documentation.
- Doc comments for boolean-returning methods must start with `"Returns <see langword="true"/> if …"`.
- Doc comments for boundary methods must state whether the original `DateTimeKind` or UTC offset is preserved.

---

## 8. Type-Specific Rules

### 8.1 `DateTime` extensions

- Methods that return a `DateTime` based on an input `DateTime` must always preserve `DateTimeKind`.
  - Pass `value.Kind` to every `new DateTime(...)` constructor call in these methods.
- `IsPast` / `IsFuture`: compare using `ToUniversalTime()` vs `DateTime.UtcNow`.
- `IsToday`: compare `value.Date` vs `DateTime.Today` (local date).
- `Age`: compare `value.Date` vs `DateTime.Today`; subtract if birthday hasn't occurred yet this year.

### 8.2 `DateOnly` extensions

- No `DateTimeKind` to preserve; constructors take `(year, month, day)` only.
- `IsToday`: use `DateOnly.FromDateTime(DateTime.Today)`.
- `IsPast` / `IsFuture`: compare against `DateOnly.FromDateTime(DateTime.Today)`.
- `Age`: use `DateOnly.AddYears` for birthday comparison.

### 8.3 `DateTimeOffset` extensions

- **All** boundary methods must preserve `value.Offset`.
  - Pass `value.Offset` as the last argument to every `new DateTimeOffset(...)` constructor call.
- `IsToday`: compare `value.Date` (which returns a `DateTime`) vs `DateTime.Today`.
- `IsPast` / `IsFuture`: compare against `DateTimeOffset.UtcNow`.
- `ToUnixTimestamp`: use `value.ToUnixTimeSeconds()` (BCL method, zero-allocation).
- `ToDateTimeOffset(this long)`: use `DateTimeOffset.FromUnixTimeSeconds(timestamp)`.

### 8.4 Numeric extensions

- `Clamp`: throw `ArgumentException` when `min > max`.
- `Factorial`: throw `ArgumentException` for negative inputs.
- `IsPrime`: return `false` for values ≤ 1.
- `[AggressiveInlining]` applies to all sign/parity/range predicates and single-line arithmetic.

---

## 9. Argument Validation

| Scenario | Action |
|----------|--------|
| `min > max` in range/clamp operations | `throw new ArgumentException(...)` |
| Negative values for factorial, square-root-based operations | `throw new ArgumentException(...)` |
| Null references on reference-type parameters | `throw new ArgumentNullException(nameof(param))` |

Error messages in exceptions must be descriptive and include the parameter name. Do not use `ArgumentOutOfRangeException` — use `ArgumentException` consistently.

---

## 10. Test Conventions

### 10.1 Naming

```
{MethodName}_{Scenario}_{ExpectedResult}
```

Examples:
- `IsWeekend_Saturday_ReturnsTrue`
- `EndOfMonth_LeapYear_Returns29`
- `ToUnixTimestamp_KnownDate_ReturnsCorrectTimestamp`

### 10.2 Test class declaration

```csharp
public sealed class {TypeName}ExtensionsTests
```

- Always `public sealed class`. Never `abstract`, `partial`, or `static`.
- One test class per extension class.
- One test file per test class.

### 10.3 Assertion library

Use **FluentAssertions only**. Never use `Assert.*` from xUnit or MSTest.

```csharp
// ✅
result.Should().Be(expected);
value.Should().BeGreaterThan(0);

// ❌
Assert.Equal(expected, result);
```

### 10.4 Parameterized tests

Use `[Theory]` + `[InlineData(...)]` for any test that checks the same logic with 3+ data points. Never write multiple nearly-identical `[Fact]` methods that differ only in input values.

### 10.5 Sections in test files

Mirror the section structure of the source file with the same divider comments:

```csharp
// ── MethodGroup ──────────────────────────────────────────────────────────
```

### 10.6 Type aliases in tests

Declare a file-scoped alias when the extension class name shadows a BCL type name:

```csharp
using SysDateTime = System.DateTime;
```

Alias prefix is always `Sys`.

### 10.7 No internal state

Test classes must not hold mutable instance state. Use `private static readonly` for shared test constants (e.g. timezone offsets).

```csharp
private static readonly TimeSpan Utc3 = TimeSpan.FromHours(3);
```

---

## 11. README Requirements

### 11.1 Every category folder must have a `README.md`

Minimum structure:

```markdown
# {TypeName} Extensions

Brief description.

## Methods

| Method | Description |
|--------|-------------|
| `MethodName(params)` | One-line description |

## Usage Examples

\```csharp
// Example code
\```
```

### 11.2 Grouped categories

For grouped categories (`Numeric/`, `Date/`), a **single** `README.md` at the group level covers all subtypes. No per-subtype READMEs.

### 11.3 Root README (`README.md`)

The root README must:
- Contain a namespace/class table listing every available extension category with a link to its `README.md`.
- Contain a **Quick Examples** section with at least one code snippet per category.
- Use the correct, current namespace names (no stale references).

---

## 12. Code Style

### 12.1 Expression-bodied members

Single-expression methods **must** use expression body syntax (`=>`). Multi-statement methods **must** use block body syntax (`{ ... }`).

### 12.2 Pattern matching

Use pattern matching where it improves readability:

```csharp
// ✅
value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday

// ❌
value.DayOfWeek == DayOfWeek.Saturday || value.DayOfWeek == DayOfWeek.Sunday
```

### 12.3 Target-typed `new`

Use target-typed `new()` when the type is unambiguous from context. Use full `new {Type}(...)` when clarity benefits readability (especially in return statements).

### 12.4 `var` usage

- Use `var` when the right-hand side makes the type obvious.
- Use explicit types for method parameters and return types.

### 12.5 String formatting

Never use string interpolation or `string.Format` for timestamp or date formatting in production code. Use `.ToString("format")` with the exact format literal.

### 12.6 File organization order (per file)

1. Copyright header comment
2. `#nullable enable`
3. `using` directives (alphabetical)
4. `namespace` declaration (file-scoped)
5. `public static class` declaration
6. `private static readonly` fields
7. Method groups (ordered by logical domain, divided by section comments)

---

## 13. Changelog Maintenance

Every code change must be reflected in [CHANGELOG.md](CHANGELOG.md) **before the pull request is merged**. The changelog follows the [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) format and [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### 13.1 Always write under `[Unreleased]`

All changes go into the `## [Unreleased]` section at the top of the file. When a version is released, the entire `[Unreleased]` block is moved to a new versioned section (e.g. `## [1.1.0] — YYYY-MM-DD`) and a fresh empty `[Unreleased]` placeholder is inserted.

### 13.2 Required change categories

Use only the following headings inside a release section, and only include headings that have entries:

| Heading | When to use |
|---------|------------|
| `### Added` | New extension method, new class, new namespace |
| `### Changed` | Renamed method or parameter, behaviour change, namespace rename |
| `### Deprecated` | Method still present but marked obsolete |
| `### Removed` | Method or class deleted |
| `### Fixed` | Bug fix in existing behaviour |

### 13.3 Entry format

Each entry must follow this format:

```markdown
- `Namespace.ClassName.MethodName(params)` — one-sentence description.
```

**Rules:**
- Reference the fully-qualified method name in backticks.
- Write the description in the imperative mood and keep it to one sentence.
- For namespace additions, reference the namespace and list the added class(es).
- Do **not** mention internal implementation details (e.g. `Span<T>` usage, regex flags) — describe observable behaviour only.

**Examples:**

```markdown
### Added
- `Oxtensions.String.StringExtensions.Truncate(int, string)` — truncates a string to the given length, appending an optional suffix.
- `Oxtensions.Async` — new namespace containing `TaskExtensions` with `FireAndForget`, `WithTimeout`, `Retry`.

### Changed
- `Oxtensions.Numeric.DecimalExtensions.Abs()` — renamed from `AbsoluteValue()` for consistency with `int` and `double` overloads.

### Fixed
- `Oxtensions.Streams.StreamExtensions.DecompressGzip()` — no longer throws on an already-disposed stream.
```

### 13.4 What must be logged

| Change | Required entry |
|--------|---------------|
| New public method | `### Added` |
| New class | `### Added` |
| New namespace | `### Added` |
| Method renamed | `### Changed` — state old and new name |
| Parameter renamed or type changed | `### Changed` |
| Behaviour change (even without signature change) | `### Changed` |
| Method marked `[Obsolete]` | `### Deprecated` |
| Method or class removed | `### Removed` |
| Bug fix | `### Fixed` |
| Namespace rename | `### Changed` — state old → new name |

> **Internal-only changes** (private helpers, performance tuning with no observable behaviour difference, XML doc text fixes) do **not** require a changelog entry.

---

## 14. Release & Publishing

### 14.1 Versioning

Oxtensions follows [Semantic Versioning 2.0.0](https://semver.org/):

| Change type | Version bump | Example |
|-------------|-------------|--------|
| Backwards-compatible new method | **MINOR** | 1.0.0 → 1.1.0 |
| Backwards-compatible bug fix | **PATCH** | 1.0.0 → 1.0.1 |
| Breaking change (rename, removal, signature change) | **MAJOR** | 1.0.0 → 2.0.0 |

Update `<Version>` in `src/Oxtensions/Oxtensions.csproj` before tagging.

### 14.2 Release checklist

Before creating a release tag, verify all of the following:

- [ ] All tests pass on `main`: `dotnet test --configuration Release`
- [ ] `<Version>` in `Oxtensions.csproj` matches the intended tag.
- [ ] `CHANGELOG.md` `[Unreleased]` section has been moved to a new versioned block (e.g. `## [1.1.0] — YYYY-MM-DD`) and a fresh empty `[Unreleased]` placeholder has been added.
- [ ] All changes are committed and pushed to `main`.

### 14.3 Tagging & publishing

Publishing is **fully automated via CI** (`.github/workflows/ci.yml`). When a version tag is pushed, the workflow:

1. Runs all tests across net6–net9.
2. Runs `dotnet pack`.
3. Pushes `.nupkg` + `.snupkg` to nuget.org.
4. Creates a GitHub Release with the tag and `CHANGELOG.md` as body.

**To release:**

```powershell
# 1. Bump version in Oxtensions.csproj, update CHANGELOG.md, commit
git add src/Oxtensions/Oxtensions.csproj CHANGELOG.md
git commit -m "chore: release v1.x.x"
git push

# 2. Create and push the tag — this triggers the publish workflow
git tag v1.x.x
git push origin v1.x.x
```

### 14.4 NuGet API key

The API key must be stored as a **repository secret** named `NUGET_API_KEY` in GitHub:

> GitHub repo → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**
> - Name: `NUGET_API_KEY`  
> - Value: nuget.org API key (scope: Push, glob `Oxtensions*`)

**The API key must never be committed to any file in the repository.**

#### API key expiry & renewal

nuget.org API keys have a maximum lifetime of **365 days**. The current key expires on **2027-02-22**.

nuget.org sends a reminder e-mail ~30 days before expiry. If no e-mail arrives, renew manually around the expiry date each year.

**Renewal steps (takes ~2 minutes):**

1. **Generate a new key on nuget.org:**  
   👉 https://www.nuget.org/account/apikeys  
   Click **Regenerate** on the existing key (or delete it and create a new one).  
   Settings: glob `Oxtensions*` | Permission: Push new packages and package versions.

2. **Update the GitHub secret:**  
   👉 https://github.com/Oxara/Oxtensions/settings/secrets/actions  
   Click **Update** on `NUGET_API_KEY` and paste the new key.

3. **Update the local NuGet config** (only needed if you push manually):
   ```powershell
   dotnet nuget update source nuget.org --password <YENİ_ANAHTAR>
   ```

> If the key expires, only the CI publish step will fail — builds and tests continue to work. Updating the GitHub secret (step 2) is sufficient to restore publishing.

### 14.5 Manual publish (emergency only)

If CI is unavailable and a release must be published manually:

```powershell
dotnet pack src/Oxtensions/Oxtensions.csproj -c Release -o ./nupkg

dotnet nuget push nupkg\Oxtensions.*.nupkg `
  --api-key <KEY> `
  --source https://api.nuget.org/v3/index.json

dotnet nuget push nupkg\Oxtensions.*.snupkg `
  --api-key <KEY> `
  --source https://api.nuget.org/v3/index.json
```

Delete the `nupkg/` folder afterwards — it is listed in `.gitignore` and must not be committed.

---

*Last updated: 2026 — Oxtensions Development Team*
