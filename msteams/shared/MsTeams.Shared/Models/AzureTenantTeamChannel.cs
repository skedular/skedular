using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class AzureTenantTeamChannel : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? WebUrl { get; set; }
    public string? Email { get; set; }
    public AzureTenantTeam AzureTenantTeam { get; set; } = new();
}
