using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public OrganizationTagType? Type { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
}
