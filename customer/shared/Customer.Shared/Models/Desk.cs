using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Desk : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public Location Location { get; set; }
    public ICollection<LocationTag> Tags { get; set; } = [];
    public ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
