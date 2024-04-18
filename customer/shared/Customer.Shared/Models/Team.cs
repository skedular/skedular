using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public Organization? Organization { get; set; }
    public ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
