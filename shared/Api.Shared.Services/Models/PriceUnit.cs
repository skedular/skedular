namespace Api.Shared.Services.Models;

public enum PriceUnit
{
    PerMinute,
    PerHour,
    PerUse
}

public static class PriceUnitConstants
{
    public const string PerMinute = "PER_MINUTE";
    public const string PerHour = "PER_HOUR";
    public const string PerUse = "PER_USE";
}

public static class PriceUnitExtensions
{
    public static PriceUnit ToPriceUnit(this string src) =>
        src switch
        {
            PriceUnitConstants.PerMinute => PriceUnit.PerMinute,
            PriceUnitConstants.PerHour => PriceUnit.PerHour,
            PriceUnitConstants.PerUse => PriceUnit.PerUse,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPriceUnit(this PriceUnit src) =>
        src switch
        {
            PriceUnit.PerMinute => PriceUnitConstants.PerMinute,
            PriceUnit.PerHour => PriceUnitConstants.PerHour,
            PriceUnit.PerUse => PriceUnitConstants.PerUse,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPriceUnitName(this PriceUnit src) =>
        src switch
        {
            PriceUnit.PerMinute => "Per Minute",
            PriceUnit.PerHour => "Per Hour",
            PriceUnit.PerUse => "One-time charge (not based on duration)",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPriceUnitName(this string src) =>
        src switch
        {
            PriceUnitConstants.PerMinute => "Per Minute",
            PriceUnitConstants.PerHour => "Per Hour",
            PriceUnitConstants.PerUse => "One-time charge (not based on duration)",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePriceUnitName(this PriceUnit src) =>
        src switch
        {
            PriceUnit.PerMinute => "Minute",
            PriceUnit.PerHour => "Hour",
            PriceUnit.PerUse => "One-time charge (not based on duration)",
            _ => throw new ArgumentOutOfRangeException()
        };
}
