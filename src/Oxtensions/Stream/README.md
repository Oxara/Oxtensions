# Streams Extensions

> Namespace: `Oxtensions.Streams`

Convenience extensions on `System.IO.Stream` and `byte[]` for reading, encoding, and GZip compression/decompression — all without third-party dependencies.

---

## Classes

| Class | Extends | Description |
|-------|---------|-------------|
| `StreamExtensions` | `System.IO.Stream` | Read all bytes/text, Base64 encode, GZip compress/decompress |
| `ByteStreamExtensions` | `byte[]` | Wrap in a `MemoryStream`, GZip compress/decompress |

---

## Methods — StreamExtensions

| Method | Description |
|--------|-------------|
| `ToByteArray()` | Reads the entire stream and returns a `byte[]`. Rewinds seekable streams first. |
| `ToBase64String()` | Reads the stream and returns a Base64-encoded string |
| `ReadAllText(Encoding?)` | Reads the stream as text. Defaults to UTF-8. |
| `CompressGzip()` | Compresses stream contents with GZip; returns compressed `byte[]` |
| `DecompressGzip()` | Decompresses a GZip-compressed stream; returns decompressed `byte[]` |

## Methods — ByteStreamExtensions

| Method | Description |
|--------|-------------|
| `ToStream()` | Wraps the byte array in a read-only `MemoryStream` |
| `CompressGzip()` | GZip-compresses the byte array; returns compressed `byte[]` |
| `DecompressGzip()` | GZip-decompresses the byte array; returns original `byte[]` |

---

## Usage Examples

```csharp
using Oxtensions.Streams;

// Stream → byte[]
using var stream = File.OpenRead("file.bin");
byte[] all = stream.ToByteArray();

// Stream → text
using var fs = File.OpenRead("file.txt");
string text = fs.ReadAllText();              // UTF-8 by default
string utf16 = fs.ReadAllText(Encoding.Unicode);

// GZip round-trip via Stream
byte[] original = Encoding.UTF8.GetBytes("compress me!");
using var ms = new MemoryStream(original);
byte[] compressed   = ms.CompressGzip();
using var cms = new MemoryStream(compressed);
byte[] decompressed = cms.DecompressGzip(); // == original

// GZip round-trip via byte[]
byte[] compressed2   = original.CompressGzip();
byte[] decompressed2 = compressed2.DecompressGzip(); // == original

// byte[] → MemoryStream
using var s = original.ToStream();
```
