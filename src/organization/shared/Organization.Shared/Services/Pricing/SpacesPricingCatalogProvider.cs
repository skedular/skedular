using Api.Shared.Services.Offering;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public class SpacesPricingCatalogProvider
{
    public static ProductOffering GetSpacesOffering()
    {
        var freeTierOffering = OfferingCode.SpacesFreeTierV1.GetOffering();
        var spaceGrowOffering = OfferingCode.SpacesGrowthV1.GetOffering();
        var spaceBusinessOffering = OfferingCode.SpacesBusinessV1.GetOffering();
        var spacesContactUsOffering = OfferingCode.SpacesContactUsV1.GetOffering();

        return new ProductOffering(
            PricingCatalogProductOfferingCode.Spaces,
            "Skedular Spaces",
            "Framework-level pricing representation for workspace operators and flexible workspace providers.",
            PricingCatalogVisibility.Public,
            [
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.Free,
                    "14-day free trial",
                    "Try all current Free Spaces capabilities for 14 days. Upgrade after the trial to continue using Spaces and accepting bookings.",
                    PricingCatalogCommercialModel.Free,
                    [
                        new PlanFeature("booking", "Workspace booking", 1),
                        new PlanFeature("trial-period", "14-day free trial", 2),
                        new PlanFeature("monthly-quota", GetBookingInstanceFeatureName(freeTierOffering), 3),
                    ],
                    [
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances per month",
                            freeTierOffering.MaxBookingInstanceCount,
                            !freeTierOffering.MaxBookingInstanceCount.HasValue),
                        new PlanLimit(
                            "locations",
                            "Locations",
                            freeTierOffering.MaxLocationCount,
                            !freeTierOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            freeTierOffering.MaxResourceCount,
                            !freeTierOffering.MaxResourceCount.HasValue),
                    ],
                    // ReSharper disable once PossibleLossOfFraction
                    [new PlanPrice(freeTierOffering.Currency, (freeTierOffering.FixedPrice ?? 0) / 100, "month", false)],
                    [],
                    PricingCatalogPlanAvailability.SelfService,
                    false,
                    1),
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.Growth,
                    "Growth",
                    "Usage-based Spaces pricing for organizations that need more booking capacity.",
                    PricingCatalogCommercialModel.UsageBased,
                    [
                        new PlanFeature("booking", "Workspace booking", 1),
                        new PlanFeature("monthly-quota", GetBookingInstanceFeatureName(spaceGrowOffering), 2),
                        new PlanFeature("upgrade-path", "Server-driven upgrade prompts", 3),
                    ],
                    [
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances per month",
                            spaceGrowOffering.MaxBookingInstanceCount,
                            !spaceGrowOffering.MaxBookingInstanceCount.HasValue),
                        new PlanLimit(
                            "locations",
                            "Locations",
                            spaceGrowOffering.MaxLocationCount,
                            !spaceGrowOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            spaceGrowOffering.MaxResourceCount,
                            !spaceGrowOffering.MaxResourceCount.HasValue),
                    ],
                    // ReSharper disable once PossibleLossOfFraction
                    [new PlanPrice(spaceGrowOffering.Currency, (spaceGrowOffering.FixedPrice ?? 0) / 100, "month", false)],
                    [],
                    PricingCatalogPlanAvailability.SelfService,
                    true,
                    2),
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.Business,
                    "Business",
                    "Business Spaces pricing for organizations with higher booking volume.",
                    PricingCatalogCommercialModel.UsageBased,
                    [
                        new PlanFeature("booking", "Workspace booking", 1),
                        new PlanFeature("monthly-quota", GetBookingInstanceFeatureName(spaceBusinessOffering), 2),
                        new PlanFeature("support", "Priority support", 3),
                    ],
                    [
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances per month",
                            spaceBusinessOffering.MaxBookingInstanceCount,
                            !spaceBusinessOffering.MaxBookingInstanceCount.HasValue),
                        new PlanLimit(
                            "locations",
                            "Locations",
                            spaceBusinessOffering.MaxLocationCount,
                            !spaceBusinessOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            spaceBusinessOffering.MaxResourceCount,
                            !spaceBusinessOffering.MaxResourceCount.HasValue),
                    ],
                    // ReSharper disable once PossibleLossOfFraction
                    [new PlanPrice(spaceBusinessOffering.Currency, (spaceBusinessOffering.FixedPrice ?? 0) / 100, "month", false)],
                    [],
                    PricingCatalogPlanAvailability.SelfService,
                    false,
                    3),
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.ContactUs,
                    "Contact Us",
                    "Custom Spaces pricing for organizations that negotiate booking-instance capacity with Skedular.",
                    PricingCatalogCommercialModel.CapacityBased,
                    [
                        new PlanFeature("capacity", "Purchased booking-instance capacity", 1),
                        new PlanFeature("procurement", "Procurement-ready billing", 2),
                        new PlanFeature("support", "Priority support", 3),
                    ],
                    [
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances per month",
                            spacesContactUsOffering.MaxBookingInstanceCount,
                            !spacesContactUsOffering.MaxBookingInstanceCount.HasValue),
                        new PlanLimit(
                            "locations",
                            "Locations",
                            spacesContactUsOffering.MaxLocationCount,
                            !spacesContactUsOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            spacesContactUsOffering.MaxResourceCount,
                            !spacesContactUsOffering.MaxResourceCount.HasValue),
                    ],
                    [],
                    [new CapacityOption("spaces-custom", null, "Contact Us", null, PricingCatalogPlanAvailability.ContactUs, 1)],
                    PricingCatalogPlanAvailability.ContactUs,
                    false,
                    4),
            ]);
    }

    private static string GetBookingInstanceFeatureName(Offering offering) =>
        offering.MaxBookingInstanceCount.HasValue
            ? $"{offering.MaxBookingInstanceCount.Value:N0} booking instances per month"
            : "Custom booking instance capacity";
}
