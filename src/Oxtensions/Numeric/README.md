# NumericExtensions

> `using Oxtensions.Numeric;`

Extension methods for `int`, `long`, `double`, and `decimal` — sign checks, parity, range clamping, membership tests, primality, factorial, digit decomposition, floating-point predicates, rounding, currency formatting, and percentage calculations.

---

## Methods at a Glance

### `int`

| Method | Description |
|--------|-------------|
| `IsPositive()` | `value > 0` |
| `IsNegative()` | `value < 0` |
| `IsZero()` | `value == 0` |
| `IsEven()` | Bitwise even check |
| `IsOdd()` | Bitwise odd check |
| `Clamp(min, max)` | Restrict to `[min, max]` |
| `IsBetween(min, max)` | Inclusive range membership |
| `Abs()` | Absolute value |
| `IsPrime()` | Primality test (trial division) |
| `Factorial()` | n! as `long` (0–20) |
| `DigitCount()` | Number of digits in absolute value |
| `ToDigits()` | Decompose into `int[]` of digits |
| `ToOrdinal()` | Ordinal string: `1` → `"1st"`, `12` → `"12th"` |
| `ToRoman()` | Roman numeral string: `4` → `"IV"`, `14` → `"XIV"` |
| `IsMultipleOf(n)` | `true` if evenly divisible by `n` |
| `Pow(exp)` | Integer exponentiation: `2.Pow(10)` → `1024` |

### `long`

| Method | Description |
|--------|-------------|
| `IsPositive()` / `IsNegative()` / `IsZero()` | Sign checks |
| `IsEven()` / `IsOdd()` | Parity |
| `Clamp(min, max)` | Restrict to `[min, max]` |
| `IsBetween(min, max)` | Inclusive range membership |
| `Abs()` | Absolute value |
| `DigitCount()` | Number of digits |
| `ToDigits()` | Decompose into `int[]` of digits |
| `IsMultipleOf(n)` | `true` if evenly divisible by `n` |
| `Pow(exp)` | Long exponentiation: `2L.Pow(10)` → `1024L` |

### `double`

| Method | Description |
|--------|-------------|
| `IsNaN()` | `double.IsNaN(value)` |
| `IsInfinity()` | Positive or negative infinity |
| `IsFinite()` | Finite and not NaN |
| `IsPositive()` | `> 0` and finite |
| `IsNegative()` | `< 0` and finite |
| `IsZero()` | `== 0` |
| `Clamp(min, max)` | Restrict to `[min, max]` |
| `IsBetween(min, max)` | Inclusive range membership |
| `Abs()` | Absolute value |
| `RoundTo(decimals)` | Round with `MidpointRounding.AwayFromZero` |
| `Lerp(a, b)` | Linear interpolation: `t.Lerp(a, b)` → `a + t*(b-a)` |
| `Normalize(min, max)` | Scale value to `[0, 1]` range |
| `ToRadians()` | Convert degrees to radians |
| `ToDegrees()` | Convert radians to degrees |

### `decimal`

| Method | Description |
|--------|-------------|
| `RoundTo(decimals)` | Round with `MidpointRounding.AwayFromZero` |
| `Percentage(total)` | What % of `total` the value is: `value / total * 100` |
| `IsPositive()` | `value > 0` |
| `IsNegative()` | `value < 0` |
| `IsZero()` | `value == 0` |
| `Clamp(min, max)` | Restrict to `[min, max]` |
| `ToCurrencyString(culture?)` | Format as currency string; defaults to `CultureInfo.CurrentCulture` |
| `Abs()` | `Math.Abs(value)` |
| `IsBetween(min, max)` | Inclusive range membership |
| `ToNearest(step)` | Round to nearest multiple of `step` |

---

## Usage Examples

### Sign Checks

```csharp
5.IsPositive();    // true
(-3).IsNegative(); // true
0.IsZero();        // true

99L.IsPositive();  // true
(-1L).IsNegative();// true

1.5d.IsPositive(); // true
double.PositiveInfinity.IsPositive(); // false  (infinity excluded)
```

### Parity

```csharp
4.IsEven();  // true
3.IsOdd();   // true
(-6).IsEven(); // true   (works on negatives)
```

### Clamp & IsBetween

```csharp
200.Clamp(0, 100);    // 100
(-5).Clamp(0, 100);   // 0
50.Clamp(0, 100);     // 50

5.IsBetween(1, 10);   // true
1.IsBetween(1, 10);   // true  (boundary inclusive)
11.IsBetween(1, 10);  // false

5.5d.IsBetween(1d, 10d); // true
200L.Clamp(0L, 100L);    // 100L
```

### Absolute Value

```csharp
(-7).Abs();    // 7
(-99L).Abs();  // 99L
(-7.5d).Abs(); // 7.5
```

### IsPrime

