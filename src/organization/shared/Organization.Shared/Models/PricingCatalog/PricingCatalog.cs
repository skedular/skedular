using Api.Shared.Services.Models;

namespace Organization.Shared.Models.PricingCatalog;

public record PricingCatalog(
    string Id,
    PricingCatalogVersion ActiveVersion,
    IReadOnlyList<ProductOffering> ProductOfferings,
    DateTimeOffset GeneratedAt);

public record PricingCatalogVersion(
    string Code,
    PricingCatalogVersionStatus Status,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveUntil,
    string CompatibilityNotes);

public record ProductOffering(
    PricingCatalogProductOfferingCode Code,
    string Name,
    string Description,
    PricingCatalogVisibility Visibility,
    IReadOnlyList<SubscriptionPlan> Plans);

public record SubscriptionPlan(
    PricingCatalogSubscriptionPlanCode Code,
    string Name,
    string Description,
    PricingCatalogCommercialModel CommercialModel,
    IReadOnlyList<PlanFeature> Features,
    IReadOnlyList<PlanLimit> Limits,
    IReadOnlyList<PlanPrice> Prices,
    IReadOnlyList<CapacityOption> CapacityOptions,
    PricingCatalogPlanAvailability Availability,
    bool Recommended,
    int DisplayOrder);

public record CapacityOption(
    string Code,
    int? UserCapacity,
    string Label,
    PlanPrice? Price,
    PricingCatalogPlanAvailability Availability,
    int DisplayOrder);

public record PlanFeature(string Code, string Name, int DisplayOrder);

public record PlanLimit(string Code, string Name, int? Limit, bool Unlimited);

public record PlanPrice(Currency Currency, decimal Amount, string Cadence, bool TaxInclusive);
