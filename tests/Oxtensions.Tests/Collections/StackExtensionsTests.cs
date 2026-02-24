// -----------------------------------------------------------------------
// <copyright file="StackExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class StackExtensionsTests
{
    // ── IsNullOrEmpty ─────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullStack_ReturnsTrue()
        => ((Stack<int>?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyStack_ReturnsTrue()
        => new Stack<int>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptyStack_ReturnsFalse()
        => new Stack<int>(new[] { 1 }).IsNullOrEmpty().Should().BeFalse();

    // ── PeekOrDefault ─────────────────────────────────────────────────────────

    [Fact]
    public void PeekOrDefault_EmptyStack_ReturnsSuppliedDefault()
        => new Stack<int>().PeekOrDefault(-1).Should().Be(-1);

    [Fact]
    public void PeekOrDefault_NonEmptyStack_ReturnsTopWithoutRemoving()
    {
        var stack = new Stack<int>(new[] { 1, 2, 3 }); // push order 1→2→3, top = 3
        stack.PeekOrDefault().Should().Be(3);
        stack.Count.Should().Be(3);
    }

    [Fact]
    public void PeekOrDefault_NullStack_ThrowsArgumentNullException()
    {
        Stack<int>? stack = null;
        var act = () => stack!.PeekOrDefault();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── PopOrDefault ──────────────────────────────────────────────────────────

    [Fact]
    public void PopOrDefault_EmptyStack_ReturnsSuppliedDefault()
        => new Stack<string>().PopOrDefault("fallback").Should().Be("fallback");

    [Fact]
    public void PopOrDefault_NonEmptyStack_RemovesAndReturnsTop()
    {
        var stack = new Stack<int>(new[] { 1, 2, 3 }); // push order 1→2→3, top = 3
        stack.PopOrDefault().Should().Be(3);
        stack.Count.Should().Be(2);
    }

    [Fact]
    public void PopOrDefault_NullStack_ThrowsArgumentNullException()
    {
        Stack<int>? stack = null;
        var act = () => stack!.PopOrDefault();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── PushRange ─────────────────────────────────────────────────────────────

    [Fact]
    public void PushRange_ValidItems_PushesAllInOrder()
    {
        var stack = new Stack<int>();
        stack.PushRange(new[] { 1, 2, 3 });
        stack.Count.Should().Be(3);
        stack.Pop().Should().Be(3); // last pushed = top
    }

    [Fact]
    public void PushRange_EmptySource_LeavesStackUnchanged()
    {
        var stack = new Stack<int>(new[] { 99 });
        stack.PushRange(Array.Empty<int>());
        stack.Count.Should().Be(1);
    }

    [Fact]
    public void PushRange_NullStack_ThrowsArgumentNullException()
    {
        Stack<int>? stack = null;
        var act = () => stack!.PushRange(new[] { 1 });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PushRange_NullItems_ThrowsArgumentNullException()
    {
        var stack = new Stack<int>();
        var act = () => stack.PushRange(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── PopMany ───────────────────────────────────────────────────────────────

    [Fact]
    public void PopMany_CountWithinSize_ReturnsPoppedElementsInOrder()
    {
        var stack = new Stack<int>(new[] { 3, 2, 1 }); // push order 3→2→1, top = 1
        stack.PopMany(2).Should().Equal(1, 2);
        stack.Count.Should().Be(1);
    }

    [Fact]
    public void PopMany_CountExceedsSize_ReturnsAllElements()
    {
        var stack = new Stack<int>(new[] { 2, 1 }); // top = 1
        stack.PopMany(10).Should().Equal(1, 2);
        stack.Count.Should().Be(0);
    }

    [Fact]
    public void PopMany_ZeroCount_ReturnsEmptyArray()
    {
        var stack = new Stack<int>(new[] { 1, 2 });
        stack.PopMany(0).Should().BeEmpty();
        stack.Count.Should().Be(2);
    }

    [Fact]
    public void PopMany_NegativeCount_ThrowsArgumentException()
    {
        var stack = new Stack<int>(new[] { 1 });
        var act = () => stack.PopMany(-1);
        act.Should().Throw<ArgumentException>().WithParameterName("count");
    }

    [Fact]
    public void PopMany_NullStack_ThrowsArgumentNullException()
    {
        Stack<int>? stack = null;
        var act = () => stack!.PopMany(1);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── Clone ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Clone_NonEmptyStack_ReturnsCopyWithSameTopElement()
    {
        var original = new Stack<int>(new[] { 3, 2, 1 }); // top = 1
        var copy = original.Clone();
        copy.Should().NotBeSameAs(original);
        copy.Count.Should().Be(3);
        copy.Pop().Should().Be(original.Pop()); // both tops identical
    }

    [Fact]
    public void Clone_ModifyingCopy_DoesNotAffectOriginal()
    {
        var original = new Stack<int>(new[] { 1, 2, 3 });
        var copy = original.Clone();
        copy.Push(99);
        original.Count.Should().Be(3);
        copy.Count.Should().Be(4);
    }

    [Fact]
    public void Clone_EmptyStack_ReturnsEmptyStack()
        => new Stack<int>().Clone().Count.Should().Be(0);

    [Fact]
    public void Clone_NullStack_ThrowsArgumentNullException()
    {
        Stack<int>? stack = null;
        var act = () => stack!.Clone();
        act.Should().Throw<ArgumentNullException>();
    }
}