```csharp
2.IsPrime();   // true
7.IsPrime();   // true
97.IsPrime();  // true
1.IsPrime();   // false
4.IsPrime();   // false
(-5).IsPrime();// false
```

### Factorial

```csharp
0.Factorial();  // 1
5.Factorial();  // 120
10.Factorial(); // 3628800
20.Factorial(); // 2432902008176640000L  (max supported)

(-1).Factorial(); // throws ArgumentOutOfRangeException
21.Factorial();   // throws ArgumentOutOfRangeException
```

> Supports `n` in `[0, 20]` — all values that fit in `long`.

### DigitCount & ToDigits

```csharp
0.DigitCount();       // 1
99.DigitCount();      // 2
(-1234).DigitCount(); // 4

1234.ToDigits();      // [1, 2, 3, 4]
(-567).ToDigits();    // [5, 6, 7]   (absolute value)
0.ToDigits();         // [0]

123456L.ToDigits();   // [1, 2, 3, 4, 5, 6]
```

### ToOrdinal & ToRoman

```csharp
1.ToOrdinal();    // "1st"
2.ToOrdinal();    // "2nd"
3.ToOrdinal();    // "3rd"
11.ToOrdinal();   // "11th"
21.ToOrdinal();   // "21st"

1.ToRoman();      // "I"
4.ToRoman();      // "IV"
9.ToRoman();      // "IX"
14.ToRoman();     // "XIV"
2024.ToRoman();   // "MMXXIV"
```

### IsMultipleOf & Pow

```csharp
12.IsMultipleOf(4);     // true
12.IsMultipleOf(5);     // false
0.IsMultipleOf(7);      // true  (0 is multiple of everything)

2.Pow(10);              // 1024
3.Pow(3);               // 27
5.Pow(0);               // 1

2L.IsMultipleOf(2L);    // true
2L.Pow(10);             // 1024L
```

### double Linear Interpolation & Angle Conversion

```csharp
// Lerp: a + t*(b-a)
0.0d.Lerp(0d, 100d);    // 0.0
0.5d.Lerp(0d, 100d);    // 50.0
1.0d.Lerp(0d, 100d);    // 100.0

// Normalize: scale to [0, 1]
50d.Normalize(0d, 100d);    // 0.5
0d.Normalize(0d, 100d);     // 0.0
100d.Normalize(0d, 100d);   // 1.0

// Angle conversion
180d.ToRadians();           // Math.PI  (≈3.14159…)
Math.PI.ToDegrees();        // 180.0
90d.ToRadians().ToDegrees(); // 90.0  (round-trip)
```

### double Predicates

```csharp
double.NaN.IsNaN();                // true
1.5d.IsNaN();                      // false

double.PositiveInfinity.IsInfinity(); // true
double.NegativeInfinity.IsInfinity(); // true
1.0d.IsInfinity();                    // false

3.14d.IsFinite();                  // true
double.NaN.IsFinite();             // false
double.PositiveInfinity.IsFinite();// false
```

### double RoundTo

```csharp
2.345d.RoundTo(2);    // 2.35
2.5d.RoundTo(0);      // 3.0
(-2.345d).RoundTo(2); // -2.35
```

### decimal Operations

```csharp
// Rounding
2.345m.RoundTo(2);    // 2.35m
(-2.345m).RoundTo(2); // -2.35m

// Percentage: value / total * 100
25m.Percentage(200m); // 12.5m
100m.Percentage(100m);// 100m
10m.Percentage(0m);   // 0m  (zero-safe)

// Sign
5m.IsPositive();      // true
(-3m).IsNegative();   // true
0m.IsZero();          // true

// Clamp
150m.Clamp(0m, 100m); // 100m

// Currency
1234.56m.ToCurrencyString("en-US"); // "$1,234.56"
1234.56m.ToCurrencyString();        // uses current culture

// Abs
(-42.5m).Abs(); // 42.5m

// IsBetween
50m.IsBetween(0m, 100m);     // true
150m.IsBetween(0m, 100m);    // false

// ToNearest
7.3m.ToNearest(0.5m);        // 7.5m
10.1m.ToNearest(5m);         // 10m
```

---

## Performance Notes

- All sign checks, parity, `Abs`, `IsNaN`, `IsInfinity`, `IsFinite`, `IsZero` are decorated with `[MethodImpl(AggressiveInlining)]` — JIT-inlinable to single instructions.
- `IsEven` / `IsOdd` use bitwise `&` instead of modulo — avoids division.
- `IsPrime` uses trial division up to `√n` with early exit for even numbers — O(√n).
- `Factorial` is an iterative loop with no recursion overhead.
- `DigitCount` uses `Math.Log10` — O(1) regardless of number size.
- `ToDigits` allocates exactly one `int[]` of the right size — no resizing.
