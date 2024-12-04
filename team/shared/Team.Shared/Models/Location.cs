using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }

    public Organization? Organization { get; set; }
}
