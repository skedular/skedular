using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Organization Organization { get; set; }
}
