using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Servus.Diagnostics;

/// <summary>
/// Extension methods for registering <see cref="ServusTrace"/> services with
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ServusTraceExtensions
{
    public static IServiceCollection AddServusLoggerTracing(
        this IServiceCollection services,
        TraceLevel minimumLevel = TraceLevel.Debug,
        params string[] categories)
    {
        services.AddSingleton<IServusTraceListener>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var listener = new TraceLogger(loggerFactory, minimumLevel, categories.Contains);
            Senf.Tracing.Configure(listener, minimumLevel, categories.Contains);
            return listener;
        });
        return services;
    }

    public static IServiceCollection AddServusLoggerTracing(
        this IServiceCollection services,
        TraceLevel minimumLevel = TraceLevel.Debug,
        Func<string, bool>? categoryFilter = null)
    {
        services.AddSingleton<IServusTraceListener>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var listener = new TraceLogger(loggerFactory, minimumLevel, categoryFilter);
            Senf.Tracing.Configure(listener, minimumLevel, categoryFilter);
            return listener;
        });
        return services;
    }

    public static IServiceCollection AddServusTraceListener(
        this IServiceCollection services,
        IServusTraceListener listener,
        TraceLevel minimumLevel = TraceLevel.Debug,
        params string[] categories)
    {
        ArgumentNullException.ThrowIfNull(listener);
        Senf.Tracing.Configure(listener, minimumLevel, categories.Contains);
        services.AddSingleton(listener);
        return services;
    }

    public static IServiceCollection AddServusTraceListener(
        this IServiceCollection services,
        IServusTraceListener listener,
        TraceLevel minimumLevel = TraceLevel.Debug,
        Func<string, bool>? categoryFilter = null)
    {
        ArgumentNullException.ThrowIfNull(listener);
        Senf.Tracing.Configure(listener, minimumLevel, categoryFilter);
        services.AddSingleton(listener);
        return services;
    }
}
