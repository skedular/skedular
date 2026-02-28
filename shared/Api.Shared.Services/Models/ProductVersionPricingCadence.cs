namespace Api.Shared.Services.Models;

public enum ProductVersionPricingCadence
{
    OneTimeV1 = 0,
    PerMinuteV1 = 1,
    DailyV1 = 2,
    WeeklyV1 = 3,
    MonthlyV1 = 4
}

public static class ProductVersionPricingCadenceConstants
{
    public const string OneTimeV1 = "ONE_TIME_V1";
    public const string PerMinuteV1 = "PER_MINUTE_V1";
    public const string DailyV1 = "DAILY_V1";
    public const string WeeklyV1 = "WEEKLY_V1";
    public const string MonthlyV1 = "MONTHLY_V1";
}

public static class ProductVersionPricingCadenceExtensions
{
    extension(ProductVersionPricingCadence src)
    {
        public string ToProductVersionPricingCadence() =>
            src switch
            {
                ProductVersionPricingCadence.OneTimeV1 => ProductVersionPricingCadenceConstants.OneTimeV1,
                ProductVersionPricingCadence.PerMinuteV1 => ProductVersionPricingCadenceConstants.PerMinuteV1,
                ProductVersionPricingCadence.DailyV1 => ProductVersionPricingCadenceConstants.DailyV1,
                ProductVersionPricingCadence.WeeklyV1 => ProductVersionPricingCadenceConstants.WeeklyV1,
                ProductVersionPricingCadence.MonthlyV1 => ProductVersionPricingCadenceConstants.MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToProductVersionPricingCadenceName() =>
            src switch
            {
                ProductVersionPricingCadence.OneTimeV1 => "One Time",
                ProductVersionPricingCadence.PerMinuteV1 => "Per Minute",
                ProductVersionPricingCadence.DailyV1 => "Daily",
                ProductVersionPricingCadence.WeeklyV1 => "Daily",
                ProductVersionPricingCadence.MonthlyV1 => "Daily",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public string ToProductVersionPricingCadenceName() =>
            src switch
            {
                ProductVersionPricingCadenceConstants.OneTimeV1 => "One Time",
                ProductVersionPricingCadenceConstants.PerMinuteV1 => "Per Minute",
                ProductVersionPricingCadenceConstants.DailyV1 => "Daily",
                ProductVersionPricingCadenceConstants.WeeklyV1 => "Weekly",
                ProductVersionPricingCadenceConstants.MonthlyV1 => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public ProductVersionPricingCadence ToProductVersionPricingCadence() =>
            src switch
            {
                ProductVersionPricingCadenceConstants.OneTimeV1 => ProductVersionPricingCadence.OneTimeV1,
                ProductVersionPricingCadenceConstants.PerMinuteV1 => ProductVersionPricingCadence.PerMinuteV1,
                ProductVersionPricingCadenceConstants.DailyV1 => ProductVersionPricingCadence.DailyV1,
                ProductVersionPricingCadenceConstants.WeeklyV1 => ProductVersionPricingCadence.WeeklyV1,
                ProductVersionPricingCadenceConstants.MonthlyV1 => ProductVersionPricingCadence.MonthlyV1,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
