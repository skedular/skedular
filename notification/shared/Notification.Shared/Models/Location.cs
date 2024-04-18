using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
