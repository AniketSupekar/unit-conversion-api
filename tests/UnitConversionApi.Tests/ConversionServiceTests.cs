using FluentAssertions;
using UnitConversionApi.Converters;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _service;

    public ConversionServiceTests()
    {
        var converters = new List<IUnitConverter>
        {
            new LengthConverter(),
            new WeightConverter(),
            new TemperatureConverter(),
            new AreaConverter(),
            new VolumeConverter(),
            new SpeedConverter()
        };
        _service = new ConversionService(converters);
    }

    [Fact]
    public void Convert_ValidRequest_ReturnsCorrectResponse()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "celsius",
            ToUnit   = "fahrenheit"
        };

        var result = _service.Convert(request);

        result.Result.Should().BeApproximately(212, 0.001);
        result.Category.Should().Be("temperature");
        result.FromUnit.Should().Be("celsius");
        result.ToUnit.Should().Be("fahrenheit");
    }

    [Fact]
    public void Convert_UnknownFromUnit_ThrowsUnsupportedUnitException()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "parsec",   
            ToUnit   = "meter"
        };

        var act = () => _service.Convert(request);
        act.Should().Throw<UnsupportedUnitException>()
           .WithMessage("*parsec*");
    }

    [Fact]
    public void Convert_UnknownToUnit_ThrowsUnsupportedUnitException()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "meter",
            ToUnit   = "furlong"   
        };

        var act = () => _service.Convert(request);
        act.Should().Throw<UnsupportedUnitException>();
    }

    [Fact]
    public void Convert_IncompatibleCategories_ThrowsIncompatibleUnitsException()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "meter",
            ToUnit   = "kilogram" 
        };

        var act = () => _service.Convert(request);
        act.Should().Throw<IncompatibleUnitsException>()
           .WithMessage("*length*weight*");
    }

    [Fact]
    public void Convert_UnitNamesAreCaseInsensitive()
    {
        var request = new ConversionRequest
        {
            Value    = 0,
            FromUnit = "Celsius",     
            ToUnit   = "FAHRENHEIT"   
        };

        var result = _service.Convert(request);
        result.Result.Should().BeApproximately(32, 0.001);
    }

    [Fact]
    public void GetSupportedUnits_ReturnsAllCategories()
    {
        var units = _service.GetSupportedUnits();

        units.Should().ContainKey("temperature");
        units.Should().ContainKey("length");
        units.Should().ContainKey("weight");
        units.Should().ContainKey("area");
        units.Should().ContainKey("volume");
        units.Should().ContainKey("speed");
    }
}