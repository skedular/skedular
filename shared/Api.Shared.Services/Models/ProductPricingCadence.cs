namespace Api.Shared.Services.Models;

public enum ProductPricingCadence
{
    NotSet = 0,
    OneTimeV1 = 1,
    PerMinuteV1 = 2,
    PerHourV1 = 3,
    DailyV1 = 4,
    WeeklyV1 = 5,
    MonthlyV1 = 6
}

public static class ProductPricingCadenceConstants
{
    public const string NotSet = "NOT_SET";
    public const string OneTimeV1 = "ONE_TIME_V1";
    public const string PerMinuteV1 = "PER_MINUTE_V1";
    public const string PerHourV1 = "PER_HOUR_V1";
    public const string DailyV1 = "DAILY_V1";
    public const string WeeklyV1 = "WEEKLY_V1";
    public const string MonthlyV1 = "MONTHLY_V1";
}

public static class ProductPricingCadenceExtensions
{
    extension(ProductPricingCadence src)
    {
        public string ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadence.OneTimeV1 => ProductPricingCadenceConstants.OneTimeV1,
                ProductPricingCadence.PerMinuteV1 => ProductPricingCadenceConstants.PerMinuteV1,
                ProductPricingCadence.PerHourV1 => ProductPricingCadenceConstants.PerHourV1,
                ProductPricingCadence.DailyV1 => ProductPricingCadenceConstants.DailyV1,
                ProductPricingCadence.WeeklyV1 => ProductPricingCadenceConstants.WeeklyV1,
                ProductPricingCadence.MonthlyV1 => ProductPricingCadenceConstants.MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToProductPricingCadenceName() =>
            src switch
            {
                ProductPricingCadence.OneTimeV1 => "One Time",
                ProductPricingCadence.PerMinuteV1 => "Per Minute",
                ProductPricingCadence.PerHourV1 => "Per Hour",
                ProductPricingCadence.DailyV1 => "Daily",
                ProductPricingCadence.WeeklyV1 => "Weekly",
                ProductPricingCadence.MonthlyV1 => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToInvoicePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.OneTimeV1 => "one-time",
                ProductPricingCadence.PerMinuteV1 => "p/m",
                ProductPricingCadence.PerHourV1 => "p/h",
                ProductPricingCadence.DailyV1 => "daily",
                ProductPricingCadence.WeeklyV1 => "weekly",
                ProductPricingCadence.MonthlyV1 => "monthly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToStripePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.OneTimeV1 => "One-Time",
                ProductPricingCadence.PerMinuteV1 => "Minute",
                ProductPricingCadence.PerHourV1 => "Hour",
                ProductPricingCadence.DailyV1 => "Daily",
                ProductPricingCadence.WeeklyV1 => "Weekly",
                ProductPricingCadence.MonthlyV1 => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public string ToProductPricingCadenceName() =>
            src switch
            {
                ProductPricingCadenceConstants.OneTimeV1 => "One Time",
                ProductPricingCadenceConstants.PerMinuteV1 => "Per Minute",
                ProductPricingCadenceConstants.PerHourV1 => "Per Hour",
                ProductPricingCadenceConstants.DailyV1 => "Daily",
                ProductPricingCadenceConstants.WeeklyV1 => "Weekly",
                ProductPricingCadenceConstants.MonthlyV1 => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public ProductPricingCadence ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadenceConstants.OneTimeV1 => ProductPricingCadence.OneTimeV1,
                ProductPricingCadenceConstants.PerMinuteV1 => ProductPricingCadence.PerMinuteV1,
                ProductPricingCadenceConstants.PerHourV1 => ProductPricingCadence.PerHourV1,
                ProductPricingCadenceConstants.DailyV1 => ProductPricingCadence.DailyV1,
                ProductPricingCadenceConstants.WeeklyV1 => ProductPricingCadence.WeeklyV1,
                ProductPricingCadenceConstants.MonthlyV1 => ProductPricingCadence.MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
