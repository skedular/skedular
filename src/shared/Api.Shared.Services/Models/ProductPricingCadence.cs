namespace Api.Shared.Services.Models;

public enum ProductPricingCadence
{
    NotSet = 0,
    OneTime = 1,
    PerMinute = 2,
    PerHour = 3,
    Daily = 4,
    Weekly = 5,
    Monthly = 6,
    HalfDay = 7,
    Per15Minutes = 8,
    Per30Minutes = 9,
    TwoMonths = 10,
    Quarterly = 11,
    FourMonths = 12,
    FiveMonths = 13,
    SixMonths = 14,
    Yearly = 15,
    Fortnightly = 16
}

public static class ProductPricingCadenceConstants
{
    public const string NotSet = "NOT_SET";
    public const string OneTime = "ONE_TIME_V1";
    public const string PerMinute = "PER_MINUTE";
    public const string PerHour = "PER_HOUR";
    public const string Daily = "DAILY";
    public const string Weekly = "WEEKLY";
    public const string Monthly = "MONTHLY";
    public const string HalfDay = "HALF_DAY";
    public const string Per15Minutes = "PER_15_MINUTES";
    public const string Per30Minutes = "PER_30_MINUTES";
    public const string TwoMonths = "TWO_MONTHS";
    public const string Quarterly = "QUARTERLY";
    public const string FourMonths = "FOUR_MONTHS";
    public const string FiveMonths = "FIVE_MONTHS";
    public const string SixMonths = "SIX_MONTHS";
    public const string Yearly = "YEARLY";
    public const string Fortnightly = "FORTNIGHTLY";
}

public static class ProductPricingCadenceExtensions
{
    extension(ProductPricingCadence src)
    {
        public string ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadence.NotSet => ProductPricingCadenceConstants.NotSet,
                ProductPricingCadence.OneTime => ProductPricingCadenceConstants.OneTime,
                ProductPricingCadence.PerMinute => ProductPricingCadenceConstants.PerMinute,
                ProductPricingCadence.PerHour => ProductPricingCadenceConstants.PerHour,
                ProductPricingCadence.Daily => ProductPricingCadenceConstants.Daily,
                ProductPricingCadence.Weekly => ProductPricingCadenceConstants.Weekly,
                ProductPricingCadence.Monthly => ProductPricingCadenceConstants.Monthly,
                ProductPricingCadence.HalfDay => ProductPricingCadenceConstants.HalfDay,
                ProductPricingCadence.Per15Minutes => ProductPricingCadenceConstants.Per15Minutes,
                ProductPricingCadence.Per30Minutes => ProductPricingCadenceConstants.Per30Minutes,
                ProductPricingCadence.TwoMonths => ProductPricingCadenceConstants.TwoMonths,
                ProductPricingCadence.Quarterly => ProductPricingCadenceConstants.Quarterly,
                ProductPricingCadence.FourMonths => ProductPricingCadenceConstants.FourMonths,
                ProductPricingCadence.FiveMonths => ProductPricingCadenceConstants.FiveMonths,
                ProductPricingCadence.SixMonths => ProductPricingCadenceConstants.SixMonths,
                ProductPricingCadence.Yearly => ProductPricingCadenceConstants.Yearly,
                ProductPricingCadence.Fortnightly => ProductPricingCadenceConstants.Fortnightly,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };

        public string ToProductPricingCadenceName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "Not Set",
                ProductPricingCadence.OneTime => "One Time",
                ProductPricingCadence.PerMinute => "Per Minute",
                ProductPricingCadence.PerHour => "Per Hour",
                ProductPricingCadence.Daily => "Daily",
                ProductPricingCadence.Weekly => "Weekly",
                ProductPricingCadence.Monthly => "Monthly",
                ProductPricingCadence.HalfDay => "Half Day",
                ProductPricingCadence.Per15Minutes => "Per 15 Minutes",
                ProductPricingCadence.Per30Minutes => "Per 30 Minutes",
                ProductPricingCadence.TwoMonths => "2-Monthly",
                ProductPricingCadence.Quarterly => "Quarterly",
                ProductPricingCadence.FourMonths => "4-Monthly",
                ProductPricingCadence.FiveMonths => "5-Monthly",
                ProductPricingCadence.SixMonths => "6-Monthly",
                ProductPricingCadence.Yearly => "Yearly",
                ProductPricingCadence.Fortnightly => "Fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };

