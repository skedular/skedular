using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationDeskType : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Desk> TaggedDesks { get; set; } = [];
}
