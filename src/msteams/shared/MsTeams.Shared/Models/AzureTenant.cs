using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class AzureTenant : ReplicatedModelBaseWithDeleted
{
    public Organization Organization { get; set; } = new();
}
