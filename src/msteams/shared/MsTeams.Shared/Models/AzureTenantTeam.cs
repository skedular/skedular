using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class AzureTenantTeam : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? WebUrl { get; set; }
    public AzureTenant AzureTenant { get; set; } = new();
    public IReadOnlyList<AzureTenantTeamChannel> AzureTenantTeamChannels { get; set; } = [];
}
