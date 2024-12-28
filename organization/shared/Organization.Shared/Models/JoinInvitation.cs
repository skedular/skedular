using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class JoinInvitation : ModelBaseWithDeleted
{
    public string? Email { get; set; } = string.Empty;
    public InvitationStatus Status { get; set; }
    public OrganizationMembershipType MembershipType { get; set; }
    public Organization Organization { get; set; }
    public Customer CreatedBy { get; set; }
    public Customer? Invitee { get; set; }
}
