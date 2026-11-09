using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Servus.Resilience;

namespace Servus.Application.Startup;

public partial class AppRunner
{
    /// <summary>Wait between checks of closed startup gates: 1 s, doubling up to 60 s.</summary>
    internal static readonly BackoffPolicy GateBackoff = Backoff.Create(TimeSpan.FromSeconds(1), 2.0, TimeSpan.FromSeconds(60));

    private async Task RunStartupGatesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetService<ILogger<AppRunner>>();

        for (var attempt = 0; !cancellationToken.IsCancellationRequested; attempt++)
        {
            var tasks = _startupGates.Select(gate => gate.CheckAsync(cancellationToken));
            var results = await Task.WhenAll(tasks);

            if (results.All(result => result))
            {
                logger?.LogDebug("All startup gates are cleared!");
                return; // All gates are ready
            }

            var delay = GateBackoff.Delay(attempt);
            logger?.LogWarning("Not all startup gates are clear. Next retry in [{CurrentDelay}]", delay);
            await Task.Delay(delay, cancellationToken);
        }
    }
}
