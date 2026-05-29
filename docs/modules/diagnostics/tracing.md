# Tracing

Zero-cost developer tracing built on `System.Diagnostics.ActivitySource`. When no listener is configured, trace calls are no-ops (single null-check).

The entry point is `Senf.Tracing` — a static `ServusTrace` instance. You create named channels, trace through them, and wire up a listener at startup.

## Quick start

```csharp
using Servus.Diagnostics;

// 1. Create a channel (store in a static field for zero-allocation reuse)
private static readonly TraceChannel _trace = Senf.Tracing.For("OrderService");

// 2. Trace at any level
_trace.Info(this, "Processing order {0}", orderId);
_trace.Debug(this, "Calculated total: {0:C}", total);
_trace.Warning(this, "Retry attempt {0} of {1}", attempt, maxRetries);
```

Without a listener configured, every call above is a no-op.

## Configuring a listener

### Via DI — bridge to `ILogger`

The most common setup: pipe trace events into the standard logging pipeline.

```csharp
using Servus.Diagnostics;

builder.Services.AddServusLoggerTracing(TraceLevel.Debug);
```

Filter by category:

```csharp
builder.Services.AddServusLoggerTracing(
    TraceLevel.Debug,
    "OrderService", "PaymentService");

// Or with a predicate
builder.Services.AddServusLoggerTracing(
    TraceLevel.Debug,
    category => category.StartsWith("Order"));
```

### Manual — custom listener

Implement `IServusTraceListener` for custom sinks (metrics, file, network):

```csharp
public class ConsoleTraceListener : IServusTraceListener
{
    public bool IsEnabled(TraceLevel level, string category) => true;

    public void Write(in TraceEvent evt)
    {
        Console.WriteLine($"[{evt.Level}] {evt.Category}/{evt.SourceType}: {evt.FormatMessage()}");
    }
}

// Register
builder.Services.AddServusTraceListener(
    new ConsoleTraceListener(),
    TraceLevel.Trace);
```

## `TraceChannel`

A channel is bound to a single category. It exposes level-specific methods:

```csharp
private static readonly TraceChannel _trace = Senf.Tracing.For("MyCategory");

_trace.Trace(this, "Finest detail");
_trace.Debug(this, "Diagnostic info");
_trace.Info(this, "Notable event: {0}", eventName);
_trace.Warning(this, "Unexpected state: {0}", state);
_trace.Error(this, "Failed: {0}", exception.Message);
```

Each method takes the source object (`this`) for identity tracking and a format template with optional arguments. Formatting is deferred — the `TraceEvent.FormatMessage()` allocation only happens inside the listener.

## `TraceEvent`

An immutable struct passed to listeners:

| Property | Type | Description |
|---|---|---|
| `TimestampTicks` | `long` | `Stopwatch.GetTimestamp()` value |
| `Level` | `TraceLevel` | Severity |
| `Category` | `string` | Channel category name |
| `SourceType` | `string` | Short type name of the source object |
| `SourceHash` | `int` | Identity hash of the source object |
| `Template` | `string` | Format template |

Call `FormatMessage()` to produce the formatted string.

## `TraceLevel`

```csharp
public enum TraceLevel : byte
{
    Trace   = 0,
    Debug   = 1,
    Info    = 2,
    Warning = 3,
    Error   = 4,
}
```

Maps directly to `Microsoft.Extensions.Logging.LogLevel`.

## `ActivitySource`

`ServusTrace` also exposes an `ActivitySource` named `"Servus"` for OpenTelemetry integration:

```csharp
using var activity = Senf.Tracing.Source.StartActivity("process-order");
```

## API

```csharp
public static class Senf
{
    public static readonly ServusTrace Tracing;
    public static readonly ServusMetrics Metrics;
}

public class ServusTrace
{
    public ActivitySource Source { get; }

    public void Configure(IServusTraceListener listener,
        TraceLevel minimumLevel = TraceLevel.Trace,
        Func<string, bool>? categoryFilter = null);

    public TraceChannel For(string categoryName);
    public void Disable();
}

public readonly struct TraceChannel
{
    public void Trace<T>(T source, string message, params object?[] args);
    public void Debug<T>(T source, string message, params object?[] args);
    public void Info<T>(T source, string message, params object?[] args);
    public void Warning<T>(T source, string message, params object?[] args);
    public void Error<T>(T source, string message, params object?[] args);
}

public interface IServusTraceListener
{
    bool IsEnabled(TraceLevel level, string category);
    void Write(in TraceEvent evt);
}

public static class ServusTraceExtensions
{
    public static IServiceCollection AddServusLoggerTracing(
        this IServiceCollection services,
        TraceLevel minimumLevel = TraceLevel.Debug,
        params string[] categories);

    public static IServiceCollection AddServusLoggerTracing(
        this IServiceCollection services,
        TraceLevel minimumLevel = TraceLevel.Debug,
        Func<string, bool>? categoryFilter = null);

    public static IServiceCollection AddServusTraceListener(
        this IServiceCollection services,
        IServusTraceListener listener,
        TraceLevel minimumLevel = TraceLevel.Debug,
        params string[] categories);

    public static IServiceCollection AddServusTraceListener(
        this IServiceCollection services,
        IServusTraceListener listener,
        TraceLevel minimumLevel = TraceLevel.Debug,
        Func<string, bool>? categoryFilter = null);
}
```
