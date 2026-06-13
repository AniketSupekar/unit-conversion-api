namespace UnitConversionApi.Models;

public class ConversionResponse
{
    public double InputValue { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public double Result { get; set; }
    public string Category { get; set; } = string.Empty;
}
 