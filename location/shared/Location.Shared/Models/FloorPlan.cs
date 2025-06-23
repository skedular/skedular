using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class FloorPlan : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public CdnImageFile Image { get; set; } = new(null, null);
    public Location Location { get; set; } = new();
    public ICollection<ResourcePosition> ResourcePositions { get; set; } = [];
}
