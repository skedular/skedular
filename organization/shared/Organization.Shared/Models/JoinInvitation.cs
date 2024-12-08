using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class JoinInvitation : ModelBaseWithDeleted
{
    public string? Email { get; set; } = string.Empty;
    public string Status { get; set; }
    public string MembershipType { get; set; }
    public Organization Organization { get; set; }
    public Customer CreatedBy { get; set; }
    public Customer? Invitee { get; set; }
}
