using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class TeamMember : ReplicatedModelBaseWithDeleted
{
    public OldTeamMembershipType? MembershipType { get; set; }

    public Team Team { get; set; }
    public Customer Customer { get; set; }
    public OrganizationMember? OrganizationMember { get; set; }
}
