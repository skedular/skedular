using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }

    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
}
