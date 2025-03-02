using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class OrganizationResourceType : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public OrganizationResourceTypeSystemType? SystemType { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Desk> Desks { get; set; } = [];
    public ICollection<Room> Rooms { get; set; } = [];
}
