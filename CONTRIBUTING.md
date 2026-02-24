# Contributing to Oxtensions

Thank you for your interest in contributing! Oxtensions is a focused, rule-governed library, so please read this guide carefully before opening an issue or submitting a pull request.

---

## Table of Contents

1. [Code of Conduct](#code-of-conduct)
2. [Prerequisites](#prerequisites)
3. [Getting Started](#getting-started)
4. [Branch & Commit Conventions](#branch--commit-conventions)
5. [Development Rules](#development-rules)
6. [Writing Tests](#writing-tests)
7. [Opening a Pull Request](#opening-a-pull-request)
8. [Reporting Bugs](#reporting-bugs)
9. [Requesting Features](#requesting-features)

---

## Code of Conduct

This project follows the [Contributor Covenant v2.1](https://www.contributor-covenant.org/version/2/1/code_of_conduct/). By participating you agree to abide by its terms.

---

## Prerequisites

| Tool | Minimum version |
|------|----------------|
| .NET SDK | 8.0 (tests target net6–net10) |
| Git | 2.x |
| IDE | Visual Studio 2022 **or** VS Code + C# Dev Kit |

---

## Getting Started

```bash
# 1. Fork the repository on GitHub, then clone your fork
git clone https://github.com/<your-username>/oxtensions.git
cd oxtensions

# 2. Restore and build
dotnet build

# 3. Run all tests
dotnet test --configuration Release
```

All 632 tests must pass before you submit a PR.

---

## Branch & Commit Conventions

### Branches

| Pattern | Purpose |
|---------|---------|
| `feat/<namespace>/<short-name>` | New extension method(s) |
| `fix/<short-description>` | Bug fix |
| `docs/<short-description>` | Documentation only |
| `refactor/<short-description>` | Non-functional restructuring |

### Commit messages

Follow [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/):

```
<type>(<scope>): <imperative short description>

[optional body]

[optional footer: Closes #123]
```

**Types:** `feat`, `fix`, `docs`, `refactor`, `test`, `chore`  
**Scope:** the namespace segment, e.g. `string`, `numeric`, `async`

Examples:
```
feat(numeric): add Factorial() to IntExtensions
fix(streams): handle empty byte array in CompressGzip
docs(readme): add Oxtensions.Async quick example
```

---

## Development Rules

All contributions **must** comply with [DEVELOPMENT_RULES.md](DEVELOPMENT_RULES.md). Key points:

1. **One class per file** — one `static` extension class per `.cs` file.
2. **No cross-class calls** — each extension class must be self-contained. Never call methods from another extension class.
3. **Namespace semantics** — use the correct `Oxtensions.<Category>` namespace (see the table in DEVELOPMENT_RULES.md). Do **not** create a namespace that shadows a BCL type (`Guid`, `Stream`, `TimeSpan`, etc.).
4. **No third-party packages** — only .NET BCL APIs.
5. **XML documentation** — every `public` member must have `<summary>`, `<param>`, and `<returns>` tags.
6. **`ArgumentNullException.ThrowIfNull`** — use this for null-guard; do not write manual `if (x == null) throw`.

---

## Writing Tests

- Test project: `tests/Oxtensions.Tests/`
- Framework: **xUnit** + **FluentAssertions**
- Mirror the source folder structure: `src/Oxtensions/Char/CharExtensions.cs` → `tests/Oxtensions.Tests/Char/CharExtensionsTests.cs`
- Minimum coverage expectation: every `public` method must have at least:
  - one **happy-path** test
  - one **edge-case / boundary** test
  - one **null or invalid input** test (where applicable)
- Use descriptive test method names: `MethodName_Condition_ExpectedResult`

---

## Opening a Pull Request

1. Make sure all tests pass locally: `dotnet test --configuration Release`
2. Update `CHANGELOG.md` under `## [Unreleased]`.
3. Fill in the pull request template completely.
4. Keep PRs focused — one logical change per PR.

A maintainer will review your PR within a reasonable time. Feedback will be given inline; please address comments before requesting a re-review.

---

## Reporting Bugs

Use the **Bug Report** issue template. Include:
- A minimal reproduction (code snippet or unit test).
- The .NET version(s) affected.
- Expected vs actual behaviour.

---

## Requesting Features

Use the **Feature Request** issue template. Include:
- The target namespace and proposed method signature.
- Motivation / use-case.
- Any edge cases to consider.

PRs that implement a feature without a prior issue are still welcome, but opening an issue first allows discussion before you invest time coding.
