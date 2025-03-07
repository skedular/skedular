using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class OrganizationResourceType : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public OrganizationResourceTypeSystemType? SystemType { get; set; }
    public Organization? Organization { get; set; }
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
