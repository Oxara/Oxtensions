// -----------------------------------------------------------------------
// <copyright file="ByteExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Text;
using FluentAssertions;
using Oxtensions.Byte;
using Xunit;

namespace Oxtensions.Tests.Byte;

public sealed class ByteExtensionsTests
{
    // ── IsNullOrEmpty ────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullArray_ReturnsTrue()
        => ((byte[]?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyArray_ReturnsTrue()
        => Array.Empty<byte>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptyArray_ReturnsFalse()
        => new byte[] { 1 }.IsNullOrEmpty().Should().BeFalse();

    // ── ToHexString ──────────────────────────────────────────────────────────

    [Fact]
    public void ToHexString_KnownBytes_ReturnsUpperHex()
        => new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }.ToHexString().Should().Be("DEADBEEF");

    [Fact]
    public void ToHexString_NullArray_ReturnsEmpty()
        => ((byte[]?)null).ToHexString().Should().Be(string.Empty);

    [Fact]
    public void ToHexString_SingleByte_ReturnsTwoChars()
        => new byte[] { 0x0A }.ToHexString().Should().Be("0A");

    [Theory]
    [InlineData(new byte[] { 0x00 }, "00")]
    [InlineData(new byte[] { 0xFF }, "FF")]
    [InlineData(new byte[] { 0x01, 0x02 }, "0102")]
    public void ToHexString_VariousBytes_ReturnsExpected(byte[] input, string expected)
        => input.ToHexString().Should().Be(expected);

    // ── ToBase64String ───────────────────────────────────────────────────────

    [Fact]
    public void ToBase64String_ValidBytes_ReturnsBase64()
        => Encoding.UTF8.GetBytes("Hello").ToBase64String().Should().Be("SGVsbG8=");

    [Fact]
    public void ToBase64String_NullArray_ReturnsEmpty()
        => ((byte[]?)null).ToBase64String().Should().Be(string.Empty);

    [Fact]
    public void ToBase64String_EmptyArray_ReturnsEmpty()
        => Array.Empty<byte>().ToBase64String().Should().Be(string.Empty);

    // ── ToUTF8String ─────────────────────────────────────────────────────────

    [Fact]
    public void ToUTF8String_ValidBytes_ReturnsString()
        => Encoding.UTF8.GetBytes("Hello").ToUTF8String().Should().Be("Hello");

    [Fact]
    public void ToUTF8String_NullArray_ReturnsEmpty()
        => ((byte[]?)null).ToUTF8String().Should().Be(string.Empty);

    [Fact]
    public void ToUTF8String_TurkishChars_RoundTrips()
    {
        const string original = "Merhaba Dünya";
        Encoding.UTF8.GetBytes(original).ToUTF8String().Should().Be(original);
    }

    // ── ToASCIIString ─────────────────────────────────────────────────────────

    [Fact]
    public void ToASCIIString_ValidAscii_ReturnsString()
        => new byte[] { 65, 66, 67 }.ToASCIIString().Should().Be("ABC");

    [Fact]
    public void ToASCIIString_NullArray_ReturnsEmpty()
        => ((byte[]?)null).ToASCIIString().Should().Be(string.Empty);

    // ── ComputeMD5 ────────────────────────────────────────────────────────────

    [Fact]
    public void ComputeMD5_ValidInput_Returns16Bytes()
        => Encoding.UTF8.GetBytes("hello").ComputeMD5().Should().HaveCount(16);

    [Fact]
    public void ComputeMD5_NullInput_ReturnsEmpty()
        => ((byte[]?)null).ComputeMD5().Should().BeEmpty();

    [Fact]
    public void ComputeMD5_SameInput_ReturnsSameHash()
    {
        byte[] input = Encoding.UTF8.GetBytes("test");
        input.ComputeMD5().Should().Equal(input.ComputeMD5());
    }

    // ── ComputeSHA256 ─────────────────────────────────────────────────────────

    [Fact]
    public void ComputeSHA256_ValidInput_Returns32Bytes()
        => Encoding.UTF8.GetBytes("hello").ComputeSHA256().Should().HaveCount(32);

    [Fact]
    public void ComputeSHA256_NullInput_ReturnsEmpty()
        => ((byte[]?)null).ComputeSHA256().Should().BeEmpty();

    [Fact]
    public void ComputeSHA256_KnownValue_ReturnsExpectedHash()
    {
        // SHA256("abc") = ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad
        byte[] input = Encoding.UTF8.GetBytes("abc");
        string hex = input.ComputeSHA256().ToHexString();
        hex.Should().Be("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD");
    }
}
