using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }

    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
}
