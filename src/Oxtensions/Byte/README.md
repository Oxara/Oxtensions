# ByteExtensions

> `using Oxtensions.Byte;`

Zero-allocation-oriented extension methods for `byte[]` — hex encoding, Base64, UTF-8/ASCII decode, MD5, and SHA-256.

---

## Methods at a Glance

| Method | Description |
|--------|-------------|
| `IsNullOrEmpty()` | Null or zero-length check |
| `ToHexString()` | Convert to lowercase hex string (zero-alloc) |
| `ToBase64String()` | Convert to Base64 string |
| `ToUTF8String()` | Decode bytes as UTF-8 |
| `ToASCIIString()` | Decode bytes as ASCII |
| `ComputeMD5()` | Compute MD5 hash (hex string) |
| `ComputeSHA256()` | Compute SHA-256 hash (hex string) |

---

## Usage Examples

### Null / Empty Check

```csharp
byte[]? data = null;
data.IsNullOrEmpty();        // true

Array.Empty<byte>().IsNullOrEmpty(); // true

new byte[] { 1, 2, 3 }.IsNullOrEmpty(); // false
```

### Hex String

```csharp
byte[] bytes = { 0xDE, 0xAD, 0xBE, 0xEF };
bytes.ToHexString();  // "deadbeef"

new byte[] { 0, 15, 255 }.ToHexString(); // "000fff"
```

> Uses `string.Create` with a stack-friendly lookup — a single allocation with no intermediate `StringBuilder`.

### Base64

```csharp
byte[] data = System.Text.Encoding.UTF8.GetBytes("Hello");
data.ToBase64String(); // "SGVsbG8="
```

### Text Decoding

```csharp
byte[] utf8 = { 72, 101, 108, 108, 111 }; // "Hello"
utf8.ToUTF8String();    // "Hello"
utf8.ToASCIIString();   // "Hello"
```

### Hashing

```csharp
byte[] input = System.Text.Encoding.UTF8.GetBytes("hello");

input.ComputeMD5();
// "5d41402abc4b2a76b9719d911017c592"

input.ComputeSHA256();
// "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824"
```

Real-world use case — verifying a downloaded file:

```csharp
byte[] fileBytes = File.ReadAllBytes("package.zip");
string actualHash   = fileBytes.ComputeSHA256();
string expectedHash = "2cf24dba...";

if (actualHash != expectedHash)
    throw new InvalidDataException("Checksum mismatch!");
```

---

## Performance Notes

- `ToHexString` uses `string.Create` with a `Span`-based hex lookup table — single allocation, no `StringBuilder`.
- `ComputeMD5` / `ComputeSHA256` use the one-shot `MD5.HashData` / `SHA256.HashData` span overloads introduced in .NET 5, avoiding manual `using` blocks and array copies.
- All methods guard against `null` input and return empty string / throw `ArgumentNullException` as appropriate.
