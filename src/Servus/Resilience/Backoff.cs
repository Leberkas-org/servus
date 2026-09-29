namespace Servus.Resilience;

public static class Backoff
{
    public static BackoffPolicy Create(TimeSpan initialDelay, double multiplier = 2.0, TimeSpan? maxDelay = null)
    {
        return new BackoffPolicy(initialDelay, multiplier, maxDelay);
    }
}

public sealed class BackoffPolicy
{
    private readonly TimeSpan _initialDelay;
    private readonly double _multiplier;
    private readonly TimeSpan _maxDelay;

    internal BackoffPolicy(TimeSpan initialDelay, double multiplier, TimeSpan? maxDelay)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(multiplier, 1.0);

        _initialDelay = initialDelay;
        _multiplier = multiplier;
        _maxDelay = maxDelay ?? TimeSpan.MaxValue;
    }

    public TimeSpan Delay(int attempt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(attempt);

        var delayMs = _initialDelay.TotalMilliseconds * Math.Pow(_multiplier, attempt);

        if (double.IsInfinity(delayMs) || delayMs > _maxDelay.TotalMilliseconds)
            return _maxDelay;

        return TimeSpan.FromMilliseconds(delayMs);
    }

    public TimeSpan DelayWithJitter(int attempt)
    {
        var baseDelay = Delay(attempt);
        var jitter = Random.Shared.NextDouble() * 0.5 + 0.75;
        var jittered = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * jitter);

        return jittered > _maxDelay ? _maxDelay : jittered;
    }
}
