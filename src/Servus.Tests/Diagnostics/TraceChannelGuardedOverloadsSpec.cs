using Servus.Diagnostics;
using Xunit;
using TraceLevel = Servus.Diagnostics.TraceLevel;

namespace Servus.Tests.Diagnostics;

/// <summary>
/// The fixed-arity Trace/Debug/... overloads must check IsEnabled BEFORE touching their
/// arguments, so a disabled trace call with value-type args allocates nothing (no params
/// object?[] array, no boxing) — the hot-path cost the params overload pays at the call site.
/// </summary>
[Collection("OTEL")]
public sealed class TraceChannelGuardedOverloadsSpec : IDisposable
{
    private sealed class MockListener : IServusTraceListener
    {
        public List<TraceEvent> Events { get; } = [];
        public bool IsEnabled(TraceLevel level, string category) => true;
        public void Write(in TraceEvent evt) => Events.Add(evt);
    }

    private readonly MockListener _mock = new();

    public TraceChannelGuardedOverloadsSpec() => Senf.Tracing.Disable();

    public void Dispose() => Senf.Tracing.Disable();

    [Fact(Timeout = 5000)]
    public void Two_arg_overload_should_not_allocate_when_disabled()
    {
        var channel = Senf.Tracing.For("Pool");

        // Warm up the JIT so the measured loop is steady-state.
        for (var i = 0; i < 50; i++)
        {
            channel.Debug(this, "x={0} y={1}", i, i + 1);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
        {
            channel.Debug(this, "x={0} y={1}", i, i + 1);
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
    }

    [Fact(Timeout = 5000)]
    public void Three_arg_overload_should_not_allocate_when_disabled()
    {
        var channel = Senf.Tracing.For("Pool");

        for (var i = 0; i < 50; i++)
        {
            channel.Debug(this, "{0}/{1}/{2}", i, i + 1, i + 2);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
        {
            channel.Debug(this, "{0}/{1}/{2}", i, i + 1, i + 2);
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
    }

    [Fact(Timeout = 5000)]
    public void Two_arg_overload_should_emit_formatted_event_when_enabled()
    {
        Senf.Tracing.Configure(_mock);
        var channel = Senf.Tracing.For("Connection");

        channel.Debug(this, "tcp connected to {0}:{1}", "localhost", 443);

        Assert.Single(_mock.Events);
        Assert.Equal("tcp connected to localhost:443", _mock.Events[0].FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Three_arg_overload_should_emit_formatted_event_when_enabled()
    {
        Senf.Tracing.Configure(_mock);
        var channel = Senf.Tracing.For("Connection");

        channel.Trace(this, "{0} {1} {2}", "a", "b", "c");

        Assert.Single(_mock.Events);
        Assert.Equal("a b c", _mock.Events[0].FormatMessage());
    }
}
