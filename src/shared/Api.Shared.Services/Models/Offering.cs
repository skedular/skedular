using Api.Shared.Services.Offering;

namespace Api.Shared.Services.Models;

public class Offering
{
    public string Id { get; set; } = string.Empty;
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int? PurchasedUserCapacity { get; set; }
    public int? PurchasedLocationCapacity { get; set; }
    public int? PurchasedTeamCapacity { get; set; }
    public int? CurrentActiveUserCount { get; set; }
    public bool? IsInteractionAllowed { get; set; }
    public string? EntitlementReasonCode { get; set; }
    public IReadOnlyList<string> ActiveCustomerIds { get; set; } = [];
    public int? SpacesPlanCode { get; set; }
    public int? SpacesQuotaLimit { get; set; }
    public int? SpacesCustomCapacity { get; set; }
    public DateTimeOffset? SpacesPeriodStart { get; set; }
    public DateTimeOffset? SpacesPeriodEnd { get; set; }
    public DateTimeOffset? SpacesTrialStartedAt { get; set; }
    public DateTimeOffset? SpacesTrialEndsAt { get; set; }
    public bool? SpacesProductEnabled { get; set; }
    public DateTimeOffset? SpacesNextBillingAt { get; set; }
    public decimal HostCommissionPercentage { get; set; }
}
