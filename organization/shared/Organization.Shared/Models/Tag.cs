using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Tag : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;

    public Organization Organization { get; set; }
}
