using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class JoinInvitation : ModelBaseWithDeleted
{
    public string? Email { get; set; }
    public InvitationStatus Status { get; set; }
    public LocationMembershipType MembershipType { get; set; }
    public Location Location { get; set; }
    public Customer CreatedBy { get; set; }
    public Customer? Invitee { get; set; }
}
