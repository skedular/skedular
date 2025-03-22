using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationZone : ModelBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; }
}
