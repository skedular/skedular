using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public LocationType? Type { get; set; }
    public ICollection<Resource> Resources { get; set; } = [];
    public Organization? Organization { get; set; }
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public WorkspaceChannel? DailyUpdateChannel { get; set; }
    public LocationPermissions Permissions { get; set; } = new();
    public bool HasFutureBooking { get; set; }
}
