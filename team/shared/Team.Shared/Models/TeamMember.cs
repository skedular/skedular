using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class TeamMember : ModelBaseWithDeleted
{
    public string MembershipType { get; set; } = TeamMembershipType.Member;

    public Team Team { get; set; }
    public Customer Customer { get; set; }
    public OrganizationMember? OrganizationMember { get; set; }
}
