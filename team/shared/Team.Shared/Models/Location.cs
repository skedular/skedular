using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public Organization Organization { get; set; } = new();
}

