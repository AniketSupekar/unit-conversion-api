using UnitConversionApi.Converters;
using UnitConversionApi.Data;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public class ConversionService : IConversionService
{
    private readonly IEnumerable<IUnitConverter> _converters;

    public ConversionService(IEnumerable<IUnitConverter> converters)
    {
        _converters = converters;
    }

    public ConversionResponse Convert(ConversionRequest request)
    {
        var fromUnit = request.FromUnit.Trim().ToLower();
        var toUnit   = request.ToUnit.Trim().ToLower();

        if (!UnitDefinitions.Units.TryGetValue(fromUnit, out var fromInfo))
            throw new UnsupportedUnitException(request.FromUnit);

        if (!UnitDefinitions.Units.TryGetValue(toUnit, out var toInfo))
            throw new UnsupportedUnitException(request.ToUnit);

        if (!fromInfo.Category.Equals(toInfo.Category, StringComparison.OrdinalIgnoreCase))
            throw new IncompatibleUnitsException(fromUnit, toUnit, fromInfo.Category, toInfo.Category);

        var converter = _converters.FirstOrDefault(c =>
            c.Category.Equals(fromInfo.Category, StringComparison.OrdinalIgnoreCase));

        if (converter is null)
            throw new InvalidOperationException(
                $"No converter registered for category '{fromInfo.Category}'. This is a configuration error.");

        double result = converter.Convert(request.Value, fromUnit, toUnit);

        result = Math.Round(result, 10);

        return new ConversionResponse
        {
            InputValue = request.Value,
            FromUnit   = fromUnit,
            ToUnit     = toUnit,
            Result     = result,
            Category   = fromInfo.Category
        };
    }

    public IReadOnlyDictionary<string, IEnumerable<string>> GetSupportedUnits()
    {
        return UnitDefinitions.GetUnitsByCategory();
    }
}