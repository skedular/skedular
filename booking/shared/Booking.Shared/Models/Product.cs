using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Product : ReplicatedModelBaseWithDeleted
{
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<ProductVersion> ProductVersions { get; set; } = [];
}
