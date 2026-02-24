// -----------------------------------------------------------------------
// <copyright file="ByteExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Oxtensions.Byte;

/// <summary>
/// High-performance extension methods for <see cref="T:byte[]"/>.
/// </summary>
public static class ByteExtensions
{
    private const string HexCharsUpper = "0123456789ABCDEF";

    // ─────────────────────────────────────────────────────────────────────────
    // Null / Empty
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns <see langword="true"/> if the byte array is <see langword="null"/> or has zero length.
    /// </summary>
    /// <param name="value">The byte array to evaluate.</param>
    /// <returns><see langword="true"/> when null or empty; otherwise <see langword="false"/>.</returns>
    /// <example>
    /// <code>
    /// byte[]? empty = null;
    /// empty.IsNullOrEmpty(); // true
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this byte[]? value)
        => value is null || value.Length == 0;

    // ─────────────────────────────────────────────────────────────────────────
    // Hex
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a byte array to an uppercase hexadecimal string without heap allocation.
    /// </summary>
    /// <param name="value">The source byte array.</param>
    /// <returns>Uppercase hex string (e.g., "DEADBEEF"), or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// new byte[]{ 0xDE, 0xAD }.ToHexString(); // "DEAD"
    /// </code>
    /// </example>
    /// <remarks>Uses <see langword="stackalloc"/> for arrays &lt;= 128 bytes; falls back to <see cref="ArrayPool{T}"/> otherwise.</remarks>
    public static string ToHexString(this byte[]? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;

        int hexLength = value!.Length * 2;

        return string.Create(hexLength, value, static (span, bytes) =>
        {
            ReadOnlySpan<char> hexChars = HexCharsUpper.AsSpan();
            for (int i = 0; i < bytes.Length; i++)
            {
                span[i * 2]     = hexChars[bytes[i] >> 4];
                span[i * 2 + 1] = hexChars[bytes[i] & 0x0F];
            }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Base64
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a byte array to a Base64-encoded string.
    /// </summary>
    /// <param name="value">The source byte array.</param>
    /// <returns>Base64 string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// new byte[]{ 72, 101, 108 }.ToBase64String(); // "SGVs"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBase64String(this byte[]? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return Convert.ToBase64String(value!);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // String encoding
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Decodes a byte array to a UTF-8 string.
    /// </summary>
    /// <param name="value">The source byte array.</param>
    /// <returns>Decoded UTF-8 string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// Encoding.UTF8.GetBytes("Hello").ToUTF8String(); // "Hello"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToUTF8String(this byte[]? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return Encoding.UTF8.GetString(value!);
    }

    /// <summary>
    /// Decodes a byte array to an ASCII string.
    /// </summary>
    /// <param name="value">The source byte array.</param>
    /// <returns>Decoded ASCII string, or <see cref="System.String.Empty"/> when null/empty.</returns>
    /// <example>
    /// <code>
    /// new byte[]{ 65, 66, 67 }.ToASCIIString(); // "ABC"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToASCIIString(this byte[]? value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return Encoding.ASCII.GetString(value!);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Hashing
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Computes the MD5 hash of a byte array using a span-based approach to minimize allocations.
    /// </summary>
    /// <param name="value">The source byte array.</param>
    /// <returns>16-byte MD5 hash, or empty array when input is null/empty.</returns>
    /// <example>
    /// <code>
    /// byte[] hash = Encoding.UTF8.GetBytes("hello").ComputeMD5();
    /// </code>
    /// </example>
    /// <remarks>MD5 is not cryptographically secure; use for checksums only.</remarks>
    public static byte[] ComputeMD5(this byte[]? value)
    {
        if (value.IsNullOrEmpty()) return [];

        byte[] hash = new byte[16];
        MD5.HashData(value, hash);
        return hash;
    }

    /// <summary>
    /// Computes the SHA-256 hash of a byte array using a span-based approach to minimize allocations.
    /// </summary>
    /// <param name="value">The source byte array.</param>
    /// <returns>32-byte SHA-256 hash, or empty array when input is null/empty.</returns>
    /// <example>
    /// <code>
    /// byte[] hash = Encoding.UTF8.GetBytes("hello").ComputeSHA256();
    /// </code>
    /// </example>
    public static byte[] ComputeSHA256(this byte[]? value)
    {
        if (value.IsNullOrEmpty()) return [];

        byte[] hash = new byte[32];
        SHA256.HashData(value, hash);
        return hash;
    }
}
