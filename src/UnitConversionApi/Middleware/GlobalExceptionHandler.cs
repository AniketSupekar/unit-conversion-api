using System.Net;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;

namespace UnitConversionApi.Middleware;

/// Catches all unhandled exceptions and maps them to consistent HTTP error responses.

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for request {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            UnsupportedUnitException e => (
                HttpStatusCode.BadRequest,
                new ApiError
                {
                    Code    = "UNSUPPORTED_UNIT",
                    Message = e.Message
                }
            ),
            IncompatibleUnitsException e => (
                HttpStatusCode.BadRequest,
                new ApiError
                {
                    Code    = "INCOMPATIBLE_UNITS",
                    Message = e.Message
                }
            ),
            ArgumentOutOfRangeException e => (
                HttpStatusCode.BadRequest,
                new ApiError
                {
                    Code    = "INVALID_VALUE",
                    Message = e.Message
                }
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ApiError
                {
                    Code    = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred."
                }
            )
        };

        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(error);
    }
}