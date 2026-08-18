using Api.Shared.Services.Offering;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models.PricingCatalog;

public class OrganizationSpacesSubscription : ModelBase
{
    public PricingCatalogSubscriptionPlanCode PlanCode { get; set; }
    public PricingCatalogCommercialModel CommercialModel { get; set; }
    public DateTimeOffset CurrentPeriodStart { get; set; }
    public DateTimeOffset CurrentPeriodEnd { get; set; }
    public int? UsageLimit { get; set; }
    public DateTimeOffset? RolloverDate { get; set; }
    public int? CustomCapacity { get; set; }
    public string CatalogVersion { get; set; } = string.Empty;
    public OrganizationOfferingPlanStatus Status { get; set; }
    public SpacesSubscriptionStatus SubscriptionStatus { get; set; }
    public SpacesAccessReasonCode AccessReason { get; set; }
    public DateTimeOffset? TrialStartedAt { get; set; }
    public DateTimeOffset? TrialEndsAt { get; set; }
    public int RemainingTrialDays { get; set; }
    public bool CanUseProduct { get; set; }
    public bool CanAcceptBookings { get; set; }
    public bool CanProtectExistingCommitments { get; set; }
    public bool UpgradeRequired { get; set; }
    public bool IsComplimentaryBridge { get; set; }
    public DateTimeOffset? NextBillingAt { get; set; }
    public Organization Organization { get; set; } = new();
}
