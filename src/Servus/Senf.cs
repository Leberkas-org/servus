using Servus.Diagnostics;

namespace Servus;

public static class Senf
{
    public static readonly ServusTrace Tracing = new();
    public static readonly ServusMetrics Metrics = new();
}