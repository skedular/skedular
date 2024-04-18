using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public OrganizationMembershipType? MembershipType { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
