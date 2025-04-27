using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class JoinInvitation : ModelBaseWithDeleted
{
    public string? Email { get; set; }
    public InvitationStatus Status { get; set; }
    public TeamMemberRole Role { get; set; }

    public Team Team { get; set; } = new();
    public Customer CreatedBy { get; set; } = new();
    public Customer? Invitee { get; set; }
}
