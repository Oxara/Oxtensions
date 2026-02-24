// -----------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.String;
using Xunit;

namespace Oxtensions.Tests.String;

public sealed class StringExtensionsTests
{
    // ── IsNullOrEmpty ────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullString_ReturnsTrue()
        => ((string?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyString_ReturnsTrue()
        => string.Empty.IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptyString_ReturnsFalse()
        => "a".IsNullOrEmpty().Should().BeFalse();

    [Theory]
    [InlineData("hello", false)]
    [InlineData(" ", false)]
    [InlineData("", true)]
    public void IsNullOrEmpty_VariousInputs_ReturnsExpected(string input, bool expected)
        => input.IsNullOrEmpty().Should().Be(expected);

    // ── IsNullOrWhiteSpace ───────────────────────────────────────────────────

    [Fact]
    public void IsNullOrWhiteSpace_NullString_ReturnsTrue()
        => ((string?)null).IsNullOrWhiteSpace().Should().BeTrue();

    [Fact]
    public void IsNullOrWhiteSpace_WhitespaceOnly_ReturnsTrue()
        => "   ".IsNullOrWhiteSpace().Should().BeTrue();

    [Fact]
    public void IsNullOrWhiteSpace_NonWhitespace_ReturnsFalse()
        => "hello".IsNullOrWhiteSpace().Should().BeFalse();

    // ── ToSlug ───────────────────────────────────────────────────────────────

    [Fact]
    public void ToSlug_AccentedInput_ReturnsSlugged()
        => "Hello Wörld! Naïve".ToSlug().Should().Be("hello-world-naive");

    [Fact]
    public void ToSlug_NullInput_ReturnsEmpty()
        => ((string?)null).ToSlug().Should().Be(string.Empty);

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("  spaces  ", "spaces")]
    [InlineData("foo--bar", "foo-bar")]
    [InlineData("SEO Friendly URL", "seo-friendly-url")]
    public void ToSlug_VariousInputs_ReturnsExpected(string input, string expected)
        => input.ToSlug().Should().Be(expected);

    // ── Truncate ─────────────────────────────────────────────────────────────

    [Fact]
    public void Truncate_LongerThanMax_TruncatesWithSuffix()
        => "Hello World".Truncate(7).Should().Be("Hell...");

    [Fact]
    public void Truncate_NullInput_ReturnsEmpty()
        => ((string?)null).Truncate(5).Should().Be(string.Empty);

    [Fact]
    public void Truncate_ShorterThanMax_ReturnsOriginal()
        => "Hi".Truncate(10).Should().Be("Hi");

    [Theory]
    [InlineData("abcdef", 4, "...", "a...")]
    [InlineData("ab", 10, "...", "ab")]
    [InlineData("Hello World", 5, "---", "He---")]
    public void Truncate_EdgeCases_ReturnsExpected(string input, int max, string suffix, string expected)
        => input.Truncate(max, suffix).Should().Be(expected);

    // ── Casing ───────────────────────────────────────────────────────────────

    [Fact]
    public void ToPascalCase_SpaceSeparated_ReturnsPascal()
        => "hello world".ToPascalCase().Should().Be("HelloWorld");

    [Fact]
    public void ToCamelCase_SpaceSeparated_ReturnsCamel()
        => "hello world".ToCamelCase().Should().Be("helloWorld");

    [Fact]
    public void ToSnakeCase_MixedInput_ReturnsSnake()
        => "Hello World".ToSnakeCase().Should().Be("hello_world");

    [Fact]
    public void ToKebabCase_MixedInput_ReturnsKebab()
        => "Hello World".ToKebabCase().Should().Be("hello-world");

    [Theory]
    [InlineData("foo bar baz", "FooBarBaz")]
    [InlineData("foo_bar", "FooBar")]
    public void ToPascalCase_VariousInputs_ReturnsExpected(string input, string expected)
        => input.ToPascalCase().Should().Be(expected);

    // ── RemoveAccents ─────────────────────────────────────────────────────────

    [Fact]
    public void RemoveAccents_AccentedChars_ReturnsAscii()
        => "café résumé façade".RemoveAccents().Should().Be("cafe resume facade");

    [Fact]
    public void RemoveAccents_LatinExtendedChars_ReturnsAscii()
        => "Türkçe Şehir İstanbul".RemoveAccents().Should().Be("Turkce Sehir Istanbul");

    [Fact]
    public void RemoveAccents_NullInput_ReturnsEmpty()
        => ((string?)null).RemoveAccents().Should().Be(string.Empty);

    [Fact]
    public void RemoveAccents_AlreadyAscii_ReturnsUnchanged()
        => "Hello".RemoveAccents().Should().Be("Hello");

    // ── ContainsIgnoreCase ────────────────────────────────────────────────────

    [Fact]
    public void ContainsIgnoreCase_MatchingDifferentCase_ReturnsTrue()
        => "Hello World".ContainsIgnoreCase("WORLD").Should().BeTrue();

    [Fact]
    public void ContainsIgnoreCase_NullValue_ReturnsFalse()
        => ((string?)null).ContainsIgnoreCase("test").Should().BeFalse();

    [Fact]
    public void ContainsIgnoreCase_NoMatch_ReturnsFalse()
        => "Hello".ContainsIgnoreCase("xyz").Should().BeFalse();

    // ── Safe Parse ────────────────────────────────────────────────────────────

    [Fact]
    public void ToInt32OrDefault_ValidNumber_ReturnsParsed()
        => "42".ToInt32OrDefault().Should().Be(42);

    [Fact]
    public void ToInt32OrDefault_InvalidInput_ReturnsDefault()
        => "abc".ToInt32OrDefault(-1).Should().Be(-1);

    [Fact]
    public void ToInt32OrDefault_NullInput_ReturnsDefault()
        => ((string?)null).ToInt32OrDefault(99).Should().Be(99);

    [Fact]
    public void ToDecimalOrDefault_ValidDecimal_ReturnsParsed()
        => "3.14".ToDecimalOrDefault().Should().Be(3.14m);

    [Fact]
    public void ToDecimalOrDefault_InvalidInput_ReturnsDefault()
        => "nope".ToDecimalOrDefault(1.5m).Should().Be(1.5m);

    [Fact]
    public void ToGuidOrDefault_ValidGuid_ReturnsParsed()
    {
        var guid = System.Guid.NewGuid();
        guid.ToString().ToGuidOrDefault().Should().Be(guid);
    }

    [Fact]
    public void ToGuidOrDefault_InvalidInput_ReturnsEmpty()
        => "not-a-guid".ToGuidOrDefault().Should().Be(System.Guid.Empty);

    // ── Mask ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Mask_StandardCardNumber_MasksCorrectly()
        => "1234567890".Mask(2, 2).Should().Be("12******90");

    [Fact]
    public void Mask_NullInput_ReturnsEmpty()
        => ((string?)null).Mask(2, 2).Should().Be(string.Empty);

    [Fact]
    public void Mask_VisibleRegionsEqualLength_ReturnsOriginal()
        => "1234".Mask(2, 2).Should().Be("1234");

    [Theory]
    [InlineData("12345678901234", 4, 4, '*', "1234******1234")]
    [InlineData("ABCDEFGH", 1, 1, '#', "A######H")]
    public void Mask_VariousConfigs_ReturnsExpected(string input, int start, int end, char ch, string expected)
        => input.Mask(start, end, ch).Should().Be(expected);

    // ── Validation ────────────────────────────────────────────────────────────

    [Fact]
    public void IsValidEmail_ValidEmail_ReturnsTrue()
        => "user@example.com".IsValidEmail().Should().BeTrue();

    [Fact]
    public void IsValidEmail_InvalidEmail_ReturnsFalse()
        => "not-an-email".IsValidEmail().Should().BeFalse();

    [Fact]
    public void IsValidEmail_NullInput_ReturnsFalse()
        => ((string?)null).IsValidEmail().Should().BeFalse();

    [Fact]
    public void IsValidUrl_ValidHttps_ReturnsTrue()
        => "https://example.com".IsValidUrl().Should().BeTrue();

    [Fact]
    public void IsValidUrl_InvalidUrl_ReturnsFalse()
        => "not-a-url".IsValidUrl().Should().BeFalse();

    [Fact]
    public void IsValidPhoneNumber_ValidPhone_ReturnsTrue()
        => "+90 532 000 0000".IsValidPhoneNumber().Should().BeTrue();

    [Fact]
    public void IsValidPhoneNumber_TooShort_ReturnsFalse()
        => "123".IsValidPhoneNumber().Should().BeFalse();

    // ── Repeat ────────────────────────────────────────────────────────────────

    [Fact]
    public void Repeat_ValidCount_ReturnsRepeated()
        => "ab".Repeat(3).Should().Be("ababab");

    [Fact]
    public void Repeat_ZeroCount_ReturnsEmpty()
        => "ab".Repeat(0).Should().Be(string.Empty);

    [Fact]
    public void Repeat_NullInput_ReturnsEmpty()
        => ((string?)null).Repeat(3).Should().Be(string.Empty);

    // ── ReverseWords / Reverse ────────────────────────────────────────────────

    [Fact]
    public void ReverseWords_ThreeWords_ReversedOrder()
        => "Hello World Foo".ReverseWords().Should().Be("Foo World Hello");

    [Fact]
    public void ReverseWords_NullInput_ReturnsEmpty()
        => ((string?)null).ReverseWords().Should().Be(string.Empty);

    [Fact]
    public void Reverse_SimpleString_ReturnsReversed()
        => "hello".Reverse().Should().Be("olleh");

    [Fact]
    public void Reverse_NullInput_ReturnsEmpty()
        => ((string?)null).Reverse().Should().Be(string.Empty);

    // ── CountOccurrences ─────────────────────────────────────────────────────

    [Fact]
    public void CountOccurrences_MultipleMatches_ReturnsCount()
        => "banana".CountOccurrences("an").Should().Be(2);

    [Fact]
    public void CountOccurrences_NoMatch_ReturnsZero()
        => "banana".CountOccurrences("xyz").Should().Be(0);

    [Fact]
    public void CountOccurrences_NullInput_ReturnsZero()
        => ((string?)null).CountOccurrences("a").Should().Be(0);

    // ── Left / Right ─────────────────────────────────────────────────────────

    [Fact]
    public void Left_ShorterThanLength_ReturnsOriginal()
        => "Hi".Left(10).Should().Be("Hi");

    [Fact]
    public void Left_ValidLength_ReturnsTrimmed()
        => "Hello World".Left(5).Should().Be("Hello");

    [Fact]
    public void Right_ValidLength_ReturnsTrimmed()
        => "Hello World".Right(5).Should().Be("World");

    // ── HTML / Base64 ─────────────────────────────────────────────────────────

    [Fact]
    public void RemoveHtmlTags_HtmlInput_ReturnsPlainText()
        => "<b>Hello</b> World".RemoveHtmlTags().Should().Be("Hello World");

    [Fact]
    public void RemoveHtmlTags_NullInput_ReturnsEmpty()
        => ((string?)null).RemoveHtmlTags().Should().Be(string.Empty);

    [Fact]
    public void ToBase64_ThenFromBase64_RoundTrips()
    {
        const string original = "Hello, Dünya!";
        original.ToBase64().FromBase64().Should().Be(original);
    }

    [Fact]
    public void FromBase64_InvalidInput_ReturnsEmpty()
        => "!!!invalid!!!".FromBase64().Should().Be(string.Empty);
}

public sealed class StringExtensions_SplitAndTrimTests
{
    [Fact]
    public void SplitAndTrim_StandardInput_SplitsAndTrims()
        => " a , b , c ".SplitAndTrim(',').Should().Equal("a", "b", "c");

