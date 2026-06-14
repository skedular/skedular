using Api.Shared.Services.Models;

namespace Organization.Shared.Models.PricingCatalog;

public record OrganizationOfferingPlan(
    string Id,
    string OrganizationId,
    PricingCatalogProductOfferingCode ProductOfferingCode,
    PricingCatalogSubscriptionPlanCode PlanCode,
    int? UnitPrice,
    int? FixedPrice,
    Currency Currency,
    int? PurchasedUserCapacity,
    int? PurchasedLocationCapacity,
    int? PurchasedTeamCapacity,
    string CatalogVersionCode,
    OrganizationOfferingPlanStatus Status,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveUntil,
    bool AutoRenew,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
