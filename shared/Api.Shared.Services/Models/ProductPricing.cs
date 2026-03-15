namespace Api.Shared.Services.Models;

public record ProductPricing(
    string Id,
    int Index,
    ListingMetadata ListingMetadata,
    ProductPricingCadence Cadence,
    decimal Price,
    bool IsTaxInclusive,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    ProductPricingBillingMode BillingMode,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer,
    int NumberOfResourcesToBook)
{
    public static ProductPricing Empty(string id) =>
        new(
            id,
            int.MinValue,
            ListingMetadata.Empty,
            ProductPricingCadence.NotSet,
            int.MinValue,
            false,
            [],
            ProductPricingBillingMode.NotSet,
            null,
            null,
            int.MinValue,
            int.MinValue,
            int.MinValue);
}
