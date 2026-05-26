using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public string? Name { get; set; }
    public ListingMetadata MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty;
    public string? Website { get; set; }
    public bool AgreedToTermsOfUse { get; set; }
    public string? LogoUrl { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationBillingCycle BillingCycle { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public bool HasAttachedPaymentMethod { get; set; }
    public IReadOnlyList<OrganizationCustomTag> Tags { get; set; } = [];
    public IReadOnlyList<OrganizationResourceType> ResourceTypes { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public IReadOnlyList<Team> Teams { get; set; } = [];
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public IReadOnlyList<Workspace> Workspaces { get; set; } = [];
    public WorkspaceChannel? DailyUpdateChannel { get; set; }
    public bool HasFutureBooking { get; set; }
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public OrganizationBillingDetails? BillingDetails { get; set; }
}
