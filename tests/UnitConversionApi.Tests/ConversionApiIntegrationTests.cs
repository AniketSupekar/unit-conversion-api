using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Models;
using Xunit;

namespace UnitConversionApi.Tests;

public class ConversionApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConversionApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostConvert_ValidRequest_Returns200WithResult()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "celsius",
            ToUnit   = "fahrenheit"
        };

        var response = await _client.PostAsJsonAsync("/api/convert", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        result!.Result.Should().BeApproximately(212, 0.001);
        result.Category.Should().Be("temperature");
    }

    [Fact]
    public async Task PostConvert_UnsupportedUnit_Returns400WithErrorCode()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "lightyear",  // not supported
            ToUnit   = "meter"
        };

        var response = await _client.PostAsJsonAsync("/api/convert", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ApiError>();
        error!.Code.Should().Be("UNSUPPORTED_UNIT");
    }

    [Fact]
    public async Task PostConvert_IncompatibleUnits_Returns400WithErrorCode()
    {
        var request = new ConversionRequest
        {
            Value    = 100,
            FromUnit = "meter",
            ToUnit   = "kilogram"
        };

        var response = await _client.PostAsJsonAsync("/api/convert", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ApiError>();
        error!.Code.Should().Be("INCOMPATIBLE_UNITS");
    }

    [Fact]
    public async Task PostConvert_MissingRequiredFields_Returns400WithValidationErrors()
    {
        var incompleteRequest = new { Value = 100 };  // missing FromUnit and ToUnit

        var response = await _client.PostAsJsonAsync("/api/convert", incompleteRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ApiError>();
        error!.Code.Should().Be("VALIDATION_ERROR");
        error.ValidationErrors.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUnits_Returns200WithAllCategories()
    {
        var response = await _client.GetAsync("/api/units");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var units = await response.Content
            .ReadFromJsonAsync<Dictionary<string, IEnumerable<string>>>();

        units.Should().ContainKey("temperature");
        units.Should().ContainKey("length");
        units.Should().ContainKey("weight");
    }

    [Fact]
    public async Task PostConvert_LengthConversion_CorrectResult()
    {
        var request = new ConversionRequest
        {
            Value    = 1,
            FromUnit = "mile",
            ToUnit   = "kilometer"
        };

        var response = await _client.PostAsJsonAsync("/api/convert", request);
        var result   = await response.Content.ReadFromJsonAsync<ConversionResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Result.Should().BeApproximately(1.60934, 0.0001);
    }
}