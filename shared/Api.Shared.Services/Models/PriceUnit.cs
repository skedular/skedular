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
    extension(string src)
    {
        public PriceUnit ToPriceUnit() =>
            src switch
            {
                PriceUnitConstants.PerMinute => PriceUnit.PerMinute,
                PriceUnitConstants.PerHour => PriceUnit.PerHour,
                PriceUnitConstants.PerUse => PriceUnit.PerUse,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToInvoicePriceUnitName() =>
            src switch
            {
                PriceUnitConstants.PerMinute => "p/m",
                PriceUnitConstants.PerHour => "p/h",
                PriceUnitConstants.PerUse => "one-time",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(PriceUnit src)
    {
        public string ToPriceUnit() =>
            src switch
            {
                PriceUnit.PerMinute => PriceUnitConstants.PerMinute,
                PriceUnit.PerHour => PriceUnitConstants.PerHour,
                PriceUnit.PerUse => PriceUnitConstants.PerUse,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToPriceUnitName() =>
            src switch
            {
                PriceUnit.PerMinute => "Per Minute",
                PriceUnit.PerHour => "Per Hour",
                PriceUnit.PerUse => "One-time charge (not based on duration)",
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToStripePriceUnitName() =>
            src switch
            {
                PriceUnit.PerMinute => "Minute",
                PriceUnit.PerHour => "Hour",
                PriceUnit.PerUse => "One-time",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
