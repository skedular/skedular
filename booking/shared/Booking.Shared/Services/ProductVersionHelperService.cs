using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;

namespace Booking.Shared.Services;

/// <summary>
///     Service for helping with product version and pricing matching operations.
/// </summary>
public interface IProductVersionHelperService
{
    /// <summary>
    ///     Finds a matching pricing option from a collection based on the provided pricing.
    ///     First tries to match by ID, then by pricing properties.
    /// </summary>
    /// <param name="pricingOptions">The collection of pricing options to search in.</param>
    /// <param name="pricing">The pricing to match against.</param>
    /// <returns>The matching pricing option, or null if not found.</returns>
    ProductPricing? FindMatchingPricing(IReadOnlyList<ProductPricing> pricingOptions, ProductPricing pricing);

    /// <summary>
    ///     Finds a matching Stripe product from a collection based on the provided pricing.
    ///     First tries to match by ID, then by pricing properties.
    /// </summary>
    /// <param name="stripeProducts">The collection of Stripe products to search in.</param>
    /// <param name="pricing">The pricing to match against.</param>
    /// <returns>The matching Stripe product, or null if not found.</returns>
    StripeProduct? FindMatchingPricing(IReadOnlyList<StripeProduct> stripeProducts, ProductPricing pricing);
}

/// <summary>
///     Implementation of the product version helper service.
/// </summary>
public class ProductVersionHelperService : IProductVersionHelperService
{
    /// <summary>
    ///     Finds a matching pricing option from a collection based on the provided pricing.
    ///     First tries to match by ID, then by pricing properties.
    /// </summary>
    /// <param name="pricingOptions">The collection of pricing options to search in.</param>
    /// <param name="pricing">The pricing to match against.</param>
    /// <returns>The matching pricing option, or null if not found.</returns>
    public ProductPricing? FindMatchingPricing(IReadOnlyList<ProductPricing> pricingOptions, ProductPricing pricing) =>
        pricingOptions.FirstOrDefault(item => item.Id == pricing.Id) ??
        pricingOptions.FirstOrDefault(item =>
            item.PurchaseCadence == pricing.PurchaseCadence &&
            item.BookingCadence == pricing.BookingCadence &&
            item.NumberOfResourcesToBook == pricing.NumberOfResourcesToBook &&
            item.BillingMode == pricing.BillingMode);

    /// <summary>
    ///     Finds a matching Stripe product from a collection based on the provided pricing.
    ///     First tries to match by ID, then by pricing properties.
    /// </summary>
    /// <param name="stripeProducts">The collection of Stripe products to search in.</param>
    /// <param name="pricing">The pricing to match against.</param>
    /// <returns>The matching Stripe product, or null if not found.</returns>
    public StripeProduct? FindMatchingPricing(IReadOnlyList<StripeProduct> stripeProducts, ProductPricing pricing) =>
        stripeProducts.FirstOrDefault(item => item.Id == pricing.Id) ??
        stripeProducts.FirstOrDefault(item =>
            item.PricingCadence.ToProductPricingCadence() == pricing.PurchaseCadence &&
            item.BillingMode.ToProductPricingBillingMode() == pricing.BillingMode &&
            item.NumberOfResourcesToBook == pricing.NumberOfResourcesToBook);
}
