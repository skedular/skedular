using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Product : ReplicatedModelBaseWithDeleted
{
    public Organization Organization { get; set; }
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
