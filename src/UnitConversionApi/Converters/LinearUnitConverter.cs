using UnitConversionApi.Data;

namespace UnitConversionApi.Converters;

/// Base class for all converters that use a linear factor relative to a base unit.
public abstract class LinearUnitConverter : IUnitConverter
{
    public abstract string Category { get; }

    public bool CanConvert(string fromUnit, string toUnit)
    {
        return UnitDefinitions.Units.TryGetValue(fromUnit, out var from)
            && UnitDefinitions.Units.TryGetValue(toUnit, out var to)
            && from.Category.Equals(Category, StringComparison.OrdinalIgnoreCase)
            && to.Category.Equals(Category, StringComparison.OrdinalIgnoreCase);
    }

    public double Convert(double value, string fromUnit, string toUnit)
    {
        var from = UnitDefinitions.Units[fromUnit];
        var to = UnitDefinitions.Units[toUnit];

        return value * (from.Factor / to.Factor);
    }
}
