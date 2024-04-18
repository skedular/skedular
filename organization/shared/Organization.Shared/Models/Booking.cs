using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Booking : ReplicatedModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public Organization Organization { get; set; }
}
