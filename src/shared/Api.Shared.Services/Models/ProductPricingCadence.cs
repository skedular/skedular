namespace Api.Shared.Services.Models;

public enum ProductPricingCadence
{
    NotSet = 0,
    Daily = 1,
    Weekly = 2,
    Fortnightly = 3,
    Monthly = 4,
    TwoMonths = 5,
    Quarterly = 6,
    FourMonths = 7,
    FiveMonths = 8,
    SixMonths = 9,
    Yearly = 10,
}

public static class ProductPricingCadenceConstants
{
    public const string NotSet = "NOT_SET";
    public const string Daily = "DAILY";
    public const string Weekly = "WEEKLY";
    public const string Fortnightly = "FORTNIGHTLY";
    public const string Monthly = "MONTHLY";
    public const string TwoMonths = "TWO_MONTHS";
    public const string Quarterly = "QUARTERLY";
    public const string FourMonths = "FOUR_MONTHS";
    public const string FiveMonths = "FIVE_MONTHS";
    public const string SixMonths = "SIX_MONTHS";
    public const string Yearly = "YEARLY";
}

public static class ProductPricingCadenceExtensions
{
    extension(ProductPricingCadence src)
    {
        public string ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadence.NotSet => ProductPricingCadenceConstants.NotSet,
                ProductPricingCadence.Daily => ProductPricingCadenceConstants.Daily,
                ProductPricingCadence.Weekly => ProductPricingCadenceConstants.Weekly,
                ProductPricingCadence.Monthly => ProductPricingCadenceConstants.Monthly,
                ProductPricingCadence.TwoMonths => ProductPricingCadenceConstants.TwoMonths,
                ProductPricingCadence.Quarterly => ProductPricingCadenceConstants.Quarterly,
                ProductPricingCadence.FourMonths => ProductPricingCadenceConstants.FourMonths,
                ProductPricingCadence.FiveMonths => ProductPricingCadenceConstants.FiveMonths,
                ProductPricingCadence.SixMonths => ProductPricingCadenceConstants.SixMonths,
                ProductPricingCadence.Yearly => ProductPricingCadenceConstants.Yearly,
                ProductPricingCadence.Fortnightly => ProductPricingCadenceConstants.Fortnightly,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToProductPricingCadenceName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "Not Set",
                ProductPricingCadence.Daily => "Daily",
                ProductPricingCadence.Weekly => "Weekly",
                ProductPricingCadence.Monthly => "Monthly",
                ProductPricingCadence.TwoMonths => "2-Monthly",
                ProductPricingCadence.Quarterly => "Quarterly",
                ProductPricingCadence.FourMonths => "4-Monthly",
                ProductPricingCadence.FiveMonths => "5-Monthly",
                ProductPricingCadence.SixMonths => "6-Monthly",
                ProductPricingCadence.Yearly => "Yearly",
                ProductPricingCadence.Fortnightly => "Fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToInvoicePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "not-set",
                ProductPricingCadence.Daily => "daily",
                ProductPricingCadence.Weekly => "weekly",
                ProductPricingCadence.Monthly => "monthly",
                ProductPricingCadence.TwoMonths => "2-monthly",
                ProductPricingCadence.Quarterly => "quarterly",
                ProductPricingCadence.FourMonths => "4-monthly",
                ProductPricingCadence.FiveMonths => "5-monthly",
                ProductPricingCadence.SixMonths => "6-monthly",
                ProductPricingCadence.Yearly => "yearly",
                ProductPricingCadence.Fortnightly => "fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToStripePriceUnitName() =>
            src switch
            {
                ProductPricingCadence.NotSet => "Not-Set",
                ProductPricingCadence.Daily => "Daily",
                ProductPricingCadence.Weekly => "Weekly",
                ProductPricingCadence.Monthly => "Monthly",
                ProductPricingCadence.TwoMonths => "2-Monthly",
                ProductPricingCadence.Quarterly => "Quarterly",
                ProductPricingCadence.FourMonths => "4-Monthly",
                ProductPricingCadence.FiveMonths => "5-Monthly",
                ProductPricingCadence.SixMonths => "6-Monthly",
                ProductPricingCadence.Yearly => "Yearly",
                ProductPricingCadence.Fortnightly => "Fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }

    extension(string src)
    {
        public ProductPricingCadence ToProductPricingCadence() =>
            src switch
            {
                ProductPricingCadenceConstants.NotSet => ProductPricingCadence.NotSet,
                ProductPricingCadenceConstants.Daily => ProductPricingCadence.Daily,
                ProductPricingCadenceConstants.Weekly => ProductPricingCadence.Weekly,
                ProductPricingCadenceConstants.Fortnightly => ProductPricingCadence.Fortnightly,
                ProductPricingCadenceConstants.Monthly => ProductPricingCadence.Monthly,
                ProductPricingCadenceConstants.TwoMonths => ProductPricingCadence.TwoMonths,
                ProductPricingCadenceConstants.Quarterly => ProductPricingCadence.Quarterly,
                ProductPricingCadenceConstants.FourMonths => ProductPricingCadence.FourMonths,
                ProductPricingCadenceConstants.FiveMonths => ProductPricingCadence.FiveMonths,
                ProductPricingCadenceConstants.SixMonths => ProductPricingCadence.SixMonths,
                ProductPricingCadenceConstants.Yearly => ProductPricingCadence.Yearly,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }
}
