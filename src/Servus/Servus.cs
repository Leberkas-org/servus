using Servus.Diagnostics;

namespace Servus;

public static class Servus
{
    public static readonly ServusTrace Tracing = new();
    public static readonly ServusMetrics Metrics = new();
}