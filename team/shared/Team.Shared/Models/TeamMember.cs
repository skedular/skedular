using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class TeamMember : ModelBaseWithDeleted
{
    public OldTeamMembershipType MembershipType { get; set; } = OldTeamMembershipType.Member;

    public Team Team { get; set; }
    public Customer Customer { get; set; }
    public OrganizationMember? OrganizationMember { get; set; }
}
