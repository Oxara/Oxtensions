---
name: Feature Request
about: Propose a new extension method or enhancement to an existing one
title: "[Feature] <namespace> — <MethodName>()"
labels: enhancement
assignees: ''
---

## Target namespace

<!-- Which namespace should this method belong to?
     e.g. Oxtensions.String, Oxtensions.Numeric, Oxtensions.Async
     If you're proposing a brand-new namespace, explain why a new one is needed. -->

## Proposed method signature

```csharp
// Example:
public static string Truncate(this string value, int maxLength, string suffix = "…")
```

## Motivation / use-case

<!-- Why is this useful? Describe the problem you are solving and provide a concrete example. -->

## Example usage

```csharp
"Hello, World!".Truncate(7);        // "Hello, …"
"Hello, World!".Truncate(7, "..."); // "Hello,..."
```

## Edge cases to consider

<!-- null input, empty string, negative length, boundary values, etc. -->

## Alternatives considered

<!-- Any workaround you currently use, or alternative method name / behaviour. -->
