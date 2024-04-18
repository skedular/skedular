using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Tag : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;

    public Location Location { get; set; }
    public ICollection<Desk> Desks { get; set; } = [];
}
