using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public string? Timezone { get; set; }
}
