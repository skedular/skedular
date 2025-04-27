using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class TeamMember : ModelBaseWithDeleted
{
    public TeamMemberRole Role { get; set; }
    public TeamMemberStatus Status { get; set; }

    public Team Team { get; set; } = new();
    public Customer Customer { get; set; } = new();
    public OrganizationMember? OrganizationMember { get; set; }
}
