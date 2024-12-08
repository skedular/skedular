using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public string? MembershipType { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
