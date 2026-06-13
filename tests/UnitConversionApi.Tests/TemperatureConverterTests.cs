using FluentAssertions;
using UnitConversionApi.Converters;
using Xunit;

namespace UnitConversionApi.Tests;

public class TemperatureConverterTests
{
    private readonly TemperatureConverter _converter = new();

    [Theory]
    [InlineData(0,    "celsius",    32,    "fahrenheit")]
    [InlineData(100,  "celsius",    212,   "fahrenheit")]
    [InlineData(-40,  "celsius",    -40,   "fahrenheit")]  
    [InlineData(0,    "celsius",    273.15,"kelvin")]
    [InlineData(100,  "celsius",    373.15,"kelvin")]
    [InlineData(32,   "fahrenheit", 0,     "celsius")]
    [InlineData(212,  "fahrenheit", 100,   "celsius")]
    [InlineData(273.15,"kelvin",    0,     "celsius")]
    [InlineData(0,    "kelvin",     -273.15,"celsius")]
    public void Convert_KnownValues_ReturnsCorrectResult(
        double value, string fromUnit, double expected, string toUnit)
    {
        var result = _converter.Convert(value, fromUnit, toUnit);

        result.Should().BeApproximately(expected, precision: 0.001);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        _converter.Convert(100, "celsius", "celsius").Should().Be(100);
        _converter.Convert(300, "kelvin",  "kelvin").Should().Be(300);
    }

    [Fact]
    public void Convert_BelowAbsoluteZeroKelvin_ThrowsArgumentOutOfRangeException()
    {
        var act = () => _converter.Convert(-1, "kelvin", "celsius");
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*absolute zero*");
    }

    [Fact]
    public void Convert_BelowAbsoluteZeroCelsius_ThrowsArgumentOutOfRangeException()
    {
        var act = () => _converter.Convert(-300, "celsius", "fahrenheit");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("celsius",    "fahrenheit", true)]
    [InlineData("kelvin",     "rankine",    true)]
    [InlineData("celsius",    "meters",     false)]
    [InlineData("fahrenheit", "kg",         false)]
    public void CanConvert_ReturnsExpectedResult(string from, string to, bool expected)
    {
        _converter.CanConvert(from, to).Should().Be(expected);
    }
}