using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public class SpacesPricingCatalogProvider
{
    public static ProductOffering GetSpacesOffering() =>
        new(
            PricingCatalogProductOfferingCode.Spaces,
            "Skedular Spaces",
            "Framework-level pricing representation for workspace operators and flexible workspace providers.",
            PricingCatalogVisibility.Public,
            [
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.EnterpriseCapacity,
                    "Spaces Enterprise",
                    "Sales-led Spaces pricing placeholder for future location, resource, and marketplace capacity models.",
                    PricingCatalogCommercialModel.CapacityBased,
                    [
                        new PlanFeature("framework", "Product-aware catalog support", 1),
                        new PlanFeature("sales-led", "Sales-led commercial setup", 2)
                    ],
                    [
                        new PlanLimit("locations", "Locations", null, true),
                        new PlanLimit("resources", "Resources", null, true)
                    ],
                    [],
                    [new CapacityOption("spaces-custom", null, "Contact Us", null, PricingCatalogPlanAvailability.ContactUs, 1)],
                    PricingCatalogPlanAvailability.ContactUs,
                    false,
                    1)
            ]);
}
