using Api.Shared.Services.Models;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public class TeamsPricingCatalogProvider
{
    public static ProductOffering GetTeamsOffering() =>
        new(
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
                        new PlanFeature("locations", "One location", 3)
                    ],
                    [
                        new PlanLimit("monthly-active-users", "Monthly active users", PricingCatalogConstants.FreeActiveUserLimit, false),
                        new PlanLimit("teams", "Teams", PricingCatalogConstants.FreeTeamLimit, false),
                        new PlanLimit("locations", "Locations", PricingCatalogConstants.FreeLocationLimit, false)
                    ],
                    [new PlanPrice(PricingCatalogConstants.SkedularPricingCurrency.ToCurrency(), 0, "month", false)],
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
                        new PlanFeature("active-user-billing", "Monthly active-user billing", 3)
                    ],
                    [
                        new PlanLimit("monthly-active-users", "Monthly active users", null, true),
                        new PlanLimit("teams", "Teams", null, true),
                        new PlanLimit("locations", "Locations", null, true)
                    ],
                    [new PlanPrice(PricingCatalogConstants.SkedularPricingCurrency.ToCurrency(), 3, "active user/month", false)],
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
                        new PlanFeature("support", "Priority support", 3)
                    ],
                    [
                        new PlanLimit("monthly-active-users", "Monthly active users", null, false),
                        new PlanLimit("teams", "Teams", null, true),
                        new PlanLimit("locations", "Locations", null, true)
                    ],
                    [],
                    [new CapacityOption("teams-custom", null, "Contact Us", null, PricingCatalogPlanAvailability.ContactUs, 1)],
                    PricingCatalogPlanAvailability.ContactUs,
                    false,
                    3)
            ]);
}
