using Api.Shared.Services.Offering;
using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("OrganizationSpacesSubscriptionDetails")]
public class OrganizationSpacesSubscriptionDetails
{
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("planCode")] public PricingCatalogSubscriptionPlanCode PlanCode { get; set; }
    [GraphQLName("commercialModel")] public PricingCatalogCommercialModel CommercialModel { get; set; }
    [GraphQLName("currentPeriodStartUtc")] public DateTimeOffset CurrentPeriodStartUtc { get; set; }
    [GraphQLName("currentPeriodEndUtc")] public DateTimeOffset CurrentPeriodEndUtc { get; set; }
    [GraphQLName("usageLimit")] public int? UsageLimit { get; set; }
    [GraphQLName("rolloverDate")] public DateTimeOffset? RolloverDate { get; set; }
    [GraphQLName("customCapacity")] public int? CustomCapacity { get; set; }
    [GraphQLName("catalogVersionCode")] public string CatalogVersionCode { get; set; } = string.Empty;
    [GraphQLName("status")] public OrganizationOfferingPlanStatus Status { get; set; }
    [GraphQLName("subscriptionStatus")] public SpacesSubscriptionStatus SubscriptionStatus { get; set; }
    [GraphQLName("accessReason")] public SpacesAccessReasonCode AccessReason { get; set; }
    [GraphQLName("trialStartedAt")] public DateTimeOffset? TrialStartedAt { get; set; }
    [GraphQLName("trialEndsAt")] public DateTimeOffset? TrialEndsAt { get; set; }
    [GraphQLName("remainingTrialDays")] public int RemainingTrialDays { get; set; }
    [GraphQLName("canUseProduct")] public bool CanUseProduct { get; set; }
    [GraphQLName("canAcceptBookings")] public bool CanAcceptBookings { get; set; }

    [GraphQLName("canProtectExistingCommitments")]
    public bool CanProtectExistingCommitments { get; set; }

    [GraphQLName("upgradeRequired")] public bool UpgradeRequired { get; set; }
    [GraphQLName("isComplimentaryBridge")] public bool IsComplimentaryBridge { get; set; }
    [GraphQLName("nextBillingAt")] public DateTimeOffset? NextBillingAt { get; set; }
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [GraphQLName("updatedAt")] public DateTimeOffset UpdatedAt { get; set; }
}
