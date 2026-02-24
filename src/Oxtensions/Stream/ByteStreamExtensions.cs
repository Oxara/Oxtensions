// -----------------------------------------------------------------------
// <copyright file="ByteStreamExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace Oxtensions.Streams;

/// <summary>
/// Extension methods adding stream-conversion helpers to <c>byte[]</c>.
/// </summary>
public static class ByteStreamExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // byte[] → Stream
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Wraps the byte array in a <see cref="MemoryStream"/>.
    /// </summary>
    /// <param name="value">The byte array.</param>
    /// <returns>A readable <see cref="MemoryStream"/> positioned at offset 0.</returns>
    /// <example>
    /// <code>
    /// byte[] data = ...;
    /// using var stream = data.ToStream();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemoryStream ToStream(this byte[] value)
        => new(value, writable: false);

    /// <summary>
    /// Compresses the byte array using GZip and returns the compressed bytes.
    /// </summary>
    /// <param name="value">The bytes to compress.</param>
    /// <returns>GZip-compressed byte array.</returns>
    public static byte[] CompressGzip(this byte[] value)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, leaveOpen: true))
            gzip.Write(value, 0, value.Length);
        return output.ToArray();
    }

    /// <summary>
    /// Decompresses a GZip-compressed byte array and returns the original bytes.
    /// </summary>
    /// <param name="value">The compressed bytes.</param>
    /// <returns>Decompressed byte array.</returns>
    public static byte[] DecompressGzip(this byte[] value)
    {
        using var input  = new MemoryStream(value);
        using var gzip   = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }
}
