using System.Globalization;

namespace Servus.Conversion;

public sealed class IntegerValueConverter : IStringValueConverter
{
    public Type OutputType => typeof(int);
    public object? Convert(string value) => int.Parse(value, CultureInfo.InvariantCulture);
}