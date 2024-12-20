using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationTag : ReplicatedModelBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Organization Organization { get; set; }
}
