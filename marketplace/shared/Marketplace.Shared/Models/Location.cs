using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public Organization? Organization { get; set; }
    public LocationPhysicalAddress? PhysicalAddress { get; set; }
    public ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
}
