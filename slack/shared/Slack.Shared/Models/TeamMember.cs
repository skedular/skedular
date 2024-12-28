using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class TeamMember : ReplicatedModelBaseWithDeleted
{
    public TeamMembershipType? MembershipType { get; set; }
    public TeamMemberStatus Status { get; set; }

    public Team Team { get; set; }
    public Customer Customer { get; set; }
    public OrganizationMember? OrganizationMember { get; set; }
}
