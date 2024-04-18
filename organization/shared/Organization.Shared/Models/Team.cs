using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public Organization Organization { get; set; }
}
