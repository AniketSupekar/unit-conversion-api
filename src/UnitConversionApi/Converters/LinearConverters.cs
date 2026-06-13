namespace UnitConversionApi.Converters;

public class LengthConverter : LinearUnitConverter
{
    public override string Category => "length";
}

public class WeightConverter : LinearUnitConverter
{
    public override string Category => "weight";
}

public class AreaConverter : LinearUnitConverter
{
    public override string Category => "area";
}

public class VolumeConverter : LinearUnitConverter
{
    public override string Category => "volume";
}

public class SpeedConverter : LinearUnitConverter
{
    public override string Category => "speed";
}