# Async Extensions

> Namespace: `Oxtensions.Async`

Production-grade `Task` and `Task<T>` helpers — fire-and-forget, timeout enforcement, cancellation wrapping, and retry policies — all built on `System.Threading.Tasks` with no third-party dependencies.

---

## Methods

| Method | Description |
|--------|-------------|
| `FireAndForget(Action<Exception>?)` | Starts the task without awaiting; invokes the optional error handler on failure |
| `WithTimeout(TimeSpan)` | Wraps a `Task` — throws `TimeoutException` if the task does not complete in time |
| `WithTimeout<T>(TimeSpan)` | Same as above for `Task<T>` |
| `WithCancellation(CancellationToken)` | Throws `OperationCanceledException` when the token is cancelled |
| `WithCancellation<T>(CancellationToken)` | Same as above for `Task<T>` |
| `Retry(int, TimeSpan?, CancellationToken)` | Extension on `Func<Task>` — retries up to `maxAttempts` times with optional delay |
| `Retry<T>(int, TimeSpan?, CancellationToken)` | Extension on `Func<Task<T>>` — same as above; returns the first successful result |

---

## Usage Examples

```csharp
using Oxtensions.Async;

// Fire-and-forget (log errors silently)
SendEmailAsync(user).FireAndForget(ex => logger.LogError(ex, "Email failed"));

// Timeout enforcement
await FetchDataAsync().WithTimeout(TimeSpan.FromSeconds(10));

// Cancellation wrapping
await LongRunningAsync().WithCancellation(cancellationToken);

// Retry (extension on Func<Task>)
Func<Task> call = () => CallExternalApiAsync();
await call.Retry(maxAttempts: 3, delay: TimeSpan.FromMilliseconds(500));

// Retry with result (extension on Func<Task<T>>)
Func<Task<string>> fetch = () => GetResultAsync();
var result = await fetch.Retry(maxAttempts: 5, delay: TimeSpan.FromSeconds(1));
```

---

## Notes

- `FireAndForget` uses `Task.ContinueWith` with `TaskContinuationOptions.OnlyOnFaulted` to safely capture exceptions without crashing the process.
- `WithTimeout` uses `Task.WhenAny` + `Task.Delay` — the underlying task continues running but its result is discarded when the race is lost.
- `Retry` re-throws the last exception after exhausting all attempts.
