using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Desk> Desks { get; set; } = [];
}
