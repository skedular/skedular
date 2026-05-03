using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    public string? Timezone { get; set; }
    public LocationType? Type { get; set; }
    public IReadOnlyList<Resource> Resources { get; set; } = [];
    public Organization? Organization { get; set; }
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public WorkspaceChannel? DailyUpdateChannel { get; set; }
    public LocationPermissions Permissions { get; set; } = new();
}