        public string ToInvoicePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "not-set",
                ProductPricingCadence.OneTime => "one-time",
                ProductPricingCadence.PerMinute => "p/m",
                ProductPricingCadence.PerHour => "p/h",
                ProductPricingCadence.Daily => "daily",
                ProductPricingCadence.Weekly => "weekly",
                ProductPricingCadence.Monthly => "monthly",
                ProductPricingCadence.HalfDay => "half-day",
                ProductPricingCadence.Per15Minutes => "p/15m",
                ProductPricingCadence.Per30Minutes => "p/30m",
                ProductPricingCadence.TwoMonths => "2-monthly",
                ProductPricingCadence.Quarterly => "quarterly",
                ProductPricingCadence.FourMonths => "4-monthly",
                ProductPricingCadence.FiveMonths => "5-monthly",
                ProductPricingCadence.SixMonths => "6-monthly",
                ProductPricingCadence.Yearly => "yearly",
                ProductPricingCadence.Fortnightly => "fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };

        public string ToStripePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "Not-Set",
                ProductPricingCadence.OneTime => "One-Time",
                ProductPricingCadence.PerMinute => "Minute",
                ProductPricingCadence.PerHour => "Hour",
                ProductPricingCadence.Daily => "Daily",
                ProductPricingCadence.Weekly => "Weekly",
                ProductPricingCadence.Monthly => "Monthly",
                ProductPricingCadence.HalfDay => "Half-Day",
                ProductPricingCadence.Per15Minutes => "15-Minutes",
                ProductPricingCadence.Per30Minutes => "30-Minutes",
                ProductPricingCadence.TwoMonths => "2-Monthly",
                ProductPricingCadence.Quarterly => "Quarterly",
                ProductPricingCadence.FourMonths => "4-Monthly",
                ProductPricingCadence.FiveMonths => "5-Monthly",
                ProductPricingCadence.SixMonths => "6-Monthly",
                ProductPricingCadence.Yearly => "Yearly",
                ProductPricingCadence.Fortnightly => "Fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };
    }

    extension(string src)
    {
        public ProductPricingCadence ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadenceConstants.NotSet => ProductPricingCadence.NotSet,
                ProductPricingCadenceConstants.OneTime => ProductPricingCadence.OneTime,
                ProductPricingCadenceConstants.PerMinute => ProductPricingCadence.PerMinute,
                ProductPricingCadenceConstants.PerHour => ProductPricingCadence.PerHour,
                ProductPricingCadenceConstants.Daily => ProductPricingCadence.Daily,
                ProductPricingCadenceConstants.Weekly => ProductPricingCadence.Weekly,
                ProductPricingCadenceConstants.Monthly => ProductPricingCadence.Monthly,
                ProductPricingCadenceConstants.HalfDay => ProductPricingCadence.HalfDay,
                ProductPricingCadenceConstants.Per15Minutes => ProductPricingCadence.Per15Minutes,
                ProductPricingCadenceConstants.Per30Minutes => ProductPricingCadence.Per30Minutes,
                ProductPricingCadenceConstants.TwoMonths => ProductPricingCadence.TwoMonths,
                ProductPricingCadenceConstants.Quarterly => ProductPricingCadence.Quarterly,
                ProductPricingCadenceConstants.FourMonths => ProductPricingCadence.FourMonths,
                ProductPricingCadenceConstants.FiveMonths => ProductPricingCadence.FiveMonths,
                ProductPricingCadenceConstants.SixMonths => ProductPricingCadence.SixMonths,
                ProductPricingCadenceConstants.Yearly => ProductPricingCadence.Yearly,
                ProductPricingCadenceConstants.Fortnightly => ProductPricingCadence.Fortnightly,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };
    }
}
