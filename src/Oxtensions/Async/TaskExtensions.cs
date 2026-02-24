// -----------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;

namespace Oxtensions.Async;

/// <summary>
/// Extension methods for <see cref="Task"/> and <see cref="Task{TResult}"/> providing
/// timeout, cancellation, retry, and fire-and-forget helpers.
/// </summary>
public static class TaskExtensions
{
    // ─────────────────────────────────────────────────────────────────────────
    // Fire and forget
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Starts the task without awaiting it. Exceptions are silently swallowed unless
    /// an <paramref name="onError"/> handler is provided.
    /// </summary>
    /// <param name="task">The task to run in the background.</param>
    /// <param name="onError">Optional callback invoked when the task faults.</param>
    /// <example>
    /// <code>
    /// SendEmailAsync(user).FireAndForget(ex => logger.LogError(ex, "Email failed"));
    /// </code>
    /// </example>
    public static void FireAndForget(this Task task, Action<Exception>? onError = null)
    {
        _ = task.ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception is not null)
                onError?.Invoke(t.Exception.GetBaseException());
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Timeout
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a task that completes with the original result, or throws
    /// <see cref="TimeoutException"/> when the timeout elapses first.
    /// </summary>
    /// <param name="task">The task to apply a timeout to.</param>
    /// <param name="timeout">Maximum allowed duration.</param>
    /// <returns>A task that faults with <see cref="TimeoutException"/> on timeout.</returns>
    /// <example>
    /// <code>
    /// await FetchDataAsync().WithTimeout(5.Seconds());
    /// </code>
    /// </example>
    public static async Task WithTimeout(this Task task, System.TimeSpan timeout)
    {
        using var cts   = new CancellationTokenSource();
        var delay       = Task.Delay(timeout, cts.Token);
        var completed   = await Task.WhenAny(task, delay).ConfigureAwait(false);

        if (completed == delay)
            throw new TimeoutException($"The operation timed out after {timeout}.");

        cts.Cancel();
        await task.ConfigureAwait(false); // propagate original exceptions
    }

    /// <summary>
    /// Returns a task that completes with the original result, or throws
    /// <see cref="TimeoutException"/> when the timeout elapses first.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="task">The task to apply a timeout to.</param>
    /// <param name="timeout">Maximum allowed duration.</param>
    /// <returns>The task result, or <see cref="TimeoutException"/> on timeout.</returns>
    public static async Task<T> WithTimeout<T>(this Task<T> task, System.TimeSpan timeout)
    {
        using var cts   = new CancellationTokenSource();
        var delay       = Task.Delay(timeout, cts.Token);
        var completed   = await Task.WhenAny(task, delay).ConfigureAwait(false);

        if (completed == delay)
            throw new TimeoutException($"The operation timed out after {timeout}.");

        cts.Cancel();
        return await task.ConfigureAwait(false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Cancellation
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a task that completes with the original result, or throws
    /// <see cref="OperationCanceledException"/> when <paramref name="ct"/> is cancelled.
    /// </summary>
    /// <param name="task">The task to make cancellable.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A cancellable task.</returns>
    public static async Task WithCancellation(this Task task, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var reg = ct.Register(() => tcs.TrySetCanceled());
        var completed = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
        await completed.ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a task that completes with the original result, or throws
    /// <see cref="OperationCanceledException"/> when <paramref name="ct"/> is cancelled.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="task">The task to make cancellable.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A cancellable task with the result.</returns>
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var reg = ct.Register(() => tcs.TrySetCanceled());
        var completed = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
        return await ((Task<T>)completed).ConfigureAwait(false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Retry
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Retries a task-returning factory up to <paramref name="maxAttempts"/> times,
    /// waiting <paramref name="delay"/> between attempts. Re-throws the last exception on failure.
    /// </summary>
    /// <param name="taskFactory">A function that returns a new <see cref="Task"/> on each call.</param>
    /// <param name="maxAttempts">Total number of attempts (minimum 1).</param>
    /// <param name="delay">Optional delay between attempts. Defaults to no delay.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A completed task, or the last exception re-thrown.</returns>
    /// <example>
    /// <code>
    /// Func&lt;Task&gt; fetch = () => FetchAsync();
    /// await fetch.Retry(maxAttempts: 3, delay: 2.Seconds());
    /// </code>
    /// </example>
    public static async Task Retry(
        this Func<Task> taskFactory,
        int maxAttempts,
        System.TimeSpan? delay = null,
        CancellationToken ct = default)
    {
        if (maxAttempts < 1) throw new ArgumentException("maxAttempts must be at least 1.", nameof(maxAttempts));

        Exception? last = null;
        for (int i = 0; i < maxAttempts; i++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                await taskFactory().ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                last = ex;
                if (i < maxAttempts - 1 && delay.HasValue)
                    await Task.Delay(delay.Value, ct).ConfigureAwait(false);
            }
        }
        throw last!;
    }

    /// <summary>
    /// Retries a task-returning factory up to <paramref name="maxAttempts"/> times and returns the result.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="taskFactory">A function that returns a new <see cref="Task{T}"/> on each call.</param>
    /// <param name="maxAttempts">Total number of attempts (minimum 1).</param>
    /// <param name="delay">Optional delay between attempts.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>The result from the first successful attempt.</returns>
    public static async Task<T> Retry<T>(
        this Func<Task<T>> taskFactory,
        int maxAttempts,
        System.TimeSpan? delay = null,
        CancellationToken ct = default)
    {
        if (maxAttempts < 1) throw new ArgumentException("maxAttempts must be at least 1.", nameof(maxAttempts));

        Exception? last = null;
        for (int i = 0; i < maxAttempts; i++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                return await taskFactory().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                last = ex;
                if (i < maxAttempts - 1 && delay.HasValue)
                    await Task.Delay(delay.Value, ct).ConfigureAwait(false);
            }
        }
        throw last!;
    }
}
