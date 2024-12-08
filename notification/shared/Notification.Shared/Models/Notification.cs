using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Notification : ModelBaseWithDeleted
{
    public DateTimeOffset EventRaisedAt { get; set; }
    public string SourceId { get; set; }
    public string Type { get; set; }

    public virtual Customer? InvitedBy { get; set; }
    public virtual Customer? Invitee { get; set; }
    public virtual Organization? Organization { get; set; }
    public virtual Location? Location { get; set; }
    public virtual Team? Team { get; set; }
}
