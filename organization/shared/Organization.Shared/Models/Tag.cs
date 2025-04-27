using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Tag : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public OrganizationTagType Type { get; set; }
    public string? Color { get; set; }

    public Organization Organization { get; set; } = new();
}
