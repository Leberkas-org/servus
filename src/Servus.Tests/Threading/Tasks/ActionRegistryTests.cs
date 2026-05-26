using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servus.Collections;
using Servus.Threading.Tasks;
using Xunit;

namespace Servus.Tests.Threading.Tasks;

public class ActionRegistryTests
{
    public class TestInjectable
    {
        public static Task RunAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public static Task<T> RunAsync<T>(T value, CancellationToken token)
        {
            return Task.FromResult(value);
        }
    }

    public class TestTask : IAsyncTask, IAsyncTask<bool>
    {
        private readonly TestInjectable _injectable;

        public TestTask(TestInjectable injectable)
        {
            _injectable = injectable;
        }

        public async ValueTask RunAsync(CancellationToken token) => await TestInjectable.RunAsync(token);
        async ValueTask<bool> IAsyncTask<bool>.RunAsync(CancellationToken token) => await TestInjectable.RunAsync(true, token);
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<TestInjectable>();
        return builder.Build().Services;
    }

    [Fact]
    public async Task RegisterTaskTest()
    {
        var services = BuildServiceProvider();
        var registry = new ActionRegistry<IAsyncTask>();
        registry.Register<TestTask>();

        var cts = new CancellationTokenSource();
        await registry.RunAsyncParallel(services, (f, c) => f.RunAsync(c), cts.Token);
        await registry.RunAllAsync(services, (f, t) => f.RunAsync(t), cts.Token);
    }

    [Fact]
    public async Task RegisterAsyncTaskTest()
    {
        var services = BuildServiceProvider();
        var registry = new ActionRegistry<IAsyncTask<bool>, bool>();
        registry.Register<TestTask>();

        var cts = new CancellationTokenSource();

        var any = await registry.RunAllAsync(services, cts.Token).AnyAsync();
        var all = await registry.RunAllAsync(services, cts.Token).AllAsync();

        Assert.True(any);
        Assert.True(all);
    }
}
