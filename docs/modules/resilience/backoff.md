# Backoff

Pure calculation helper for **exponential backoff delays**. Instead of hardcoding delay arrays or hand-rolling `Math.Pow` in every retry loop, create a `BackoffPolicy` once and call `Delay(attempt)`.

## Usage

```csharp
using Servus.Resilience;

private static readonly BackoffPolicy _backoff =
    Backoff.Create(TimeSpan.FromMilliseconds(500), maxDelay: TimeSpan.FromSeconds(30));

// Wherever you need the delay:
var delay = _backoff.Delay(attempt);
// 500ms → 1s → 2s → 4s → …
```

### With jitter

Jitter prevents multiple callers from retrying in lock-step (thundering herd):

```csharp
var delay = _backoff.DelayWithJitter(attempt);
```

### With Akka.NET timers

```csharp
private static readonly BackoffPolicy _backoff =
    Backoff.Create(TimeSpan.FromMilliseconds(500), maxDelay: TimeSpan.FromSeconds(30));

private void HandleQueueFull(int attempt)
{
    if (attempt < MaxRetries)
    {
        Timers.StartSingleTimer("retry", new RetryQuery(attempt + 1), _backoff.Delay(attempt));
    }
}
```

## Semantics

- `attempt` is zero-based — attempt `0` returns `initialDelay`.
- The delay for attempt `n` is `initialDelay × multiplier^n`.
- When the calculated delay exceeds `maxDelay`, `maxDelay` is returned.
- Overflow from very high attempt counts is handled safely (caps at `maxDelay`).
- `DelayWithJitter` multiplies the base delay by a random factor in `[0.75, 1.25)`.

## Default progression (500 ms initial, 2× multiplier)

| Attempt | Delay |
|---|---|
| 0 | 500 ms |
| 1 | 1 s |
| 2 | 2 s |
| 3 | 4 s |
| 4 | 8 s |
| 5 | 16 s |

## API

```csharp
public static class Backoff
{
    public static BackoffPolicy Create(
        TimeSpan initialDelay,
        double multiplier = 2.0,
        TimeSpan? maxDelay = null);
}

public sealed class BackoffPolicy
{
    public TimeSpan Delay(int attempt);
    public TimeSpan DelayWithJitter(int attempt);
}
```

## When to use it

- **Actor retry timers** — compute the next delay for `Timers.StartSingleTimer`.
- **Manual retry loops** — anywhere you'd otherwise hardcode a `TimeSpan[]` of backoff values.
- **Queue-full / rate-limit handling** — back off before retrying a request.

## When NOT to use it

- You need a full retry policy with circuit breakers, bulkheads, etc. — use [Polly](https://github.com/App-vNext/Polly).
- You want an all-in-one "call this function with retries" helper — wrap the policy in your own loop or use Polly's `WaitAndRetryAsync`.
