namespace UnitConversionApi.Converters;

public interface IUnitConverter
{
    string Category { get; }
    bool CanConvert(string fromUnit, string toUnit);
    double Convert(double value, string fromUnit, string toUnit);
}