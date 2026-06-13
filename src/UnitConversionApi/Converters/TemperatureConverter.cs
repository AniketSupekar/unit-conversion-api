using UnitConversionApi.Data;

namespace UnitConversionApi.Converters;

/// Temperature requires special handling because conversions involve both degree and farenheit.
public class TemperatureConverter : IUnitConverter
{
    public string Category => "temperature";

    public bool CanConvert(string fromUnit, string toUnit)
    {
        return UnitDefinitions.Units.TryGetValue(fromUnit, out var from)
            && UnitDefinitions.Units.TryGetValue(toUnit, out var to)
            && from.Category == "temperature"
            && to.Category == "temperature";
    }

    public double Convert(double value, string fromUnit, string toUnit)
    {
        if (fromUnit.Equals(toUnit, StringComparison.OrdinalIgnoreCase))
            return value;

        // Validate Kelvin and Rankine can't go below absolute zero
        ValidateAbsoluteZero(value, fromUnit);

        double celsius = ToCelsius(value, fromUnit);
        return FromCelsius(celsius, toUnit);
    }

    private static double ToCelsius(double value, string unit) =>
        unit.ToLower() switch
        {
            "celsius" or "c"    => value,
            "fahrenheit" or "f" => (value - 32) * 5 / 9,
            "kelvin" or "k"     => value - 273.15,
            "rankine" or "r"    => (value - 491.67) * 5 / 9,
            _ => throw new ArgumentException($"Unknown temperature unit: {unit}")
        };

    private static double FromCelsius(double celsius, string unit) =>
        unit.ToLower() switch
        {
            "celsius" or "c"    => celsius,
            "fahrenheit" or "f" => (celsius * 9 / 5) + 32,
            "kelvin" or "k"     => celsius + 273.15,
            "rankine" or "r"    => (celsius + 273.15) * 9 / 5,
            _ => throw new ArgumentException($"Unknown temperature unit: {unit}")
        };

    private static void ValidateAbsoluteZero(double value, string unit)
    {
        bool isBelowAbsoluteZero = unit.ToLower() switch
        {
            "kelvin" or "k"   => value < 0,
            "rankine" or "r"  => value < 0,
            "celsius" or "c"  => value < -273.15,
            "fahrenheit" or "f" => value < -459.67,
            _ => false
        };

        if (isBelowAbsoluteZero)
            throw new ArgumentOutOfRangeException(nameof(value),
                $"Value {value} {unit} is below absolute zero. This is physically impossible.");
    }
}
