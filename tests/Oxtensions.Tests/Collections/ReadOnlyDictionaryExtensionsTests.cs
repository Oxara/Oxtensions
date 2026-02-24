// -----------------------------------------------------------------------
// <copyright file="ReadOnlyDictionaryExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class ReadOnlyDictionaryExtensionsTests
{
    private static IReadOnlyDictionary<string, int> Sample()
        => new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };

    // ── ContainsAllKeys ───────────────────────────────────────────────────────

    [Fact]
    public void ContainsAllKeys_AllPresent_ReturnsTrue()
        => Sample().ContainsAllKeys(new[] { "a", "b" }).Should().BeTrue();

    [Fact]
    public void ContainsAllKeys_SomeMissing_ReturnsFalse()
        => Sample().ContainsAllKeys(new[] { "a", "z" }).Should().BeFalse();

    [Fact]
    public void ContainsAllKeys_EmptyKeys_ReturnsTrue()
        => Sample().ContainsAllKeys(Array.Empty<string>()).Should().BeTrue();

    [Fact]
    public void ContainsAllKeys_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyDictionary<string, int>)null!).ContainsAllKeys(new[] { "a" });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ContainsAllKeys_NullKeys_ThrowsArgumentNullException()
    {
        var act = () => Sample().ContainsAllKeys(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ContainsAnyKey ────────────────────────────────────────────────────────

    [Fact]
    public void ContainsAnyKey_OnePresent_ReturnsTrue()
        => Sample().ContainsAnyKey(new[] { "a", "z" }).Should().BeTrue();

    [Fact]
    public void ContainsAnyKey_NonePresent_ReturnsFalse()
        => Sample().ContainsAnyKey(new[] { "x", "y", "z" }).Should().BeFalse();

    [Fact]
    public void ContainsAnyKey_EmptyKeys_ReturnsFalse()
        => Sample().ContainsAnyKey(Array.Empty<string>()).Should().BeFalse();

    [Fact]
    public void ContainsAnyKey_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyDictionary<string, int>)null!).ContainsAnyKey(new[] { "a" });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ContainsAnyKey_NullKeys_ThrowsArgumentNullException()
    {
        var act = () => Sample().ContainsAnyKey(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ToDictionary ──────────────────────────────────────────────────────────

    [Fact]
    public void ToDictionary_ReturnsMutableCopyWithSameEntries()
    {
        var result = Sample().ToDictionary();
        result.Should().BeOfType<Dictionary<string, int>>();
        result.Should().BeEquivalentTo(Sample());
    }

    [Fact]
    public void ToDictionary_MutatingCopyDoesNotAffectOriginal()
    {
        IReadOnlyDictionary<string, int> original = new Dictionary<string, int> { ["a"] = 1 };
        var copy = original.ToDictionary();
        copy["a"] = 99;
        original["a"].Should().Be(1);
    }

    [Fact]
    public void ToDictionary_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyDictionary<string, int>)null!).ToDictionary();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── FilterByKeys ──────────────────────────────────────────────────────────

    [Fact]
    public void FilterByKeys_SubsetOfKeys_ReturnsOnlyMatchingEntries()
    {
        var result = Sample().FilterByKeys(new[] { "a", "c" });
        result.Should().HaveCount(2);
        result.Should().ContainKey("a").WhoseValue.Should().Be(1);
        result.Should().ContainKey("c").WhoseValue.Should().Be(3);
        result.Should().NotContainKey("b");
    }

    [Fact]
    public void FilterByKeys_MissingKeys_AreIgnored()
    {
        var result = Sample().FilterByKeys(new[] { "a", "z" });
        result.Should().HaveCount(1);
        result.Should().ContainKey("a");
    }

    [Fact]
    public void FilterByKeys_EmptyKeys_ReturnsEmptyDictionary()
        => Sample().FilterByKeys(Array.Empty<string>()).Should().BeEmpty();

    [Fact]
    public void FilterByKeys_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyDictionary<string, int>)null!).FilterByKeys(new[] { "a" });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FilterByKeys_NullKeys_ThrowsArgumentNullException()
    {
        var act = () => Sample().FilterByKeys(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}