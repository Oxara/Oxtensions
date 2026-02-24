// -----------------------------------------------------------------------
// <copyright file="ListExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class ListExtensionsTests
{
    // ── IsNullOrEmpty ─────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullCollection_ReturnsTrue()
        => ((List<int>?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyList_ReturnsTrue()
        => new List<int>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptyList_ReturnsFalse()
        => new List<int> { 1 }.IsNullOrEmpty().Should().BeFalse();

    // ── Chunk ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Chunk_EvenSplit_ReturnsEqualChunks()
    {
        var result = new List<int> { 1, 2, 3, 4 }.Chunk(2).ToList();
        result.Should().HaveCount(2);
        result[0].Should().Equal(1, 2);
        result[1].Should().Equal(3, 4);
    }

    [Fact]
    public void Chunk_UnevenSplit_LastChunkSmallerSize()
    {
        var result = new List<int> { 1, 2, 3, 4, 5 }.Chunk(2).ToList();
        result.Should().HaveCount(3);
        result[2].Should().HaveCount(1);
    }

    [Fact]
    public void Chunk_ZeroSize_ThrowsArgumentException()
    {
        var act = () => new List<int> { 1 }.Chunk(0).ToList();
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(3, 2)]
    [InlineData(5, 1)]
    public void Chunk_VariousSizes_AllElementsPresent(int chunkSize, int expectedChunks)
    {
        var source = new List<int> { 1, 2, 3, 4, 5 };
        var result = source.Chunk(chunkSize).ToList();
        result.Should().HaveCount(expectedChunks);
        result.SelectMany(x => x).Should().Equal(source);
    }

    // ── Shuffle ───────────────────────────────────────────────────────────────

    [Fact]
    public void Shuffle_List_ContainsSameElements()
    {
        var original = Enumerable.Range(1, 100).ToList();
        var copy = original.ToList();
        copy.Shuffle();
        copy.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Shuffle_NullList_ThrowsArgumentNullException()
    {
        var act = () => ((IList<int>)null!).Shuffle();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── DistinctBy ────────────────────────────────────────────────────────────

    [Fact]
    public void DistinctBy_DuplicateKeys_ReturnsFirstOccurrence()
    {
        var items = new[] { (Id: 1, Name: "a"), (Id: 1, Name: "b"), (Id: 2, Name: "c") };
        var result = ListExtensions.DistinctBy(items, x => x.Id).ToList();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("a");
    }

    [Fact]
    public void DistinctBy_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.DistinctBy((IEnumerable<int>)null!, x => x).ToList();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void DistinctBy_NullKeySelector_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.DistinctBy(new[] { 1, 2 }, (Func<int, int>)null!).ToList();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ForEach ───────────────────────────────────────────────────────────────

    [Fact]
    public void ForEach_ValidList_ExecutesActionForEachElement()
    {
        var result = new List<int>();
        new[] { 1, 2, 3 }.ForEach(result.Add);
        result.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void ForEach_EmptyList_DoesNotExecuteAction()
    {
        int count = 0;
        Array.Empty<int>().ForEach(_ => count++);
        count.Should().Be(0);
    }

    [Fact]
    public void ForEach_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<int>)null!).ForEach(_ => { });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ForEach_NullAction_ThrowsArgumentNullException()
    {
        var act = () => new[] { 1 }.ForEach(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ToHashSet ─────────────────────────────────────────────────────────────

    [Fact]
    public void ToHashSet_WithDuplicates_ReturnsUniqueElements()
        => ListExtensions.ToHashSet(new[] { 1, 1, 2, 3, 3 }).Should().HaveCount(3);

    [Fact]
    public void ToHashSet_Empty_ReturnsEmptySet()
        => ListExtensions.ToHashSet(Array.Empty<int>()).Should().BeEmpty();

    [Fact]
    public void ToHashSet_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.ToHashSet((IEnumerable<int>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── Paginate ──────────────────────────────────────────────────────────────

    [Fact]
    public void Paginate_FirstPage_ReturnsFirstItems()
    {
        var result = Enumerable.Range(1, 20).Paginate(0, 5).ToList();
        result.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void Paginate_SecondPage_ReturnsCorrectItems()
    {
        var result = Enumerable.Range(1, 20).Paginate(1, 5).ToList();
        result.Should().Equal(6, 7, 8, 9, 10);
    }

    [Fact]
    public void Paginate_PageBeyondEnd_ReturnsEmpty()
        => Enumerable.Range(1, 5).Paginate(10, 5).Should().BeEmpty();

    [Theory]
    [InlineData(0, 3, new[] { 1, 2, 3 })]
    [InlineData(1, 3, new[] { 4, 5, 6 })]
    public void Paginate_VariousPages_ReturnsExpected(int page, int size, int[] expected)
        => Enumerable.Range(1, 9).Paginate(page, size).Should().Equal(expected);

    // ── AddRangeIfNotExists ────────────────────────────────────────────────────

    [Fact]
    public void AddRangeIfNotExists_NewItems_AddsAll()
    {
        var list = new List<int> { 1, 2 };
        list.AddRangeIfNotExists(new[] { 3, 4 });
        list.Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void AddRangeIfNotExists_DuplicateItems_SkipsDuplicates()
    {
        var list = new List<int> { 1, 2, 3 };
        list.AddRangeIfNotExists(new[] { 2, 3, 4 });
        list.Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void AddRangeIfNotExists_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((List<int>)null!).AddRangeIfNotExists(new[] { 1 });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddRangeIfNotExists_NullItems_ThrowsArgumentNullException()
    {
        var act = () => new List<int>().AddRangeIfNotExists(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── RemoveWhere ────────────────────────────────────────────────────────────

    [Fact]
    public void RemoveWhere_MatchingElements_RemovesAll()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        int count = list.RemoveWhere(x => x % 2 == 0);
        count.Should().Be(2);
        list.Should().Equal(1, 3, 5);
    }

    [Fact]
    public void RemoveWhere_NoMatch_ReturnsZero()
    {
        var list = new List<int> { 1, 3, 5 };
        list.RemoveWhere(x => x % 2 == 0).Should().Be(0);
        list.Should().HaveCount(3);
    }

    [Fact]
    public void RemoveWhere_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((List<int>)null!).RemoveWhere(x => x > 0);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveWhere_NullPredicate_ThrowsArgumentNullException()
    {
        var act = () => new List<int> { 1 }.RemoveWhere(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── MinBy / MaxBy ──────────────────────────────────────────────────────────

    [Fact]
    public void MinBy_ValidList_ReturnsMinElement()
    {
        var items = new[] { (3, "c"), (1, "a"), (2, "b") };
        ListExtensions.MinBy(items, x => x.Item1).Should().Be((1, "a"));
    }

    [Fact]
    public void MaxBy_ValidList_ReturnsMaxElement()
    {
        var items = new[] { (3, "c"), (1, "a"), (2, "b") };
        ListExtensions.MaxBy(items, x => x.Item1).Should().Be((3, "c"));
    }

    [Fact]
    public void MinBy_EmptyList_ReturnsDefault()
        => ListExtensions.MinBy(Array.Empty<int>(), x => x).Should().Be(default);

    [Fact]
    public void MinBy_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.MinBy((IEnumerable<int>)null!, x => x);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MinBy_NullKeySelector_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.MinBy(new[] { 1 }, (Func<int, int>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MaxBy_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.MaxBy((IEnumerable<int>)null!, x => x);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MaxBy_NullKeySelector_ThrowsArgumentNullException()
    {
        var act = () => ListExtensions.MaxBy(new[] { 1 }, (Func<int, int>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MaxBy_EmptyList_ReturnsDefault()
        => ListExtensions.MaxBy(Array.Empty<int>(), x => x).Should().Be(default);
}

public sealed class ListExtensions_FlattenTests
{
    [Fact]
    public void Flatten_NestedArrays_ReturnsAllElements()
        => new[] { new[] { 1, 2 }, new[] { 3, 4 } }.Flatten().Should().Equal(1, 2, 3, 4);

    [Fact]
    public void Flatten_EmptyOuter_ReturnsEmpty()
        => Array.Empty<int[]>().Flatten().Should().BeEmpty();

    [Fact]
    public void Flatten_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<IEnumerable<int>>)null!).Flatten().ToList();
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_RandomItemTests
{
    [Fact]
    public void RandomItem_NonEmptyList_ReturnsMember()
    {
        var list = new[] { 1, 2, 3, 4, 5 };
        list.RandomItem().Should().BeOneOf(list);
    }

    [Fact]
    public void RandomItem_EmptyList_ThrowsArgumentException()
    {
        var act = () => Array.Empty<int>().RandomItem();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RandomItem_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IList<int>)null!).RandomItem();
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_RandomItemsTests
{
    [Fact]
    public void RandomItems_CountSubset_ReturnsCorrectCount()
    {
        var list = new[] { 1, 2, 3, 4, 5 };
        var result = list.RandomItems(3);
        result.Should().HaveCount(3);
        result.Should().OnlyContain(x => list.Contains(x));
    }

    [Fact]
    public void RandomItems_CountEqualsLength_ReturnsAllDistinct()
    {
        var list = new[] { 1, 2, 3 };
        list.RandomItems(3).Distinct().Should().HaveCount(3);
    }

    [Fact]
    public void RandomItems_CountZero_ReturnsEmpty()
        => new[] { 1, 2, 3 }.RandomItems(0).Should().BeEmpty();

    [Fact]
    public void RandomItems_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IList<int>)null!).RandomItems(1).ToList();
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_RotateTests
{
    [Fact]
    public void Rotate_ByTwo_ShiftsLeft()
    {
        var list = new System.Collections.Generic.List<int> { 1, 2, 3, 4, 5 };
        list.Rotate(2);
        list.Should().Equal(3, 4, 5, 1, 2);
    }

    [Fact]
    public void Rotate_ByNegativeOne_ShiftsRight()
    {
        var list = new System.Collections.Generic.List<int> { 1, 2, 3, 4, 5 };
        list.Rotate(-1);
        list.Should().Equal(5, 1, 2, 3, 4);
    }

    [Fact]
    public void Rotate_ByLength_SameOrder()
    {
        var list = new System.Collections.Generic.List<int> { 1, 2, 3 };
        list.Rotate(3);
        list.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Rotate_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IList<int>)null!).Rotate(1);
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_InterleaveTests
{
    [Fact]
    public void Interleave_EqualLength_Alternates()
        => new[] { 1, 2, 3 }.Interleave(new[] { 10, 20, 30 }).Should().Equal(1, 10, 2, 20, 3, 30);

    [Fact]
    public void Interleave_UnequalLength_AppendsRemainder()
        => new[] { 1, 2 }.Interleave(new[] { 10, 20, 30 }).Should().Equal(1, 10, 2, 20, 30);

    [Fact]
    public void Interleave_NullFirst_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<int>)null!).Interleave(new[] { 1 }).ToList();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Interleave_NullSecond_ThrowsArgumentNullException()
    {
        var act = () => new[] { 1 }.Interleave(null!).ToList();
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_CountByTests
{
    [Fact]
    public void CountBy_Words_CountsByLength()
    {
        var words = new[] { "a", "bb", "cc", "d" };
        var result = words.CountBy(w => w.Length);
        result[1].Should().Be(2);
        result[2].Should().Be(2);
    }

    [Fact]
    public void CountBy_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<string>)null!).CountBy(s => s.Length);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CountBy_NullKeySelector_ThrowsArgumentNullException()
    {
        var act = () => new[] { "a" }.CountBy((Func<string, int>)null!);
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_NoneTests
{
    [Fact]
    public void None_WithPredicate_AllPositive_ReturnsTrue()
        => new[] { 1, 2, 3 }.None(x => x < 0).Should().BeTrue();

    [Fact]
    public void None_WithPredicate_SomeNegative_ReturnsFalse()
        => new[] { 1, -2, 3 }.None(x => x < 0).Should().BeFalse();

    [Fact]
    public void None_NoPredicate_EmptySequence_ReturnsTrue()
        => Array.Empty<int>().None().Should().BeTrue();

    [Fact]
    public void None_NoPredicate_NonEmpty_ReturnsFalse()
        => new[] { 1 }.None().Should().BeFalse();

    [Fact]
    public void None_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<int>)null!).None();
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_HasDuplicatesTests
{
    [Fact]
    public void HasDuplicates_WithDuplicates_ReturnsTrue()
        => new[] { 1, 2, 2, 3 }.HasDuplicates().Should().BeTrue();

    [Fact]
    public void HasDuplicates_AllUnique_ReturnsFalse()
        => new[] { 1, 2, 3 }.HasDuplicates().Should().BeFalse();

    [Fact]
    public void HasDuplicates_Empty_ReturnsFalse()
        => Array.Empty<int>().HasDuplicates().Should().BeFalse();

    [Fact]
    public void HasDuplicates_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<int>)null!).HasDuplicates();
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class ListExtensions_DuplicatesTests
{
    [Fact]
    public void Duplicates_ReturnsDuplicateElements()
    {
        var items = new[] { 1, 2, 2, 3, 3, 3 };
        items.Duplicates(x => x).Should().Equal(2, 3);
    }

    [Fact]
    public void Duplicates_NoDuplicates_ReturnsEmpty()
        => new[] { 1, 2, 3 }.Duplicates(x => x).Should().BeEmpty();

    [Fact]
    public void Duplicates_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IEnumerable<int>)null!).Duplicates(x => x).ToList();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Duplicates_NullKeySelector_ThrowsArgumentNullException()
    {
        var act = () => new[] { 1, 1 }.Duplicates((Func<int, int>)null!).ToList();
        act.Should().Throw<ArgumentNullException>();
    }
}
