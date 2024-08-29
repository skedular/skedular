using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class AzureTenant : ReplicatedModelBaseWithDeleted
{
    public DateTimeOffset? TeamsAndChannelsLastRefreshedAt { get; set; }
    public Organization Organization { get; set; }
}
