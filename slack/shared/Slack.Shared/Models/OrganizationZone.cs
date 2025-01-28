using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationZone : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Desk> TaggedDesks { get; set; } = [];
    public ICollection<Room> TaggedRooms { get; set; } = [];
}
