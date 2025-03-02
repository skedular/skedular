using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class ResourceType : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public OrganizationResourceTypeSystemType? SystemType { get; set; }

    public Organization Organization { get; set; }
}
