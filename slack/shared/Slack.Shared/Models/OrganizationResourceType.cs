using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationResourceType : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public OrganizationResourceTypeSystemType? SystemType { get; set; }
    public Organization Organization { get; set; }
}
