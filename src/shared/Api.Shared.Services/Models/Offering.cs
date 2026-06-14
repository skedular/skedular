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
}
