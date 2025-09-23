using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public Organization? Organization { get; set; }
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
