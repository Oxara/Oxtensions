# Char Extensions

> Namespace: `Oxtensions.Char`

Lightweight predicates for working with individual `char` values — vowel/consonant detection, ASCII category checks, case tests, and whitespace/newline detection.

---

## Methods

| Method | Description |
|--------|-------------|
| `IsVowel()` | Returns `true` if the character is an English vowel (a e i o u, case-insensitive) |
| `IsConsonant()` | Returns `true` if the character is an ASCII letter but not a vowel |
| `IsAsciiLetter()` | Returns `true` for a–z and A–Z |
| `IsAsciiDigit()` | Returns `true` for 0–9 |
| `IsAsciiLetterOrDigit()` | Returns `true` for a–z, A–Z, or 0–9 |
| `IsWhitespace()` | Returns `true` for space, tab, newline, and other Unicode whitespace |
| `IsNewLine()` | Returns `true` for `\n` or `\r` |
| `IsUppercase()` | Returns `true` for uppercase letters |
| `IsLowercase()` | Returns `true` for lowercase letters |

---

## Usage Examples

```csharp
using Oxtensions.Char;

'a'.IsVowel();            // true
'b'.IsConsonant();        // true
'Z'.IsAsciiLetter();      // true
'9'.IsAsciiDigit();       // true
'k'.IsAsciiLetterOrDigit(); // true
' '.IsWhitespace();       // true
'\n'.IsNewLine();         // true
'A'.IsUppercase();        // true
'z'.IsLowercase();        // true
```
