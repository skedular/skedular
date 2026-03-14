namespace Api.Shared.Services.Models;

public enum ProductPricingBillingInterval
{
    NotSet = 0,
    FullTerm = 1,
    PerBooking = 2,
    Weekly = 3,
    Fortnightly = 4,
    Monthly = 5
}

public static class ProductPricingBillingIntervalConstants
{
    public const string NotSet = "NOT_SET";
    public const string FullTerm = "FULL_TERM";
    public const string PerBooking = "PER_BOOKING";
    public const string Weekly = "WEEKLY";
    public const string Fortnightly = "FORTNIGHTLY";
    public const string Monthly = "MONTHLY";
}

public static class ProductPricingBillingIntervalExtensions
{
    extension(ProductPricingBillingInterval src)
    {
        public string ToProductPricingBillingInterval() =>
            src switch
            {
                ProductPricingBillingInterval.NotSet => ProductPricingBillingIntervalConstants.NotSet,
                ProductPricingBillingInterval.FullTerm => ProductPricingBillingIntervalConstants.FullTerm,
                ProductPricingBillingInterval.PerBooking => ProductPricingBillingIntervalConstants.PerBooking,
                ProductPricingBillingInterval.Weekly => ProductPricingBillingIntervalConstants.Weekly,
                ProductPricingBillingInterval.Fortnightly => ProductPricingBillingIntervalConstants.Fortnightly,
                ProductPricingBillingInterval.Monthly => ProductPricingBillingIntervalConstants.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToProductPricingBillingIntervalName() =>
            src switch
            {
                ProductPricingBillingInterval.NotSet => "Not Set",
                ProductPricingBillingInterval.FullTerm => "Full Term",
                ProductPricingBillingInterval.PerBooking => "Per Booking",
                ProductPricingBillingInterval.Weekly => "Weekly",
                ProductPricingBillingInterval.Fortnightly => "Fortnightly",
                ProductPricingBillingInterval.Monthly => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public string ToProductPricingBillingIntervalName() =>
            src switch
            {
                ProductPricingBillingIntervalConstants.NotSet => "Not Set",
                ProductPricingBillingIntervalConstants.FullTerm => "Full Term",
                ProductPricingBillingIntervalConstants.PerBooking => "Per Booking",
                ProductPricingBillingIntervalConstants.Weekly => "Weekly",
                ProductPricingBillingIntervalConstants.Fortnightly => "Fortnightly",
                ProductPricingBillingIntervalConstants.Monthly => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };

        public ProductPricingBillingInterval ToProductPricingBillingInterval() =>
            src switch
            {
                ProductPricingBillingIntervalConstants.NotSet => ProductPricingBillingInterval.NotSet,
                ProductPricingBillingIntervalConstants.FullTerm => ProductPricingBillingInterval.FullTerm,
                ProductPricingBillingIntervalConstants.PerBooking => ProductPricingBillingInterval.PerBooking,
                ProductPricingBillingIntervalConstants.Weekly => ProductPricingBillingInterval.Weekly,
                ProductPricingBillingIntervalConstants.Fortnightly => ProductPricingBillingInterval.Fortnightly,
                ProductPricingBillingIntervalConstants.Monthly => ProductPricingBillingInterval.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
