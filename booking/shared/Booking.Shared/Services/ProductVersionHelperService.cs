using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;

namespace Booking.Shared.Services;

public interface IProductVersionHelperService
{
    ProductPricing? FindMatchingPricing(ICollection<ProductPricing> pricingOptions, ProductPricing pricing);
    StripeProduct? FindMatchingPricing(ICollection<StripeProduct> stripeProducts, ProductPricing pricing);
}

public class ProductVersionHelperService : IProductVersionHelperService
{
    public ProductPricing? FindMatchingPricing(ICollection<ProductPricing> pricingOptions, ProductPricing pricing) =>
        pricingOptions.FirstOrDefault(item => item.Id == pricing.Id) ??
        pricingOptions.FirstOrDefault(item =>
            item.Cadence == pricing.Cadence &&
            item.NumberOfResourcesToBook == pricing.NumberOfResourcesToBook &&
            item.BillingMode == pricing.BillingMode);

    public StripeProduct? FindMatchingPricing(ICollection<StripeProduct> stripeProducts, ProductPricing pricing) =>
        stripeProducts.FirstOrDefault(item => item.Id == pricing.Id) ??
        stripeProducts.FirstOrDefault(item =>
            item.PricingCadence.ToProductPricingCadence() == pricing.Cadence &&
            item.BillingMode.ToProductPricingBillingMode() == pricing.BillingMode &&
            item.NumberOfResourcesToBook == pricing.NumberOfResourcesToBook);
}
