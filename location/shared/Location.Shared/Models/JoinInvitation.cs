using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class JoinInvitation : ModelBaseWithDeleted
{
    public string? Email { get; set; }
    public string Status { get; set; }
    public string MembershipType { get; set; }
    public Location Location { get; set; }
    public Customer CreatedBy { get; set; }
    public Customer? Invitee { get; set; }
}
