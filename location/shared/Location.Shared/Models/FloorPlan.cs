using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class FloorPlan : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public int FloorLevel { get; set; }
    public string? FloorName { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Location Location { get; set; } = new();
    public ICollection<ResourcePosition> ResourcePositions { get; set; } = [];
}