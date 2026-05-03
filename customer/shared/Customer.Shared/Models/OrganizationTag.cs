using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationTagType? Type { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<Customer> PreferredByCustomers { get; set; } = [];
}
