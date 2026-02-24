// -----------------------------------------------------------------------
// <copyright file="QueueExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Collections;
using Xunit;

namespace Oxtensions.Tests.Collections;

public sealed class QueueExtensionsTests
{
    // ── IsNullOrEmpty ─────────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_NullQueue_ReturnsTrue()
        => ((Queue<int>?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyQueue_ReturnsTrue()
        => new Queue<int>().IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_NonEmptyQueue_ReturnsFalse()
        => new Queue<int>(new[] { 1 }).IsNullOrEmpty().Should().BeFalse();

    // ── PeekOrDefault ─────────────────────────────────────────────────────────

    [Fact]
    public void PeekOrDefault_EmptyQueue_ReturnsSuppliedDefault()
        => new Queue<int>().PeekOrDefault(-1).Should().Be(-1);

    [Fact]
    public void PeekOrDefault_NonEmptyQueue_ReturnsFrontWithoutRemoving()
    {
        var queue = new Queue<int>(new[] { 10, 20, 30 });
        queue.PeekOrDefault().Should().Be(10);
        queue.Count.Should().Be(3);
    }

    [Fact]
    public void PeekOrDefault_NullQueue_ThrowsArgumentNullException()
    {
        Queue<int>? queue = null;
        var act = () => queue!.PeekOrDefault();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── DequeueOrDefault ──────────────────────────────────────────────────────

    [Fact]
    public void DequeueOrDefault_EmptyQueue_ReturnsSuppliedDefault()
        => new Queue<string>().DequeueOrDefault("fallback").Should().Be("fallback");

    [Fact]
    public void DequeueOrDefault_NonEmptyQueue_RemovesAndReturnsFront()
    {
        var queue = new Queue<int>(new[] { 10, 20, 30 });
        queue.DequeueOrDefault().Should().Be(10);
        queue.Count.Should().Be(2);
    }

    [Fact]
    public void DequeueOrDefault_NullQueue_ThrowsArgumentNullException()
    {
        Queue<int>? queue = null;
        var act = () => queue!.DequeueOrDefault();
        act.Should().Throw<ArgumentNullException>();
    }

    // ── EnqueueRange ──────────────────────────────────────────────────────────

    [Fact]
    public void EnqueueRange_ValidItems_EnqueuesAllInOrder()
    {
        var queue = new Queue<int>();
        queue.EnqueueRange(new[] { 1, 2, 3 });
        queue.Count.Should().Be(3);
        queue.Dequeue().Should().Be(1); // first enqueued = front
    }

    [Fact]
    public void EnqueueRange_EmptySource_LeavesQueueUnchanged()
    {
        var queue = new Queue<int>(new[] { 99 });
        queue.EnqueueRange(Array.Empty<int>());
        queue.Count.Should().Be(1);
    }

    [Fact]
    public void EnqueueRange_NullQueue_ThrowsArgumentNullException()
    {
        Queue<int>? queue = null;
        var act = () => queue!.EnqueueRange(new[] { 1 });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EnqueueRange_NullItems_ThrowsArgumentNullException()
    {
        var queue = new Queue<int>();
        var act = () => queue.EnqueueRange(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── DequeueMany ───────────────────────────────────────────────────────────

    [Fact]
    public void DequeueMany_CountWithinSize_ReturnsDequeuedElementsInOrder()
    {
        var queue = new Queue<int>(new[] { 1, 2, 3, 4 });
        queue.DequeueMany(2).Should().Equal(1, 2);
        queue.Count.Should().Be(2);
    }

    [Fact]
    public void DequeueMany_CountExceedsSize_ReturnsAllElements()
    {
        var queue = new Queue<int>(new[] { 5, 6 });
        queue.DequeueMany(10).Should().Equal(5, 6);
        queue.Count.Should().Be(0);
    }

    [Fact]
    public void DequeueMany_ZeroCount_ReturnsEmptyArray()
    {
        var queue = new Queue<int>(new[] { 1, 2 });
        queue.DequeueMany(0).Should().BeEmpty();
        queue.Count.Should().Be(2);
    }

    [Fact]
    public void DequeueMany_NegativeCount_ThrowsArgumentException()
    {
        var queue = new Queue<int>(new[] { 1 });
        var act = () => queue.DequeueMany(-1);
        act.Should().Throw<ArgumentException>().WithParameterName("count");
    }

    [Fact]
    public void DequeueMany_NullQueue_ThrowsArgumentNullException()
    {
        Queue<int>? queue = null;
        var act = () => queue!.DequeueMany(1);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── Clone ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Clone_NonEmptyQueue_ReturnsCopyWithSameFrontElement()
    {
        var original = new Queue<int>(new[] { 1, 2, 3 });
        var copy = original.Clone();
        copy.Should().NotBeSameAs(original);
        copy.Count.Should().Be(3);
        copy.Dequeue().Should().Be(original.Dequeue()); // both fronts identical
    }

    [Fact]
    public void Clone_ModifyingCopy_DoesNotAffectOriginal()
    {
        var original = new Queue<int>(new[] { 1, 2, 3 });
        var copy = original.Clone();
        copy.Enqueue(99);
        original.Count.Should().Be(3);
        copy.Count.Should().Be(4);
    }

    [Fact]
    public void Clone_EmptyQueue_ReturnsEmptyQueue()
        => new Queue<int>().Clone().Count.Should().Be(0);

    [Fact]
    public void Clone_NullQueue_ThrowsArgumentNullException()
    {
        Queue<int>? queue = null;
        var act = () => queue!.Clone();
        act.Should().Throw<ArgumentNullException>();
    }
}
