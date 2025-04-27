using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Notification : ModelBaseWithDeleted
{
    public DateTimeOffset EventRaisedAt { get; set; }
    public string SourceId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }

    public Customer? InvitedBy { get; set; }
    public Customer? Invitee { get; set; }
    public Organization? Organization { get; set; }
    public Location? Location { get; set; }
    public Team? Team { get; set; }
}
