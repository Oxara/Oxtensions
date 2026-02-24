// -----------------------------------------------------------------------
// <copyright file="ReadOnlyListExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class ReadOnlyListExtensionsTests
{
    // â”€â”€ IsNullOrEmpty â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [Fact]
    public void IsNullOrEmpty_NullList_ReturnsTrue()
        => ((IReadOnlyList<int>?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyArray_ReturnsTrue()
        => Array.Empty<int>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptyArray_ReturnsFalse()
        => new int[] { 1 }.IsNullOrEmpty().Should().BeFalse();

    // â”€â”€ IndexOf â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [Fact]
    public void IndexOf_ExistingElement_ReturnsCorrectIndex()
    {
        IReadOnlyList<int> list = new[] { 10, 20, 30 };
        list.IndexOf(20).Should().Be(1);
    }

    [Fact]
    public void IndexOf_MissingElement_ReturnsMinusOne()
    {
        IReadOnlyList<int> list = new[] { 10, 20, 30 };
        list.IndexOf(99).Should().Be(-1);
    }

    [Fact]
    public void IndexOf_DuplicateElements_ReturnsFirstOccurrence()
    {
        IReadOnlyList<int> list = new[] { 5, 5, 5 };
        list.IndexOf(5).Should().Be(0);
    }

    [Fact]
    public void IndexOf_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyList<int>)null!).IndexOf(1);
        act.Should().Throw<ArgumentNullException>();
    }

    // â”€â”€ LastIndexOf â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [Fact]
    public void LastIndexOf_ExistingElement_ReturnsLastIndex()
    {
        IReadOnlyList<int> list = new[] { 10, 20, 10 };
        list.LastIndexOf(10).Should().Be(2);
    }

    [Fact]
    public void LastIndexOf_MissingElement_ReturnsMinusOne()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3 };
        list.LastIndexOf(99).Should().Be(-1);
    }

    [Fact]
    public void LastIndexOf_UniqueElement_SameAsIndexOf()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3 };
        list.LastIndexOf(2).Should().Be(list.IndexOf(2));
    }

    [Fact]
    public void LastIndexOf_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyList<int>)null!).LastIndexOf(1);
        act.Should().Throw<ArgumentNullException>();
    }

    // â”€â”€ BinarySearch â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [Fact]
    public void BinarySearch_ExistingElement_ReturnsCorrectIndex()
    {
        IReadOnlyList<int> list = new[] { 1, 3, 5, 7, 9 };
        list.BinarySearch(5).Should().Be(2);
    }

    [Fact]
    public void BinarySearch_FirstElement_ReturnsZero()
    {
        IReadOnlyList<int> list = new[] { 1, 3, 5 };
        list.BinarySearch(1).Should().Be(0);
    }

    [Fact]
    public void BinarySearch_LastElement_ReturnsLastIndex()
    {
        IReadOnlyList<int> list = new[] { 1, 3, 5 };
        list.BinarySearch(5).Should().Be(2);
    }

    [Fact]
    public void BinarySearch_MissingElement_ReturnsNegativeBitwiseComplement()
    {
        IReadOnlyList<int> list = new[] { 1, 3, 5, 7, 9 };
        int result = list.BinarySearch(4);
        result.Should().BeNegative();
        (~result).Should().Be(2); // would insert at index 2
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(3, 1)]
    [InlineData(5, 2)]
    [InlineData(7, 3)]
    [InlineData(9, 4)]
    public void BinarySearch_AllElements_ReturnsCorrectIndices(int value, int expected)
    {
        IReadOnlyList<int> list = new[] { 1, 3, 5, 7, 9 };
        list.BinarySearch(value).Should().Be(expected);
    }

    [Fact]
    public void BinarySearch_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyList<int>)null!).BinarySearch(1);
        act.Should().Throw<ArgumentNullException>();
    }

    // â”€â”€ Slice â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [Fact]
    public void Slice_MiddleRange_ReturnsCorrectElements()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3, 4, 5 };
        list.Slice(1, 3).Should().Equal(2, 3, 4);
    }

    [Fact]
    public void Slice_FromStart_ReturnsPrefix()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3, 4, 5 };
        list.Slice(0, 2).Should().Equal(1, 2);
    }

    [Fact]
    public void Slice_ZeroLength_ReturnsEmpty()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3 };
        list.Slice(1, 0).Should().BeEmpty();
    }

    [Fact]
    public void Slice_NegativeStart_ThrowsArgumentException()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3 };
        var act = () => list.Slice(-1, 2);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Slice_LengthBeyondBound_ThrowsArgumentException()
    {
        IReadOnlyList<int> list = new[] { 1, 2, 3 };
        var act = () => list.Slice(2, 5);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Slice_NullSource_ThrowsArgumentNullException()
    {
        var act = () => ((IReadOnlyList<int>)null!).Slice(0, 1);
        act.Should().Throw<ArgumentNullException>();
    }
}
