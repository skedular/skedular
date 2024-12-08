using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class JoinInvitation : ModelBaseWithDeleted
{
    public string? Email { get; set; }
    public OldInvitationStatus Status { get; set; }
    public OldTeamMembershipType MembershipType { get; set; }

    public Team Team { get; set; }
    public Customer CreatedBy { get; set; }
    public Customer? Invitee { get; set; }
}
