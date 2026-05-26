using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Servus.Application;
using Servus.Application.Startup;
using Xunit;

namespace Servus.Tests.Application.Startup;

public class ServiceSetup : IServiceSetupContainer
{
    public bool WasCalled { get; set; }

    public void SetupServices(IServiceCollection services, IConfiguration configuration)
    {
        WasCalled = true;
    }
}

public class ConfigSetup : IConfigurationSetupContainer
{
    public bool WasCalled { get; set; }

    public void SetupConfiguration(IConfigurationManager builder)
    {
        WasCalled = true;
    }
}

public class LoggingSetup : ILoggingSetupContainer
{
    public bool WasCalled { get; set; }

    public void SetupLogging(ILoggingBuilder builder)
    {
        WasCalled = true;
    }
}

public class HostBuilderSetup : IHostApplicationBuilderSetupContainer
{
    public bool WasCalled { get; set; }

    public void ConfigureHostApplicationBuilder(IHostApplicationBuilder builder)
    {
        WasCalled = true;
    }
}

public class AppStartupTests
{
    [Fact]
    public void BuilderInvokesServiceSetupContainer()
    {
        var container = new ServiceSetup();
        var builder = ServusApplication.CreateBuilder();
        builder.WithSetup(container);
        var app = builder.Build();

        Assert.True(container.WasCalled);
        Assert.NotNull(app.Services);
    }

    [Fact]
    public void BuilderInvokesConfigSetupContainer()
    {
        var container = new ConfigSetup();
        var builder = ServusApplication.CreateBuilder();
        builder.WithSetup(container);
        builder.Build();

        Assert.True(container.WasCalled);
    }

    [Fact]
    public void BuilderInvokesLoggingSetupContainer()
    {
        var container = new LoggingSetup();
        var builder = ServusApplication.CreateBuilder();
        builder.WithSetup(container);
        builder.Build();

        Assert.True(container.WasCalled);
    }

    [Fact]
    public void BuilderInvokesHostApplicationBuilderSetupContainer()
    {
        var container = new HostBuilderSetup();
        var builder = ServusApplication.CreateBuilder();
        builder.WithSetup(container);
        builder.Build();

        Assert.True(container.WasCalled);
    }

    [Fact]
    public void WithSetupGenericCreatesAndRegisters()
    {
        var builder = ServusApplication.CreateBuilder();
        builder.WithSetup<ServiceSetup>();
        var app = builder.Build();
        Assert.NotNull(app);
    }

    [Fact]
    public async Task SuccessfulStartupWithGate()
    {
        var gateIsOpen = false;
        var cts = new CancellationTokenSource();
        var app = ServusApplication.CreateBuilder()
            .WithStartupGate(() =>
            {
                gateIsOpen = !gateIsOpen;
                return Task.FromResult(gateIsOpen);
            })
            .Build();

        Assert.False(gateIsOpen);

        await app.StartAsync(cts.Token);

        Assert.True(gateIsOpen);
        await cts.CancelAsync();
    }

    [Fact]
    public async Task CreateBuilderWithArgs()
    {
        var cts = new CancellationTokenSource();
        var app = ServusApplication.CreateBuilder([])
            .Build();

        await app.StartAsync(cts.Token);
        Assert.NotNull(app.Services);
        await cts.CancelAsync();
    }

    [Fact]
    public void GetEnvironmentVariable_ReturnsServusPrefixed()
    {
        Environment.SetEnvironmentVariable("SERVUS_TESTVAR", "hello");
        try
        {
            Assert.Equal("hello", ServusApplication.GetEnvironmentVariable("testvar"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("SERVUS_TESTVAR", null);
        }
    }

    [Fact]
    public void IsEnvironmentVariableSetTo_ComparesIgnoreCase()
    {
        Environment.SetEnvironmentVariable("SERVUS_TESTFLAG", "True");
        try
        {
            Assert.True(ServusApplication.IsEnvironmentVariableSetTo("testflag", "true"));
            Assert.False(ServusApplication.IsEnvironmentVariableSetTo("testflag", "false"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("SERVUS_TESTFLAG", null);
        }
    }
}
