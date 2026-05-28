using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Timezone { get; set; }
}
