namespace Api.Shared.Services.Models;

public enum PriceUnit
{
    PerMinute,
    PerHour,
    PerUse,
}

public static class PriceUnitConstants
{
    public const string PerMinute = "PER_MINUTE";
    public const string PerHour = "PER_HOUR";
    public const string PerUse = "PER_USE";
}
