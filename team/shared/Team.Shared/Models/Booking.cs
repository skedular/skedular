using Enterprise.Shared.Database;

namespace Team.Shared.Models;

public class Booking : ReplicatedEntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public Team Team { get; set; }
}
