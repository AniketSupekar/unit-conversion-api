using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    [HttpPost("convert")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public IActionResult Convert([FromBody] ConversionRequest request)
    {
        var result = _conversionService.Convert(request);
        return Ok(result);
    }

    /// Returns all supported units grouped by category.
    [HttpGet("units")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSupportedUnits()
    {
        var units = _conversionService.GetSupportedUnits();
        return Ok(units);
    }
}