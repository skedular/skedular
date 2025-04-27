using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }

    public Organization? Organization { get; set; }
    public ICollection<Notification> Notifications { get; set; } = [];
}
