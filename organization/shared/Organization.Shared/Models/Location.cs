using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public Organization Organization { get; set; } = new();
}
