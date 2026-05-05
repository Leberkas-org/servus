using System.Globalization;

namespace Servus.Conversion;

public sealed class FloatValueConverter : IStringValueConverter
{
    public Type OutputType => typeof(float);
    public object? Convert(string value) => float.Parse(value, CultureInfo.InvariantCulture);
}