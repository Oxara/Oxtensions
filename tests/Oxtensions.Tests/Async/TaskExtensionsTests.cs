// -----------------------------------------------------------------------
// <copyright file="TaskExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Oxtensions.Async;
using Xunit;

public sealed class TaskExtensions_FireAndForgetTests
{
    [Fact]
    public async Task FireAndForget_CompletedTask_DoesNotThrow()
    {
        Task.CompletedTask.FireAndForget();
        await Task.Delay(10);  // allow fire-and-forget to run
    }

    [Fact]
    public async Task FireAndForget_FaultedTask_InvokesErrorHandler()
    {
        Exception? captured = null;
        Task.FromException(new InvalidOperationException("oops"))
            .FireAndForget(ex => captured = ex);
        await Task.Delay(50);
        captured.Should().BeOfType<InvalidOperationException>();
    }
}

public sealed class TaskExtensions_WithTimeoutTests
{
    [Fact]
    public async Task WithTimeout_CompletesInTime_ReturnsResult()
    {
        var result = await Task.FromResult(42).WithTimeout(System.TimeSpan.FromSeconds(5));
        result.Should().Be(42);
    }

    [Fact]
    public async Task WithTimeout_NonGeneric_CompletesInTime_DoesNotThrow()
    {
        Func<Task> act = () => Task.CompletedTask.WithTimeout(System.TimeSpan.FromSeconds(5));
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task WithTimeout_ExceedsTimeout_ThrowsTimeoutException()
    {
        var slowTask = Task.Delay(System.TimeSpan.FromSeconds(60));
        Func<Task> act = () => slowTask.WithTimeout(System.TimeSpan.FromMilliseconds(50));
        await act.Should().ThrowAsync<TimeoutException>();
    }

    [Fact]
    public async Task WithTimeout_Generic_ExceedsTimeout_ThrowsTimeoutException()
    {
        async Task<int> SlowWork() { await Task.Delay(System.TimeSpan.FromSeconds(60)); return 0; }

        Func<Task> act = () => SlowWork().WithTimeout(System.TimeSpan.FromMilliseconds(50));
        await act.Should().ThrowAsync<TimeoutException>();
    }
}

public sealed class TaskExtensions_WithCancellationTests
{
    [Fact]
    public async Task WithCancellation_CancelledToken_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var longTask = Task.Delay(System.TimeSpan.FromSeconds(60));
        Func<Task> act = () => longTask.WithCancellation(cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task WithCancellation_Generic_CancelledToken_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        async Task<int> LongWork() { await Task.Delay(System.TimeSpan.FromSeconds(60)); return 0; }

        Func<Task> act = () => LongWork().WithCancellation(cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task WithCancellation_NotCancelled_CompletesNormally()
    {
        using var cts = new CancellationTokenSource();
        await Task.CompletedTask.WithCancellation(cts.Token);
    }
}

public sealed class TaskExtensions_RetryTests
{
    [Fact]
    public async Task Retry_AlwaysSucceeds_CompletesOnFirstAttempt()
    {
        int calls = 0;
        Func<Task> factory = () => { calls++; return Task.CompletedTask; };
        await factory.Retry(maxAttempts: 3);
        calls.Should().Be(1);
    }

    [Fact]
    public async Task Retry_FailsThenSucceeds_RetriesUntilSuccess()
    {
        int calls = 0;
        Func<Task> factory = () =>
        {
            calls++;
            if (calls < 3) throw new InvalidOperationException("retry");
            return Task.CompletedTask;
        };
        await factory.Retry(maxAttempts: 5, delay: System.TimeSpan.FromMilliseconds(1));
        calls.Should().Be(3);
    }

    [Fact]
    public async Task Retry_AlwaysFails_ThrowsAfterMaxAttempts()
    {
        Func<Task> factory = () => throw new InvalidOperationException("always fails");
        Func<Task> act = () => factory.Retry(maxAttempts: 2, delay: System.TimeSpan.FromMilliseconds(1));
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RetryT_SucceedsOnFirstAttempt_ReturnsValue()
    {
        Func<Task<int>> factory = () => Task.FromResult(99);
        var result = await factory.Retry(maxAttempts: 3);
        result.Should().Be(99);
    }

    [Fact]
    public async Task RetryT_FailsThenSucceeds_ReturnsValue()
    {
        int calls = 0;
        Func<Task<int>> factory = () =>
        {
            calls++;
            if (calls < 2) throw new Exception("fail");
            return Task.FromResult(42);
        };
        var result = await factory.Retry(maxAttempts: 3, delay: System.TimeSpan.FromMilliseconds(1));
        result.Should().Be(42);
    }

    [Fact]
    public async Task Retry_ZeroAttempts_ThrowsArgumentException()
    {
        Func<Task> factory = () => Task.CompletedTask;
        Func<Task> act = () => factory.Retry(maxAttempts: 0);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task RetryT_ZeroAttempts_ThrowsArgumentException()
    {
        Func<Task<int>> factory = () => Task.FromResult(0);
        Func<Task> act = () => factory.Retry(maxAttempts: 0);
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
