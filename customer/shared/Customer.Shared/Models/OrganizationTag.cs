using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public OrganizationTagType? Type { get; set; }
    public Organization Organization { get; set; } = new();
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
