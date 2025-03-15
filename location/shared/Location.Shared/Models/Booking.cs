using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Booking : ReplicatedModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }

    public Location Location { get; set; }
    public ICollection<Desk> Desks { get; set; } = [];
    public ICollection<Room> Rooms { get; set; } = [];
}
