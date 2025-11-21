using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Website { get; set; }
    public bool AgreedToTermsOfUse { get; set; }
    public string? LogoUrl { get; set; }
    public OrganizationType Type { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public bool HasAttachedPaymentMethod { get; set; }
    public ICollection<OrganizationCustomTag> Tags { get; set; } = [];
    public ICollection<OrganizationResourceType> ResourceTypes { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Workspace> Workspaces { get; set; } = [];
    public WorkspaceChannel? DailyUpdateChannel { get; set; }
    public bool HasFutureBooking { get; set; }
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public OrganizationBillingDetails? BillingDetails { get; set; }
}
