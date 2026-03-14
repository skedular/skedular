namespace Api.Shared.Services.Models;

public enum ProductPricingCadence
{
    NotSet = 0,
    OneTimeV1 = 1,
    PerMinuteV1 = 2,
    PerHourV1 = 3,
    DailyV1 = 4,
    WeeklyV1 = 5,
    MonthlyV1 = 6,
    HalfDayV1 = 7,
    Per15MinuteV1 = 8,
    Per30MinuteV1 = 9,
    TwoMonthsV1 = 10,
    QuarterlyV1 = 11,
    FourMonthsV1 = 12,
    FiveMonthsV1 = 13,
    SixMonthsV1 = 14,
    YearlyV1 = 15
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
    public const string HalfDayV1 = "HALF_DAY_V1";
    public const string Per15MinuteV1 = "PER_15_MINUTE_V1";
    public const string Per30MinuteV1 = "PER_30_MINUTE_V1";
    public const string TwoMonthsV1 = "TWO_MONTHS_V1";
    public const string QuarterlyV1 = "QUARTERLY_V1";
    public const string FourMonthsV1 = "FOUR_MONTHS_V1";
    public const string FiveMonthsV1 = "FIVE_MONTHS_V1";
    public const string SixMonthsV1 = "SIX_MONTHS_V1";
    public const string YearlyV1 = "YEARLY_V1";
}

