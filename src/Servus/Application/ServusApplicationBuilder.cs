using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Servus.Application.Startup;
using Servus.Application.Startup.Gates;

namespace Servus.Application;

public sealed class ServusApplicationBuilder
{
    private readonly HostApplicationBuilder _hostBuilder;
    private readonly List<ISetupContainer> _containers = [];
    private readonly List<IStartupGate> _gates = [];

    internal Action<IServiceProvider> StartedAction = _ => { };
    internal Action StoppingAction = () => { };
    internal Action StoppedAction = () => { };

    internal ServusApplicationBuilder(string[]? args = null)
    {
        _hostBuilder = args is null ? Host.CreateApplicationBuilder() : Host.CreateApplicationBuilder(args);
    }

    public IServiceCollection Services => _hostBuilder.Services;
    public ConfigurationManager Configuration => _hostBuilder.Configuration;
    public ILoggingBuilder Logging => _hostBuilder.Logging;

    public ServusApplicationBuilder WithSetup<TContainer>() where TContainer : class, ISetupContainer, new()
        => WithSetup(new TContainer());

    public ServusApplicationBuilder WithSetup(ISetupContainer container)
    {
        _containers.Add(container);
        return this;
    }

    public ServusApplicationBuilder WithStartupGate<TGate>() where TGate : class, IStartupGate, new()
        => WithStartupGate(new TGate());

    public ServusApplicationBuilder WithStartupGate(Func<Task<bool>> gate)
        => WithStartupGate(new ActionStartupGate(gate));

    public ServusApplicationBuilder WithStartupGate(IStartupGate gate)
    {
        _gates.Add(gate);
        return this;
    }

    public ServusApplicationBuilder OnApplicationStarted(Action<IServiceProvider> action)
    {
        StartedAction = action;
        return this;
    }

    public ServusApplicationBuilder OnApplicationStopping(Action action)
    {
        StoppingAction = action;
        return this;
    }

    public ServusApplicationBuilder OnApplicationStopped(Action action)
    {
        StoppedAction = action;
        return this;
    }

    public ServusApplication Build() => Build((host, builder) => new ServusApplication(host, builder));

    public TApp Build<TApp>(Func<IHost, ServusApplicationBuilder, TApp> factory) where TApp : ServusApplication
    {
        foreach (var container in _containers)
        {
            if (container is ILoggingSetupContainer logging)
                logging.SetupLogging(_hostBuilder.Logging);

            if (container is IConfigurationSetupContainer config)
                config.SetupConfiguration(_hostBuilder.Configuration);

            if (container is IServiceSetupContainer service)
                service.SetupServices(_hostBuilder.Services, _hostBuilder.Configuration);

            if (container is IHostApplicationBuilderSetupContainer hostSetup)
                hostSetup.ConfigureHostApplicationBuilder(_hostBuilder);
        }

        var host = _hostBuilder.Build();
        var app = factory(host, this);

        foreach (var container in _containers)
        {
            if (container is ApplicationSetupContainer appSetup)
                appSetup.InjectApp(app);
        }

        return app;
    }

    internal IReadOnlyCollection<IStartupGate> GetGates() => _gates;
}
