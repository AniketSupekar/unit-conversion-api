using UnitConversionApi.Converters;
using UnitConversionApi.Middleware;
using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var apiError = new UnitConversionApi.Models.ApiError
            {
                Code    = "VALIDATION_ERROR",
                Message = "One or more validation errors occurred.",
                ValidationErrors = errors
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(apiError);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Unit Conversion API",
        Version     = "v1",
        Description = "Converts numerical values between different units of measurement. " +
                      "Supports length, temperature, weight, area, volume, and speed."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<IUnitConverter, LengthConverter>();
builder.Services.AddSingleton<IUnitConverter, WeightConverter>();
builder.Services.AddSingleton<IUnitConverter, TemperatureConverter>();
builder.Services.AddSingleton<IUnitConverter, AreaConverter>();
builder.Services.AddSingleton<IUnitConverter, VolumeConverter>();
builder.Services.AddSingleton<IUnitConverter, SpeedConverter>();

builder.Services.AddScoped<IConversionService, ConversionService>();

builder.Services.AddTransient<GlobalExceptionHandler>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }