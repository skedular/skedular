using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public OrganizationType Type { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public IReadOnlyList<OrganizationTag> Tags { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public IReadOnlyList<Customer> DefaultedByCustomers { get; set; } = [];
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
