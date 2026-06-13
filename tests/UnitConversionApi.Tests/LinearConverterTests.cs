using FluentAssertions;
using UnitConversionApi.Converters;
using Xunit;

namespace UnitConversionApi.Tests;

public class LinearConverterTests
{

    [Theory]
    [InlineData(1,     "kilometer", 1000,        "meter")]
    [InlineData(1,     "mile",      1609.344,    "meter")]
    [InlineData(1,     "foot",      12,          "inch")]
    [InlineData(1,     "meter",     100,         "centimeter")]
    [InlineData(5280,  "feet",      1,           "mile")]
    public void LengthConverter_KnownValues_CorrectResult(
        double value, string from, double expected, string to)
    {
        var converter = new LengthConverter();
        converter.Convert(value, from, to).Should().BeApproximately(expected, 0.001);
    }


    [Theory]
    [InlineData(1,   "kilogram", 1000,       "gram")]
    [InlineData(1,   "pound",    453.592,    "gram")]
    [InlineData(1,   "kilogram", 2.20462,    "pounds")]
    [InlineData(1,   "tonne",    1000,       "kilogram")]
    public void WeightConverter_KnownValues_CorrectResult(
        double value, string from, double expected, string to)
    {
        var converter = new WeightConverter();
        converter.Convert(value, from, to).Should().BeApproximately(expected, 0.001);
    }


    [Theory]
    [InlineData(1,  "hectare",      10000,  "square meter")]
    [InlineData(1,  "square mile",  640,    "acres")]
    public void AreaConverter_KnownValues_CorrectResult(
        double value, string from, double expected, string to)
    {
        var converter = new AreaConverter();
        converter.Convert(value, from, to).Should().BeApproximately(expected, 0.01);
    }


    [Fact]
    public void LengthConverter_CannotConvertWeightUnits()
    {
        var converter = new LengthConverter();
        converter.CanConvert("meter", "kilogram").Should().BeFalse();
    }

    [Fact]
    public void LengthConverter_CanConvertSameUnit_ReturnsOriginalValue()
    {
        var converter = new LengthConverter();
        converter.Convert(42, "meter", "meter").Should().BeApproximately(42, 0.0001);
    }
}