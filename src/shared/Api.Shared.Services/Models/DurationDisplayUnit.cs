namespace Api.Shared.Services.Models;

public enum DurationDisplayUnit
{
    Minutes,
    Hours,
}

public static class DurationDisplayUnitConstants
{
    public const string Minutes = "MINUTES";
    public const string Hours = "HOURS";
}

public static class DurationDisplayUnitExtensions
{
    extension(DurationDisplayUnit value)
    {
        public string ToDurationDisplayUnitName() => value switch
        {
            DurationDisplayUnit.Minutes => "Minutes",
            DurationDisplayUnit.Hours => "Hours",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };

        public string ToDurationDisplayUnit() => value switch
        {
            DurationDisplayUnit.Minutes => DurationDisplayUnitConstants.Minutes,
            DurationDisplayUnit.Hours => DurationDisplayUnitConstants.Hours,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }

    public static DurationDisplayUnit ToDurationDisplayUnit(this string? value) => value switch
    {
        null => DurationDisplayUnit.Hours,
        DurationDisplayUnitConstants.Minutes => DurationDisplayUnit.Minutes,
        DurationDisplayUnitConstants.Hours => DurationDisplayUnit.Hours,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported duration display unit."),
    };
}
