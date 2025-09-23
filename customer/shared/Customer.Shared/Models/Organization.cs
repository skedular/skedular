using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public OrganizationType Type { get; set; }
    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
