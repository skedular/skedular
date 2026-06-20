using Api.Shared.Services.Offering;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public class HostPricingCatalogProvider
{
    public static ProductOffering GetHostOffering()
    {
        var hostStandardOffering = OfferingCode.HostStandardV1.GetOffering();

        return new ProductOffering(
            PricingCatalogProductOfferingCode.Host,
            "Skedular Host",
            "Individual space and resource rental for hosts listing their venues on the marketplace.",
            PricingCatalogVisibility.Public,
            [
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.PayAsYouGo,
                    "Host Standard",
                    "List your venue for free. Pay 5% commission per successful booking.",
                    PricingCatalogCommercialModel.UsageBased,
                    [
                        new PlanFeature("booking", "Full-place booking", 1),
                        new PlanFeature("commission", "5% commission per booking", 2),
                        new PlanFeature("map-visibility", "Public map listing", 3)
                    ],
                    [
                        new PlanLimit(
                            "locations",
                            "Locations",
                            hostStandardOffering.MaxLocationCount,
                            !hostStandardOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            hostStandardOffering.MaxResourceCount,
                            !hostStandardOffering.MaxResourceCount.HasValue)
                    ],
                    [],
                    [],
                    PricingCatalogPlanAvailability.SelfService,
                    true,
                    1)
            ]);
    }
}
