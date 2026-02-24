// -----------------------------------------------------------------------
// <copyright file="CharExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Char;
using Xunit;

public sealed class CharExtensions_IsVowelTests
{
    [Theory]
    [InlineData('a', true)]
    [InlineData('e', true)]
    [InlineData('i', true)]
    [InlineData('o', true)]
    [InlineData('u', true)]
    [InlineData('A', true)]
    [InlineData('E', true)]
    [InlineData('b', false)]
    [InlineData('z', false)]
    [InlineData('1', false)]
    public void IsVowel_ReturnsExpected(char c, bool expected)
        => c.IsVowel().Should().Be(expected);
}

public sealed class CharExtensions_IsConsonantTests
{
    [Theory]
    [InlineData('b', true)]
    [InlineData('z', true)]
    [InlineData('B', true)]
    [InlineData('a', false)]
    [InlineData('1', false)]
    [InlineData(' ', false)]
    public void IsConsonant_ReturnsExpected(char c, bool expected)
        => c.IsConsonant().Should().Be(expected);
}

public sealed class CharExtensions_IsAsciiLetterTests
{
    [Theory]
    [InlineData('a', true)]
    [InlineData('z', true)]
    [InlineData('A', true)]
    [InlineData('Z', true)]
    [InlineData('1', false)]
    [InlineData(' ', false)]
    public void IsAsciiLetter_ReturnsExpected(char c, bool expected)
        => c.IsAsciiLetter().Should().Be(expected);
}

public sealed class CharExtensions_IsAsciiDigitTests
{
    [Theory]
    [InlineData('0', true)]
    [InlineData('9', true)]
    [InlineData('a', false)]
    [InlineData(' ', false)]
    public void IsAsciiDigit_ReturnsExpected(char c, bool expected)
        => c.IsAsciiDigit().Should().Be(expected);

    [Theory]
    [InlineData('a', true)]
    [InlineData('0', true)]
    [InlineData(' ', false)]
    [InlineData('@', false)]
    public void IsAsciiLetterOrDigit_ReturnsExpected(char c, bool expected)
        => c.IsAsciiLetterOrDigit().Should().Be(expected);
}

public sealed class CharExtensions_IsWhitespaceTests
{
    [Theory]
    [InlineData(' ',  true)]
    [InlineData('\t', true)]
    [InlineData('\n', true)]
    [InlineData('a',  false)]
    [InlineData('1',  false)]
    public void IsWhitespace_ReturnsExpected(char c, bool expected)
        => c.IsWhitespace().Should().Be(expected);
}

public sealed class CharExtensions_IsNewLineTests
{
    [Theory]
    [InlineData('\n', true)]
    [InlineData('\r', true)]
    [InlineData(' ',  false)]
    [InlineData('a',  false)]
    public void IsNewLine_ReturnsExpected(char c, bool expected)
        => c.IsNewLine().Should().Be(expected);
}

public sealed class CharExtensions_CaseTests
{
    [Theory]
    [InlineData('A', true)]
    [InlineData('Z', true)]
    [InlineData('a', false)]
    [InlineData('1', false)]
    public void IsUppercase_ReturnsExpected(char c, bool expected)
        => c.IsUppercase().Should().Be(expected);

    [Theory]
    [InlineData('a', true)]
    [InlineData('z', true)]
    [InlineData('A', false)]
    [InlineData('1', false)]
    public void IsLowercase_ReturnsExpected(char c, bool expected)
        => c.IsLowercase().Should().Be(expected);
}
