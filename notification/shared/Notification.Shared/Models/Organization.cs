using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
