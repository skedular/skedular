using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }

    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}
