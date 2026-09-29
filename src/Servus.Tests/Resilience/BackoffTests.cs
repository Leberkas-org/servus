using Servus.Resilience;
using Xunit;

namespace Servus.Tests.Resilience;

public class BackoffTests
{
    #region Create

    [Fact]
    public void Create_ReturnsPolicy()
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(500));

        Assert.NotNull(policy);
    }

    [Fact]
    public void Create_MultiplierLessThanOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Backoff.Create(TimeSpan.FromMilliseconds(100), multiplier: 0.5));
    }

    #endregion

    #region Delay

    [Fact]
    public void Delay_Attempt0_ReturnsInitialDelay()
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(500));

        Assert.Equal(TimeSpan.FromMilliseconds(500), policy.Delay(0));
    }

    [Theory]
    [InlineData(0, 500)]
    [InlineData(1, 1000)]
    [InlineData(2, 2000)]
    [InlineData(3, 4000)]
    [InlineData(4, 8000)]
    public void Delay_FollowsExpectedProgression(int attempt, double expectedMs)
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(500));

        Assert.Equal(expectedMs, policy.Delay(attempt).TotalMilliseconds);
    }

    [Fact]
    public void Delay_WithCustomMultiplier()
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(100), multiplier: 3.0);

        Assert.Equal(TimeSpan.FromMilliseconds(900), policy.Delay(2));
    }

    [Fact]
    public void Delay_CapsAtMaxDelay()
    {
        var maxDelay = TimeSpan.FromSeconds(5);
        var policy = Backoff.Create(TimeSpan.FromSeconds(1), maxDelay: maxDelay);

        Assert.Equal(maxDelay, policy.Delay(10));
    }

    [Fact]
    public void Delay_VeryHighAttempt_CapsWithoutOverflow()
    {
        var maxDelay = TimeSpan.FromMinutes(1);
        var policy = Backoff.Create(TimeSpan.FromSeconds(1), maxDelay: maxDelay);

        Assert.Equal(maxDelay, policy.Delay(100));
    }

    [Fact]
    public void Delay_NegativeAttempt_Throws()
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(100));

        Assert.Throws<ArgumentOutOfRangeException>(() => policy.Delay(-1));
    }

    #endregion

    #region DelayWithJitter

    [Fact]
    public void DelayWithJitter_StaysWithinExpectedRange()
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(1000));

        for (var i = 0; i < 100; i++)
        {
            var delay = policy.DelayWithJitter(0);
            Assert.InRange(delay.TotalMilliseconds, 750, 1250);
        }
    }

    [Fact]
    public void DelayWithJitter_CapsAtMaxDelay()
    {
        var maxDelay = TimeSpan.FromSeconds(5);
        var policy = Backoff.Create(TimeSpan.FromSeconds(1), maxDelay: maxDelay);

        for (var i = 0; i < 100; i++)
        {
            Assert.True(policy.DelayWithJitter(10) <= maxDelay);
        }
    }

    [Fact]
    public void DelayWithJitter_ProducesVariation()
    {
        var policy = Backoff.Create(TimeSpan.FromMilliseconds(1000));
        var delays = new HashSet<double>();

        for (var i = 0; i < 20; i++)
        {
            delays.Add(Math.Round(policy.DelayWithJitter(0).TotalMilliseconds));
        }

        Assert.True(delays.Count > 1, "Jitter should produce varying delays");
    }

    #endregion
}
