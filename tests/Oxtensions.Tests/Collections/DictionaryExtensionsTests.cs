// -----------------------------------------------------------------------
// <copyright file="DictionaryExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class DictionaryExtensionsTests
{
    // ── IsNullOrEmpty ─────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullDictionary_ReturnsTrue()
        => ((IDictionary<string, int>?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyDictionary_ReturnsTrue()
        => new Dictionary<string, int>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmpty_ReturnsFalse()
        => new Dictionary<string, int> { ["a"] = 1 }.IsNullOrEmpty().Should().BeFalse();

    // ── GetOrDefault ──────────────────────────────────────────────────────────

    [Fact]
    public void GetOrDefault_ExistingKey_ReturnsValue()
    {
        var dict = new Dictionary<string, int> { ["x"] = 42 };
        dict.GetOrDefault("x").Should().Be(42);
    }

    [Fact]
    public void GetOrDefault_MissingKey_ReturnsDefault()
    {
        var dict = new Dictionary<string, int>();
        dict.GetOrDefault("missing", -1).Should().Be(-1);
    }

    [Fact]
    public void GetOrDefault_NullDictionary_ThrowsArgumentNullException()
    {
        var act = () => ((IDictionary<string, int>)null!).GetOrDefault("k");
        act.Should().Throw<ArgumentNullException>();
    }

    // ── GetOrAdd ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetOrAdd_ExistingKey_ReturnsExistingValue()
    {
        var dict = new Dictionary<string, List<int>> { ["k"] = [1, 2] };
        var result = dict.GetOrAdd("k", _ => []);
        result.Should().Equal(1, 2);
    }

    [Fact]
    public void GetOrAdd_MissingKey_AddsAndReturnsNewValue()
    {
        var dict = new Dictionary<string, int>();
        dict.GetOrAdd("n", k => k.Length);
        dict.Should().ContainKey("n");
    }

    // ── Merge ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Merge_NoOverwrite_DoesNotReplaceExisting()
    {
        var source = new Dictionary<string, int> { ["a"] = 1 };
        source.Merge(new Dictionary<string, int> { ["a"] = 99, ["b"] = 2 });
        source["a"].Should().Be(1);
        source["b"].Should().Be(2);
    }

    [Fact]
    public void Merge_WithOverwrite_ReplacesExisting()
    {
        var source = new Dictionary<string, int> { ["a"] = 1 };
        source.Merge(new Dictionary<string, int> { ["a"] = 99 }, overwrite: true);
        source["a"].Should().Be(99);
    }

    [Fact]
    public void Merge_NullOther_ThrowsArgumentNullException()
    {
        var act = () => new Dictionary<string, int>().Merge(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ToQueryString ─────────────────────────────────────────────────────────

    [Fact]
    public void ToQueryString_SimpleDict_ReturnsQueryString()
    {
        var dict = new Dictionary<string, string> { ["q"] = "hello", ["page"] = "1" };
        var result = dict.ToQueryString();
        result.Should().Contain("q=hello");
        result.Should().Contain("page=1");
    }

    [Fact]
    public void ToQueryString_EmptyDict_ReturnsEmpty()
        => new Dictionary<string, string>().ToQueryString().Should().Be(string.Empty);

    [Fact]
    public void ToQueryString_SpecialChars_AreEncoded()
    {
        var dict = new Dictionary<string, string> { ["k"] = "hello world" };
        dict.ToQueryString().Should().Contain("hello%20world");
    }

    // ── Invert ────────────────────────────────────────────────────────────────

    [Fact]
    public void Invert_UniqueValues_SwapsKeysAndValues()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var result = dict.Invert();
        result[1].Should().Be("a");
        result[2].Should().Be("b");
    }

    [Fact]
    public void Invert_DuplicateValues_ThrowsArgumentException()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 1 };
        var act = () => dict.Invert();
        act.Should().Throw<ArgumentException>();
    }
}

public sealed class DictionaryExtensions_AddOrUpdateTests
{
    [Fact]
    public void AddOrUpdate_NewKey_AddsEntry()
    {
        var dict = new Dictionary<string, int>();
        dict.AddOrUpdate("a", 1);
        dict["a"].Should().Be(1);
    }

    [Fact]
    public void AddOrUpdate_ExistingKey_UpdatesEntry()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        dict.AddOrUpdate("a", 99);
        dict["a"].Should().Be(99);
    }
}

public sealed class DictionaryExtensions_RemoveWhereTests
{
    [Fact]
    public void RemoveWhere_MatchingEntries_Removed()
    {
        var dict = new Dictionary<string, int> { ["keep"] = 5, ["remove"] = -1, ["also"] = -3 };
        var count = dict.RemoveWhere((_, v) => v < 0);
        count.Should().Be(2);
        dict.Should().ContainKey("keep");
        dict.Should().NotContainKey("remove");
    }

    [Fact]
    public void RemoveWhere_NoMatches_ReturnsZero()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        dict.RemoveWhere((_, v) => v < 0).Should().Be(0);
        dict.Should().HaveCount(1);
    }
}

public sealed class DictionaryExtensions_ToJsonTests
{
    [Fact]
    public void ToJson_ReturnsValidJson()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var json = dict.ToJson();
        json.Should().Contain("\"a\":");
        json.Should().Contain("\"b\":");
    }

    [Fact]
    public void ToJson_EmptyDictionary_ReturnsEmptyJsonObject()
        => new Dictionary<string, int>().ToJson().Should().Be("{}");
}
