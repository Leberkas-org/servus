using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Servus.Application;

public class ServusApplication
{
    private readonly IHost _host;
    private readonly ServusApplicationBuilder _builder;

    protected internal ServusApplication(IHost host, ServusApplicationBuilder builder)
    {
        _host = host;
        _builder = builder;
    }

    public IServiceProvider Services => _host.Services;

    public static ServusApplicationBuilder CreateBuilder() => new();

    public static ServusApplicationBuilder CreateBuilder(string[] args) => new(args);

    public async Task RunAsync(CancellationToken token = default)
        => await InternalStartAsync(host => host.RunAsync(token), token);

    public async Task StartAsync(CancellationToken token = default)
        => await InternalStartAsync(host => host.StartAsync(token), token);

    private async Task InternalStartAsync(Func<IHost, Task> startupMethod, CancellationToken token)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            RegisterLifecycleEvents();
            await RunStartupGatesAsync(cts.Token);
            await startupMethod(_host).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await cts.CancelAsync();
            System.Console.WriteLine(e);
            throw;
        }
    }

    private void RegisterLifecycleEvents()
    {
        var lifetime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() => _builder.StartedAction(Services));
        lifetime.ApplicationStopping.Register(() => _builder.StoppingAction());
        lifetime.ApplicationStopped.Register(() => _builder.StoppedAction());
    }

    private async Task RunStartupGatesAsync(CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromSeconds(1);
        var maxDelay = TimeSpan.FromSeconds(60);
        var logger = Services.GetService<ILogger<ServusApplication>>();
        var gates = _builder.GetGates();

        while (!cancellationToken.IsCancellationRequested)
        {
            var tasks = gates.Select(gate => gate.CheckAsync(cancellationToken));
            var results = await Task.WhenAll(tasks);

            if (results.All(result => result))
            {
                logger?.LogDebug("All startup gates are cleared!");
                return;
            }

            await Task.Delay(delay, cancellationToken);
            var delayMs = delay.TotalMilliseconds * 2;
            delay = TimeSpan.FromMilliseconds(Math.Min(delayMs, maxDelay.TotalMilliseconds));

            logger?.LogWarning("Not all startup gates are clear. Next retry in [{CurrentDelay}]", delay);
        }
    }

    public static string? GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable("SERVUS_" + name.ToUpper());
    }

    public static bool IsEnvironmentVariableSetTo(string name, string value)
    {
        var environmentVariable = GetEnvironmentVariable(name);
        return environmentVariable != null && environmentVariable.Equals(value, StringComparison.OrdinalIgnoreCase);
    }
}
