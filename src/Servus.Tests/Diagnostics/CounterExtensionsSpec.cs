using System.Diagnostics.Metrics;
using Servus.Diagnostics;
using Xunit;

namespace Servus.Tests.Diagnostics;

public class CounterExtensionsSpec
{
    [Theory]
    [InlineData(5, 5)]
    [InlineData(-5, 5)]
    [InlineData(0, 0)]
    public void UpDownCounter_Up_Should_Record_Absolute_Value(int input, int expected)
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateUpDownCounter<int>("updown-up");

        int? measurement = null;

        using var listener = CreateListener<int>("updown-up", value => measurement = value);

        // Act
        counter.Up(input);

        // Assert
        Assert.Equal(expected, measurement);
    }

    [Theory]
    [InlineData(5, -5)]
    [InlineData(-5, -5)]
    [InlineData(0, 0)]
    public void UpDownCounter_Down_Should_Record_Negative_Absolute_Value(int input, int expected)
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateUpDownCounter<int>("updown-down");

        int? measurement = null;

        using var listener = CreateListener<int>("updown-down", value => measurement = value);

        // Act
        counter.Down(input);

        // Assert
        Assert.Equal(expected, measurement);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(-5, 5)]
    [InlineData(0, 0)]
    public void Counter_Up_Should_Record_Absolute_Value(int input, int expected)
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateCounter<int>("counter-up");

        int? measurement = null;

        using var listener = CreateListener<int>("counter-up", value => measurement = value);

        // Act
        counter.Up(input);

        // Assert
        Assert.Equal(expected, measurement);
    }

    [Theory]
    [InlineData(5, -5)]
    [InlineData(-5, -5)]
    [InlineData(0, 0)]
    public void Counter_Down_Should_Record_Negative_Absolute_Value(int input, int expected)
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateCounter<int>("counter-down");

        int? measurement = null;

        using var listener = CreateListener<int>("counter-down", value => measurement = value);

        // Act
        counter.Down(input);

        // Assert
        Assert.Equal(expected, measurement);
    }

    [Fact]
    public void UpDownCounter_Up_Without_Value_Should_Record_One()
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateUpDownCounter<int>("updown-up-one");

        int? measurement = null;

        using var listener = CreateListener<int>("updown-up-one", value => measurement = value);

        // Act
        counter.Up();

        // Assert
        Assert.Equal(1, measurement);
    }

    [Fact]
    public void UpDownCounter_Down_Without_Value_Should_Record_Minus_One()
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateUpDownCounter<int>("updown-down-one");

        int? measurement = null;

        using var listener = CreateListener<int>("updown-down-one", value => measurement = value);

        // Act
        counter.Down();

        // Assert
        Assert.Equal(-1, measurement);
    }

    [Fact]
    public void Counter_Up_Without_Value_Should_Record_One()
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateCounter<int>("counter-up-one");

        int? measurement = null;

        using var listener = CreateListener<int>("counter-up-one", value => measurement = value);

        // Act
        counter.Up();

        // Assert
        Assert.Equal(1, measurement);
    }

    [Fact]
    public void Counter_Down_Without_Value_Should_Record_Minus_One()
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateCounter<int>("counter-down-one");

        int? measurement = null;

        using var listener = CreateListener<int>("counter-down-one", value => measurement = value);

        // Act
        counter.Down();

        // Assert
        Assert.Equal(-1, measurement);
    }

    [Fact]
    public void Counter_Up_Should_Forward_Tags()
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateCounter<int>("counter-up-tags");

        KeyValuePair<string, object?>[]? recordedTags = null;

        using var listener = new MeterListener();

        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Name == "counter-up-tags")
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };

        listener.SetMeasurementEventCallback<int>((_, _, tags, _) => { recordedTags = tags.ToArray(); });

        listener.Start();

        // Act
        counter.Up(new KeyValuePair<string, object?>("tenant", "test"));

        // Assert
        Assert.NotNull(recordedTags);
        Assert.Contains(recordedTags!, t => t is { Key: "tenant", Value: "test" });
    }

    [Fact]
    public void Counter_Down_Should_Forward_Tags()
    {
        // Arrange
        var meter = new Meter(nameof(CounterExtensionsSpec));
        var counter = meter.CreateCounter<int>("counter-down-tags");

        KeyValuePair<string, object?>[]? recordedTags = null;

        using var listener = new MeterListener();

        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Name == "counter-down-tags")
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };

        listener.SetMeasurementEventCallback<int>((_, _, tags, _) => { recordedTags = tags.ToArray(); });

        listener.Start();

        // Act
        counter.Down(new KeyValuePair<string, object?>("tenant", "test"));

        // Assert
        Assert.NotNull(recordedTags);
        Assert.Contains(recordedTags!, t => t is { Key: "tenant", Value: "test" });
    }

    private static MeterListener CreateListener<T>(
        string instrumentName,
        Action<T> callback)
        where T : struct
    {
        var listener = new MeterListener();

        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Name == instrumentName)
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };

        listener.SetMeasurementEventCallback<T>((_, measurement, _, _) => { callback(measurement); });

        listener.Start();

        return listener;
    }
}