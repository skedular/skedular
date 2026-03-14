namespace Api.Shared.Services.Models;

public record ProductPricingBillingSchedule(ProductPricingBillingMode Mode, ProductPricingBillingInterval Interval)
{
    public static ProductPricingBillingSchedule Empty => new(ProductPricingBillingMode.NotSet, ProductPricingBillingInterval.NotSet);
}
