using Api.Shared.Services.Offering;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public class TeamsPricingCatalogProvider
{
    public static ProductOffering GetTeamsOffering()
    {
        var freeTierOffering = OfferingCode.FreeTierV1.GetOffering();
        var payAsYouGoOffering = OfferingCode.PayAsYouGoV1.GetOffering();
        var enterpriseCustomOffering = OfferingCode.EnterpriseCustomV1.GetOffering();

        return new ProductOffering(
            PricingCatalogProductOfferingCode.Teams,
            "Skedular Teams",
            "Private workplace management for employees, workplace teams, and shared resources.",
            PricingCatalogVisibility.Public,
            [
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.Free,
                    "Free",
                    "Start with core workplace booking for a small organization.",
                    PricingCatalogCommercialModel.Free,
                    [
                        new PlanFeature("booking", "Desk, room, and resource booking", 1),
                        new PlanFeature("teams", "One team", 2),
                        new PlanFeature("locations", "One location", 3),
                    ],
                    [
                        new PlanLimit(
                            "monthly-active-users",
                            "Monthly active users",
                            freeTierOffering.MaxUserCount,
                            !freeTierOffering.MaxUserCount.HasValue),
                        new PlanLimit(
                            "teams",
                            "Teams",
                            freeTierOffering.MaxTeamCount,
                            !freeTierOffering.MaxTeamCount.HasValue),
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
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances",
                            freeTierOffering.MaxBookingInstanceCount,
                            !freeTierOffering.MaxBookingInstanceCount.HasValue),
                    ],
                    // ReSharper disable once PossibleLossOfFraction
                    [new PlanPrice(freeTierOffering.Currency, (freeTierOffering.UnitPrice ?? 0) / 100, "month", false)],
                    [],
                    PricingCatalogPlanAvailability.SelfService,
                    false,
                    1),
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.PayAsYouGo,
                    "Pay As You Go",
                    "Usage-based Teams pricing for organizations that want unlimited teams and locations.",
                    PricingCatalogCommercialModel.UsageBased,
                    [
                        new PlanFeature("unlimited-teams", "Unlimited teams", 1),
                        new PlanFeature("unlimited-locations", "Unlimited locations", 2),
                        new PlanFeature("active-user-billing", "Monthly active-user billing", 3),
                    ],
                    [
                        new PlanLimit(
                            "monthly-active-users",
                            "Monthly active users",
                            payAsYouGoOffering.MaxUserCount,
                            !payAsYouGoOffering.MaxUserCount.HasValue),
                        new PlanLimit(
                            "teams",
                            "Teams",
                            payAsYouGoOffering.MaxTeamCount,
                            !payAsYouGoOffering.MaxTeamCount.HasValue),
                        new PlanLimit(
                            "locations",
                            "Locations",
                            payAsYouGoOffering.MaxLocationCount,
                            !payAsYouGoOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            payAsYouGoOffering.MaxResourceCount,
                            !payAsYouGoOffering.MaxResourceCount.HasValue),
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances",
                            payAsYouGoOffering.MaxBookingInstanceCount,
                            !payAsYouGoOffering.MaxBookingInstanceCount.HasValue),
                    ],
                    // ReSharper disable once PossibleLossOfFraction
                    [new PlanPrice(payAsYouGoOffering.Currency, (payAsYouGoOffering.UnitPrice ?? 0) / 100, "active user/month", false)],
                    [],
                    PricingCatalogPlanAvailability.SelfService,
                    true,
                    2),
                new SubscriptionPlan(
                    PricingCatalogSubscriptionPlanCode.EnterpriseCapacity,
                    "Enterprise Capacity",
                    "Custom Teams pricing for organizations that negotiate a monthly active-user cap with Skedular.",
                    PricingCatalogCommercialModel.CapacityBased,
                    [
                        new PlanFeature("capacity", "Purchased active-user capacity", 1),
                        new PlanFeature("procurement", "Procurement-ready billing", 2),
                        new PlanFeature("support", "Priority support", 3),
                    ],
                    [
                        new PlanLimit(
                            "monthly-active-users",
                            "Monthly active users",
                            enterpriseCustomOffering.MaxUserCount,
                            false),
                        new PlanLimit(
                            "teams",
                            "Teams",
                            enterpriseCustomOffering.MaxTeamCount,
                            !enterpriseCustomOffering.MaxTeamCount.HasValue),
                        new PlanLimit(
                            "locations",
                            "Locations",
                            enterpriseCustomOffering.MaxLocationCount,
                            !enterpriseCustomOffering.MaxLocationCount.HasValue),
                        new PlanLimit(
                            "resources",
                            "Resources",
                            enterpriseCustomOffering.MaxResourceCount,
                            !enterpriseCustomOffering.MaxResourceCount.HasValue),
                        new PlanLimit(
                            "booking-instances",
                            "Booking instances",
                            enterpriseCustomOffering.MaxBookingInstanceCount,
                            !enterpriseCustomOffering.MaxBookingInstanceCount.HasValue),
                    ],
                    [],
                    [new CapacityOption("teams-custom", null, "Contact Us", null, PricingCatalogPlanAvailability.ContactUs, 1)],
                    PricingCatalogPlanAvailability.ContactUs,
                    false,
                    3),
            ]);
    }
}
