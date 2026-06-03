using System.Diagnostics.Metrics;
using System.Numerics;

namespace Servus.Diagnostics;

public static class CounterExtensions
{
    public static void Up<T>(this UpDownCounter<T> counter)
        where T : struct, INumber<T>
    {
        counter.Add(T.One);
    }

    public static void Down<T>(this UpDownCounter<T> counter)
        where T : struct, INumber<T>
    {
        counter.Add(-T.One);
    }

    public static void Up<T>(this UpDownCounter<T> counter, T value)
        where T : struct, INumber<T>
    {
        counter.Add(T.Abs(value));
    }

    public static void Down<T>(this UpDownCounter<T> counter, T value)
        where T : struct, INumber<T>
    {
        counter.Add(-T.Abs(value));
    }

    public static void Up<T>(this Counter<T> counter, params KeyValuePair<string, object?>[] tags)
        where T : struct, INumber<T>
    {
        counter.Add(T.One, tags.AsSpan());
    }

    public static void Down<T>(this Counter<T> counter, params KeyValuePair<string, object?>[]? tags)
        where T : struct, INumber<T>
    {
        counter.Add(-T.One, tags.AsSpan());
    }

    public static void Up<T>(this Counter<T> counter, T value, params KeyValuePair<string, object?>[] tags)
        where T : struct, INumber<T>
    {
        counter.Add(T.Abs(value), tags.AsSpan());
    }

    public static void Down<T>(this Counter<T> counter, T value, params KeyValuePair<string, object?>[]? tags)
        where T : struct, INumber<T>
    {
        counter.Add(-T.Abs(value), tags.AsSpan());
    }
}