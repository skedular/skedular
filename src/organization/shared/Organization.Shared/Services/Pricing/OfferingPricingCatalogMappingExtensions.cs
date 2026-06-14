using Api.Shared.Services.Offering;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public static class OfferingPricingCatalogMappingExtensions
{
    extension(OfferingCode offeringCode)
    {
        public PricingCatalogSubscriptionPlanCode ToPricingCatalogSubscriptionPlanCode() =>
            offeringCode switch
            {
                OfferingCode.EarlyBirdV1 => PricingCatalogSubscriptionPlanCode.LegacyEarlyBird,
                OfferingCode.FreeTierV1 => PricingCatalogSubscriptionPlanCode.Free,
                OfferingCode.PayAsYouGoV1 => PricingCatalogSubscriptionPlanCode.PayAsYouGo,
                OfferingCode.EnterpriseCustomV1 => PricingCatalogSubscriptionPlanCode.EnterpriseCapacity,
                _ => throw new ArgumentOutOfRangeException(nameof(offeringCode), offeringCode, null)
            };
    }
}
