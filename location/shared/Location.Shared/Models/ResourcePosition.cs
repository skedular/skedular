using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class ResourcePosition : ModelBase
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Shape { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    
    public Resource Resource { get; set; } = new();
    public FloorPlan FloorPlan { get; set; } = new();
}