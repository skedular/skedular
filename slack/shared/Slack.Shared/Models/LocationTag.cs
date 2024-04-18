using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class LocationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Location Location { get; set; }
    public ICollection<Desk> TaggedDesks { get; set; } = [];
}
