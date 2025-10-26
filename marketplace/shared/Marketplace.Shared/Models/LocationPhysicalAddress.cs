using Enterprise.Shared.Database;
using NetTopologySuite.Geometries;

namespace Marketplace.Shared.Models;

public class LocationPhysicalAddress : EntityBaseWithDeleted
{
    public Point? Coordinates { get; set; }
    public Location Location { get; set; } = new();
}