public static class ProductPricingCadenceExtensions
{
    extension(ProductPricingCadence src)
    {
        public string ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadence.NotSet => ProductPricingCadenceConstants.NotSet,
                ProductPricingCadence.OneTimeV1 => ProductPricingCadenceConstants.OneTimeV1,
                ProductPricingCadence.PerMinuteV1 => ProductPricingCadenceConstants.PerMinuteV1,
                ProductPricingCadence.PerHourV1 => ProductPricingCadenceConstants.PerHourV1,
                ProductPricingCadence.DailyV1 => ProductPricingCadenceConstants.DailyV1,
                ProductPricingCadence.WeeklyV1 => ProductPricingCadenceConstants.WeeklyV1,
                ProductPricingCadence.MonthlyV1 => ProductPricingCadenceConstants.MonthlyV1,
                ProductPricingCadence.HalfDayV1 => ProductPricingCadenceConstants.HalfDayV1,
                ProductPricingCadence.Per15MinuteV1 => ProductPricingCadenceConstants.Per15MinuteV1,
                ProductPricingCadence.Per30MinuteV1 => ProductPricingCadenceConstants.Per30MinuteV1,
                ProductPricingCadence.TwoMonthsV1 => ProductPricingCadenceConstants.TwoMonthsV1,
                ProductPricingCadence.QuarterlyV1 => ProductPricingCadenceConstants.QuarterlyV1,
                ProductPricingCadence.FourMonthsV1 => ProductPricingCadenceConstants.FourMonthsV1,
                ProductPricingCadence.FiveMonthsV1 => ProductPricingCadenceConstants.FiveMonthsV1,
                ProductPricingCadence.SixMonthsV1 => ProductPricingCadenceConstants.SixMonthsV1,
                ProductPricingCadence.YearlyV1 => ProductPricingCadenceConstants.YearlyV1,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToProductPricingCadenceName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "Not Set",
                ProductPricingCadence.OneTimeV1 => "One Time",
                ProductPricingCadence.PerMinuteV1 => "Per Minute",
                ProductPricingCadence.PerHourV1 => "Per Hour",
                ProductPricingCadence.DailyV1 => "Daily",
                ProductPricingCadence.WeeklyV1 => "Weekly",
                ProductPricingCadence.MonthlyV1 => "Monthly",
                ProductPricingCadence.HalfDayV1 => "Half Day",
                ProductPricingCadence.Per15MinuteV1 => "Per 15 Minutes",
                ProductPricingCadence.Per30MinuteV1 => "Per 30 Minutes",
                ProductPricingCadence.TwoMonthsV1 => "Every 2 Months",
                ProductPricingCadence.QuarterlyV1 => "Quarterly",
                ProductPricingCadence.FourMonthsV1 => "Every 4 Months",
                ProductPricingCadence.FiveMonthsV1 => "Every 5 Months",
                ProductPricingCadence.SixMonthsV1 => "Every 6 Months",
                ProductPricingCadence.YearlyV1 => "Yearly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToInvoicePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "not-set",
                ProductPricingCadence.OneTimeV1 => "one-time",
                ProductPricingCadence.PerMinuteV1 => "p/m",
                ProductPricingCadence.PerHourV1 => "p/h",
                ProductPricingCadence.DailyV1 => "daily",
                ProductPricingCadence.WeeklyV1 => "weekly",
                ProductPricingCadence.MonthlyV1 => "monthly",
                ProductPricingCadence.HalfDayV1 => "half-day",
                ProductPricingCadence.Per15MinuteV1 => "p/15m",
                ProductPricingCadence.Per30MinuteV1 => "p/30m",
                ProductPricingCadence.TwoMonthsV1 => "2-monthly",
                ProductPricingCadence.QuarterlyV1 => "quarterly",
                ProductPricingCadence.FourMonthsV1 => "4-monthly",
                ProductPricingCadence.FiveMonthsV1 => "5-monthly",
                ProductPricingCadence.SixMonthsV1 => "6-monthly",
                ProductPricingCadence.YearlyV1 => "yearly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToStripePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "Not-Set",
                ProductPricingCadence.OneTimeV1 => "One-Time",
                ProductPricingCadence.PerMinuteV1 => "Minute",
                ProductPricingCadence.PerHourV1 => "Hour",
                ProductPricingCadence.DailyV1 => "Daily",
                ProductPricingCadence.WeeklyV1 => "Weekly",
                ProductPricingCadence.MonthlyV1 => "Monthly",
                ProductPricingCadence.HalfDayV1 => "Half-Day",
                ProductPricingCadence.Per15MinuteV1 => "15 Minutes",
                ProductPricingCadence.Per30MinuteV1 => "30 Minutes",
                ProductPricingCadence.TwoMonthsV1 => "2 Months",
                ProductPricingCadence.QuarterlyV1 => "Quarterly",
                ProductPricingCadence.FourMonthsV1 => "4 Months",
                ProductPricingCadence.FiveMonthsV1 => "5 Months",
                ProductPricingCadence.SixMonthsV1 => "6 Months",
                ProductPricingCadence.YearlyV1 => "Yearly",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public string ToProductPricingCadenceName() =>
            src switch
            {
                ProductPricingCadenceConstants.NotSet => "Not Set",
                ProductPricingCadenceConstants.OneTimeV1 => "One Time",
                ProductPricingCadenceConstants.PerMinuteV1 => "Per Minute",
                ProductPricingCadenceConstants.PerHourV1 => "Per Hour",
                ProductPricingCadenceConstants.DailyV1 => "Daily",
                ProductPricingCadenceConstants.WeeklyV1 => "Weekly",
                ProductPricingCadenceConstants.MonthlyV1 => "Monthly",
                ProductPricingCadenceConstants.HalfDayV1 => "Half Day",
                ProductPricingCadenceConstants.Per15MinuteV1 => "Per 15 Minutes",
                ProductPricingCadenceConstants.Per30MinuteV1 => "Per 30 Minutes",
                ProductPricingCadenceConstants.TwoMonthsV1 => "Every 2 Months",
                ProductPricingCadenceConstants.QuarterlyV1 => "Quarterly",
                ProductPricingCadenceConstants.FourMonthsV1 => "Every 4 Months",
                ProductPricingCadenceConstants.FiveMonthsV1 => "Every 5 Months",
                ProductPricingCadenceConstants.SixMonthsV1 => "Every 6 Months",
                ProductPricingCadenceConstants.YearlyV1 => "Yearly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public ProductPricingCadence ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadenceConstants.NotSet => ProductPricingCadence.NotSet,
                ProductPricingCadenceConstants.OneTimeV1 => ProductPricingCadence.OneTimeV1,
                ProductPricingCadenceConstants.PerMinuteV1 => ProductPricingCadence.PerMinuteV1,
                ProductPricingCadenceConstants.PerHourV1 => ProductPricingCadence.PerHourV1,
                ProductPricingCadenceConstants.DailyV1 => ProductPricingCadence.DailyV1,
                ProductPricingCadenceConstants.WeeklyV1 => ProductPricingCadence.WeeklyV1,
                ProductPricingCadenceConstants.MonthlyV1 => ProductPricingCadence.MonthlyV1,
                ProductPricingCadenceConstants.HalfDayV1 => ProductPricingCadence.HalfDayV1,
                ProductPricingCadenceConstants.Per15MinuteV1 => ProductPricingCadence.Per15MinuteV1,
                ProductPricingCadenceConstants.Per30MinuteV1 => ProductPricingCadence.Per30MinuteV1,
                ProductPricingCadenceConstants.TwoMonthsV1 => ProductPricingCadence.TwoMonthsV1,
                ProductPricingCadenceConstants.QuarterlyV1 => ProductPricingCadence.QuarterlyV1,
                ProductPricingCadenceConstants.FourMonthsV1 => ProductPricingCadence.FourMonthsV1,
                ProductPricingCadenceConstants.FiveMonthsV1 => ProductPricingCadence.FiveMonthsV1,
                ProductPricingCadenceConstants.SixMonthsV1 => ProductPricingCadence.SixMonthsV1,
                ProductPricingCadenceConstants.YearlyV1 => ProductPricingCadence.YearlyV1,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
