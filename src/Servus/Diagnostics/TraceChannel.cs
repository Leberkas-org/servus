using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Servus.Diagnostics;

/// <summary>
/// A trace channel bound to a single category.
/// Obtain channels with <see cref="ServusTrace.For(string)"/>.
/// </summary>
/// <remarks>
/// Two overload families exist for each level:
/// <list type="bullet">
/// <item>The <c>params object?[]</c> form, which the compiler fills by allocating an array and
/// boxing value-type arguments at the call site — paid even when tracing is disabled.</item>
/// <item>The fixed-arity generic forms (1–3 args), which check <see cref="IsEnabled"/> first and
/// only then forward, so a disabled call allocates nothing. Overload resolution prefers these for
/// 1–3 arguments; 4+ arguments fall back to the params form.</item>
/// </list>
/// </remarks>
public readonly struct TraceChannel(string category)
{
    /// <summary>Whether a trace at <paramref name="level"/> for this channel's category would emit.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEnabled(TraceLevel level) => Senf.Tracing.IsEnabled(category, level);

    public void Trace<T>(T source, string message, params object?[] args)
        => Senf.Tracing.Trace(source, TraceLevel.Trace, category, message, null, args);

    public void Debug<T>(T source, string message, params object?[] args)
        => Senf.Tracing.Trace(source, TraceLevel.Debug, category, message, null, args);

    public void Info<T>(T source, string message, params object?[] args)
        => Senf.Tracing.Trace(source, TraceLevel.Info, category, message, null, args);

    public void Warning<T>(T source, string message, params object?[] args)
        => Senf.Tracing.Trace(source, TraceLevel.Warning, category, message, null, args);

    public void Error<T>(T source, string message, params object?[] args)
        => Senf.Tracing.Trace(source, TraceLevel.Error, category, message, null, args);

    public void Trace<T, TArg1>(T source, string message, TArg1 arg1)
    {
        if (IsEnabled(TraceLevel.Trace))
        {
            Senf.Tracing.Trace(source, TraceLevel.Trace, category, message, null, arg1);
        }
    }

    public void Trace<T, TArg1, TArg2>(T source, string message, TArg1 arg1, TArg2 arg2)
    {
        if (IsEnabled(TraceLevel.Trace))
        {
            Senf.Tracing.Trace(source, TraceLevel.Trace, category, message, null, arg1, arg2);
        }
    }

    public void Trace<T, TArg1, TArg2, TArg3>(T source, string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        if (IsEnabled(TraceLevel.Trace))
        {
            Senf.Tracing.Trace(source, TraceLevel.Trace, category, message, null, arg1, arg2, arg3);
        }
    }

    public void Debug<T, TArg1>(T source, string message, TArg1 arg1)
    {
        if (IsEnabled(TraceLevel.Debug))
        {
            Senf.Tracing.Trace(source, TraceLevel.Debug, category, message, null, arg1);
        }
    }

    public void Debug<T, TArg1, TArg2>(T source, string message, TArg1 arg1, TArg2 arg2)
    {
        if (IsEnabled(TraceLevel.Debug))
        {
            Senf.Tracing.Trace(source, TraceLevel.Debug, category, message, null, arg1, arg2);
        }
    }

    public void Debug<T, TArg1, TArg2, TArg3>(T source, string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        if (IsEnabled(TraceLevel.Debug))
        {
            Senf.Tracing.Trace(source, TraceLevel.Debug, category, message, null, arg1, arg2, arg3);
        }
    }

    public void Info<T, TArg1>(T source, string message, TArg1 arg1)
    {
        if (IsEnabled(TraceLevel.Info))
        {
            Senf.Tracing.Trace(source, TraceLevel.Info, category, message, null, arg1);
        }
    }

    public void Info<T, TArg1, TArg2>(T source, string message, TArg1 arg1, TArg2 arg2)
    {
        if (IsEnabled(TraceLevel.Info))
        {
            Senf.Tracing.Trace(source, TraceLevel.Info, category, message, null, arg1, arg2);
        }
    }

    public void Info<T, TArg1, TArg2, TArg3>(T source, string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        if (IsEnabled(TraceLevel.Info))
        {
            Senf.Tracing.Trace(source, TraceLevel.Info, category, message, null, arg1, arg2, arg3);
        }
    }

    public void Warning<T, TArg1>(T source, string message, TArg1 arg1)
    {
        if (IsEnabled(TraceLevel.Warning))
        {
            Senf.Tracing.Trace(source, TraceLevel.Warning, category, message, null, arg1);
        }
    }

    public void Warning<T, TArg1, TArg2>(T source, string message, TArg1 arg1, TArg2 arg2)
    {
        if (IsEnabled(TraceLevel.Warning))
        {
            Senf.Tracing.Trace(source, TraceLevel.Warning, category, message, null, arg1, arg2);
        }
    }

    public void Warning<T, TArg1, TArg2, TArg3>(T source, string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        if (IsEnabled(TraceLevel.Warning))
        {
            Senf.Tracing.Trace(source, TraceLevel.Warning, category, message, null, arg1, arg2, arg3);
        }
    }

    public void Error<T, TArg1>(T source, string message, TArg1 arg1)
    {
        if (IsEnabled(TraceLevel.Error))
        {
            Senf.Tracing.Trace(source, TraceLevel.Error, category, message, null, arg1);
        }
    }

    public void Error<T, TArg1, TArg2>(T source, string message, TArg1 arg1, TArg2 arg2)
    {
        if (IsEnabled(TraceLevel.Error))
        {
            Senf.Tracing.Trace(source, TraceLevel.Error, category, message, null, arg1, arg2);
        }
    }

    public void Error<T, TArg1, TArg2, TArg3>(T source, string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        if (IsEnabled(TraceLevel.Error))
        {
            Senf.Tracing.Trace(source, TraceLevel.Error, category, message, null, arg1, arg2, arg3);
        }
    }
}
