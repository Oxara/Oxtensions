// -----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace Oxtensions.Streams;

/// <summary>
/// High-performance extension methods for <see cref="Stream"/> and <c>byte[]</c>.
/// </summary>
public static class StreamExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Stream → byte[] / string
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Reads the entire stream and returns its contents as a byte array.
    /// Rewinds the stream to position 0 first if seekable.
    /// </summary>
    /// <param name="stream">The source stream.</param>
    /// <returns>All bytes from the stream.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
    public static byte[] ToByteArray(this Stream stream)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        if (stream.CanSeek) stream.Position = 0;
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Reads the entire stream and returns its contents as a Base64-encoded string.
    /// </summary>
    /// <param name="stream">The source stream.</param>
    /// <returns>Base64 string representation of the stream contents.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBase64String(this Stream stream)
        => Convert.ToBase64String(stream.ToByteArray());

    /// <summary>
    /// Reads the stream as text using the specified encoding (defaults to UTF-8).
    /// </summary>
    /// <param name="stream">The source stream.</param>
    /// <param name="encoding">Text encoding to use. Defaults to <see cref="Encoding.UTF8"/>.</param>
    /// <returns>Text content of the stream.</returns>
    public static string ReadAllText(this Stream stream, Encoding? encoding = null)
    {
        if (stream.CanSeek) stream.Position = 0;
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8, leaveOpen: true);
        return reader.ReadToEnd();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Compression
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Compresses the stream contents using GZip and returns the compressed bytes.
    /// </summary>
    /// <param name="stream">The source stream to compress.</param>
    /// <returns>GZip-compressed byte array.</returns>
    /// <example>
    /// <code>
    /// var compressed = myStream.CompressGzip();
    /// </code>
    /// </example>
    public static byte[] CompressGzip(this Stream stream)
    {
        var source = stream.ToByteArray();
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, leaveOpen: true))
            gzip.Write(source, 0, source.Length);
        return output.ToArray();
    }

    /// <summary>
    /// Decompresses a GZip-compressed stream and returns the original bytes.
    /// </summary>
    /// <param name="stream">The GZip-compressed stream.</param>
    /// <returns>Decompressed byte array.</returns>
    /// <example>
    /// <code>
    /// var original = compressedStream.DecompressGzip();
    /// </code>
    /// </example>
    public static byte[] DecompressGzip(this Stream stream)
    {
        if (stream.CanSeek) stream.Position = 0;
        using var gzip = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }
}
