using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class OrganizationTag : ModelBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public OrganizationTagType? Type { get; set; }
    public Organization Organization { get; set; } = new();
}
