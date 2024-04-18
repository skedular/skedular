using Enterprise.Shared.Database;

namespace Location.Shared.Models;

public class Booking : ReplicatedEntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public Location Location { get; set; }
    public ICollection<Desk> Desks { get; set; } = [];
}
