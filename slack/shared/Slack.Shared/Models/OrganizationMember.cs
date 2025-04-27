using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public OrganizationMemberRole? Role { get; set; }
    public OrganizationMemberStatus Status { get; set; }
    public Organization Organization { get; set; } = new();
    public Customer Customer { get; set; } = new();
}
