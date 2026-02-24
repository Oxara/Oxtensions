// -----------------------------------------------------------------------
// <copyright file="SetExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class SetExtensionsTests
{
    // ── IsNullOrEmpty ─────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullSet_ReturnsTrue()
        => ((ISet<int>?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptySet_ReturnsTrue()
        => new HashSet<int>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptySet_ReturnsFalse()
        => new HashSet<int> { 1 }.IsNullOrEmpty().Should().BeFalse();

    // ── AddRange ──────────────────────────────────────────────────────────────

    [Fact]
    public void AddRange_AllNewItems_ReturnsCountOfAll()
    {
        var set = new HashSet<int>();
        var added = set.AddRange(new[] { 1, 2, 3 });
        added.Should().Be(3);
        set.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void AddRange_WithDuplicates_OnlyNewItemsCounted()
    {
        var set = new HashSet<int> { 1, 2 };
        var added = set.AddRange(new[] { 2, 3, 4 });
        added.Should().Be(2);
        set.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void AddRange_AllDuplicates_ReturnsZero()
    {
        var set = new HashSet<int> { 1, 2, 3 };
        var added = set.AddRange(new[] { 1, 2, 3 });
        added.Should().Be(0);
        set.Should().HaveCount(3);
    }

    [Fact]
    public void AddRange_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((ISet<int>)null!).AddRange(new[] { 1 });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddRange_NullItems_ThrowsArgumentNullException()
    {
        var act = () => new HashSet<int>().AddRange(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── RemoveWhere ───────────────────────────────────────────────────────────

    [Fact]
    public void RemoveWhere_MatchingElements_RemovesAndReturnsCount()
    {
        var set = new HashSet<int> { 1, 2, 3, 4 };
        var removed = set.RemoveWhere(x => x % 2 == 0);
        removed.Should().Be(2);
        set.Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Fact]
    public void RemoveWhere_NoMatch_ReturnsZero()
    {
        var set = new HashSet<int> { 1, 3, 5 };
        var removed = set.RemoveWhere(x => x % 2 == 0);
        removed.Should().Be(0);
        set.Should().HaveCount(3);
    }

    [Fact]
    public void RemoveWhere_AllMatch_EmptiesSet()
    {
        var set = new HashSet<int> { 2, 4, 6 };
        var removed = set.RemoveWhere(x => x % 2 == 0);
        removed.Should().Be(3);
        set.Should().BeEmpty();
    }

    [Fact]
    public void RemoveWhere_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((ISet<int>)null!).RemoveWhere(x => x > 0);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveWhere_NullPredicate_ThrowsArgumentNullException()
    {
        var act = () => new HashSet<int> { 1 }.RemoveWhere(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ToSortedSet ───────────────────────────────────────────────────────────

    [Fact]
    public void ToSortedSet_UnorderedHashSet_ReturnsElementsInOrder()
    {
        var set = new HashSet<int> { 5, 1, 3, 2, 4 };
        var sorted = set.ToSortedSet();
        sorted.Should().BeInAscendingOrder();
        sorted.Should().BeEquivalentTo(set);
    }

    [Fact]
    public void ToSortedSet_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((ISet<int>)null!).ToSortedSet();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── OverlapsWith ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(new[] { 3, 4, 5 }, true)]
    [InlineData(new[] { 6, 7, 8 }, false)]
    public void OverlapsWith_VariousSequences_ReturnsExpected(int[] other, bool expected)
    {
        var set = new HashSet<int> { 1, 2, 3 };
        set.OverlapsWith(other).Should().Be(expected);
    }

    [Fact]
    public void OverlapsWith_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((ISet<int>)null!).OverlapsWith(new[] { 1 });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void OverlapsWith_NullOther_ThrowsArgumentNullException()
    {
        var act = () => new HashSet<int> { 1 }.OverlapsWith(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── SymmetricDifference ───────────────────────────────────────────────────

    [Fact]
    public void SymmetricDifference_PartialOverlap_ReturnsExclusiveElements()
    {
        var set = new HashSet<int> { 1, 2, 3 };
        var result = set.SymmetricDifference(new[] { 2, 3, 4 });
        result.Should().BeEquivalentTo(new[] { 1, 4 });
    }

    [Fact]
    public void SymmetricDifference_NoOverlap_ReturnsBothSets()
    {
        var set = new HashSet<int> { 1, 2 };
        var result = set.SymmetricDifference(new[] { 3, 4 });
        result.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void SymmetricDifference_DoesNotMutateSource()
    {
        var set = new HashSet<int> { 1, 2, 3 };
        _ = set.SymmetricDifference(new[] { 2, 3, 4 });
        set.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void SymmetricDifference_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((ISet<int>)null!).SymmetricDifference(new[] { 1 });
        act.Should().Throw<ArgumentNullException>();
    }
}
