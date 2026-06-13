using System.ComponentModel.DataAnnotations;

namespace UnitConversionApi.Models;

public class ConversionRequest
{
    [Required]
    public double Value { get; set; }

    [Required]
    [StringLength(50)]
    public string FromUnit { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ToUnit { get; set; } = string.Empty;
}
