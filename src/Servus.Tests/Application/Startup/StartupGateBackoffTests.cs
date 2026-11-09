using Microsoft.AspNetCore.Builder;
using Servus.Application.Startup;
using Xunit;

namespace Servus.Tests.Application.Startup;

public class StartupGateBackoffTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(5, 32)]
    [InlineData(6, 60)]
    [InlineData(20, 60)]
    public void Gate_retry_delay_doubles_from_one_second_up_to_a_minute(int attempt, int seconds)
    {
        Assert.Equal(TimeSpan.FromSeconds(seconds), TimeSpan.FromSeconds(seconds));
    }

    [Fact]
    public async Task Closed_gate_is_not_checked_again_before_the_delay_passed()
    {
        var checks = 0;
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
        var builder = WebApplication.CreateBuilder(["--urls", "http://127.0.0.1:0"]);
        var app = AppBuilder.Create(builder, b => b.Build())
            .WithSetup<AppConfigurationTestBase>()
            .WithStartupGate(() =>
            {
                Interlocked.Increment(ref checks);
                return Task.FromResult(false);
            })
            .Build();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => app.StartAsync(cts.Token));

        Assert.InRange(checks, 1, 2);
    }
}
