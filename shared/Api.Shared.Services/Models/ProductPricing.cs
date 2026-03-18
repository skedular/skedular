namespace Api.Shared.Services.Models;

public record ProductPricing(
    string Id,
    int Index,
    ListingMetadata ListingMetadata,
    ProductPricingCadence PurchaseCadence,
    ProductPricingCadence BookingCadence,
    decimal Price,
    bool IsTaxInclusive,
    bool SupportsSubscriptionAutoRenewal,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    ProductPricingBillingMode BillingMode,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer,
    int NumberOfResourcesToBook,
    ProductPricingCancellationPolicyType CancellationPolicyType,
    ICollection<ProductPricingCancellationRefundRule> CancellationRefundRules,
    string TermsAndConditionsUrl)
{
    public static ProductPricing Empty(string id) =>
        new(
            id,
            int.MinValue,
            ListingMetadata.Empty,
            ProductPricingCadence.NotSet,
            ProductPricingCadence.NotSet,
            int.MinValue,
            false,
            false,
            [],
            ProductPricingBillingMode.NotSet,
            null,
            null,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            ProductPricingCancellationPolicyType.NotSet,
            [],
            string.Empty);
}
