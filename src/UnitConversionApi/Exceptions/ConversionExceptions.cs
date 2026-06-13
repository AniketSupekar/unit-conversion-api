namespace UnitConversionApi.Exceptions;

public class UnsupportedUnitException : Exception
{
    public string UnitName { get; }

    public UnsupportedUnitException(string unitName)
        : base($"Unit '{unitName}' is not supported.")
    {
        UnitName = unitName;
    }
}

public class IncompatibleUnitsException : Exception
{
    public string FromUnit { get; }
    public string ToUnit { get; }

    public IncompatibleUnitsException(string fromUnit, string toUnit, string fromCategory, string toCategory)
        : base($"Cannot convert from '{fromUnit}' ({fromCategory}) to '{toUnit}' ({toCategory}). Units must belong to the same category.")
    {
        FromUnit = fromUnit;
        ToUnit = toUnit;
    }
}