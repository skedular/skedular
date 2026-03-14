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

public static class ProductPricingBillingIntervalExtensions
{
    extension(ProductPricingBillingInterval src)
    {
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
}
