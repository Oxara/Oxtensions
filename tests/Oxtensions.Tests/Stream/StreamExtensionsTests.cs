// -----------------------------------------------------------------------
// <copyright file="StreamExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.IO;
using System.Text;
using FluentAssertions;
using Oxtensions.Streams;
using Xunit;

public sealed class StreamExtensions_ToByteArrayTests
{
    [Fact]
    public void ToByteArray_ReturnsAllBytes()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        stream.ToByteArray().Should().Equal(data);
    }

    [Fact]
    public void ToByteArray_EmptyStream_ReturnsEmptyArray()
    {
        using var stream = new MemoryStream();
        stream.ToByteArray().Should().BeEmpty();
    }
}

public sealed class StreamExtensions_ToBase64StringTests
{
    [Fact]
    public void ToBase64String_RoundTrips()
    {
        var data = Encoding.UTF8.GetBytes("hello world");
        using var stream = new MemoryStream(data);
        var base64 = stream.ToBase64String();
        Convert.FromBase64String(base64).Should().Equal(data);
    }
}

public sealed class StreamExtensions_ReadAllTextTests
{
    [Fact]
    public void ReadAllText_Utf8_ReturnsString()
    {
        const string text = "Hello, World!";
        var data = Encoding.UTF8.GetBytes(text);
        using var stream = new MemoryStream(data);
        stream.ReadAllText().Should().Be(text);
    }

    [Fact]
    public void ReadAllText_CustomEncoding_ReturnsString()
    {
        const string text = "Hello";
        var data = Encoding.Unicode.GetBytes(text);
        using var stream = new MemoryStream(data);
        stream.ReadAllText(Encoding.Unicode).Should().Be(text);
    }
}

public sealed class StreamExtensions_GzipTests
{
    [Fact]
    public void CompressGzip_ThenDecompressGzip_RoundTrips()
    {
        const string text = "The quick brown fox jumps over the lazy dog. Repeated to make it compressible. " +
                            "The quick brown fox jumps over the lazy dog.";
        var original = Encoding.UTF8.GetBytes(text);
        using var inputStream = new MemoryStream(original);

        var compressed = inputStream.CompressGzip();
        compressed.Length.Should().BeGreaterThan(0);
        compressed.Should().NotEqual(original);

        using var compressedStream = new MemoryStream(compressed);
        var decompressed = compressedStream.DecompressGzip();
        decompressed.Should().Equal(original);
    }
}

public sealed class ByteStreamExtensions_ToStreamTests
{
    [Fact]
    public void ToStream_ByteArray_ReturnsReadableStream()
    {
        var data = new byte[] { 10, 20, 30 };
        using var stream = data.ToStream();
        stream.Length.Should().Be(3);
        stream.ToByteArray().Should().Equal(data);
    }

    [Fact]
    public void CompressGzip_ThenDecompressGzip_RoundTrips()
    {
        var data = Encoding.UTF8.GetBytes("byte extension gzip round-trip test ".PadRight(100, 'x'));
        var compressed = data.CompressGzip();
        compressed.Should().NotEqual(data);
        var restored = compressed.DecompressGzip();
        restored.Should().Equal(data);
    }
}