    [Fact]
    public void SplitAndTrim_NullInput_ReturnsEmptyArray()
        => ((string?)null).SplitAndTrim(',').Should().BeEmpty();

    [Fact]
    public void SplitAndTrim_NoDelimiter_ReturnsSingleElement()
        => "hello".SplitAndTrim(',').Should().Equal("hello");

    [Fact]
    public void SplitAndTrim_EmptySegmentsRemoved()
        => "a,,b".SplitAndTrim(',').Should().Equal("a", "b");
}

public sealed class StringExtensions_ContainsAnyTests
{
    [Fact]
    public void ContainsAny_MatchFound_ReturnsTrue()
        => "hello world".ContainsAny("foo", "world", "bar").Should().BeTrue();

    [Fact]
    public void ContainsAny_NoMatch_ReturnsFalse()
        => "hello world".ContainsAny("foo", "baz").Should().BeFalse();

    [Fact]
    public void ContainsAny_NullSource_ReturnsFalse()
        => ((string?)null).ContainsAny("foo").Should().BeFalse();

    [Fact]
    public void ContainsAny_EmptyValues_ReturnsFalse()
        => "hello".ContainsAny().Should().BeFalse();
}

public sealed class StringExtensions_IfNullOrEmptyTests
{
    [Fact]
    public void IfNullOrEmpty_NullInput_ReturnsFallback()
        => ((string?)null).IfNullOrEmpty("fallback").Should().Be("fallback");

    [Fact]
    public void IfNullOrEmpty_EmptyInput_ReturnsFallback()
        => "".IfNullOrEmpty("fallback").Should().Be("fallback");

    [Fact]
    public void IfNullOrEmpty_ValidInput_ReturnsOriginal()
        => "hello".IfNullOrEmpty("fallback").Should().Be("hello");

    [Fact]
    public void IfNullOrWhiteSpace_WhitespaceInput_ReturnsFallback()
        => "   ".IfNullOrWhiteSpace("fallback").Should().Be("fallback");

    [Fact]
    public void IfNullOrWhiteSpace_ValidInput_ReturnsOriginal()
        => "hello".IfNullOrWhiteSpace("fallback").Should().Be("hello");
}

public sealed class StringExtensions_CombineWithTests
{
    [Fact]
    public void CombineWith_TwoParts_CombinesCorrectly()
    {
        var expected = System.IO.Path.Combine("base", "sub", "file.txt");
        "base".CombineWith("sub", "file.txt").Should().Be(expected);
    }

    [Fact]
    public void CombineWith_NoParts_ReturnsBase()
        => "base".CombineWith().Should().Be("base");
}
