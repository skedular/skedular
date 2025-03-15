using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Booking : ModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }

    public Customer Customer { get; set; }
    public Organization? Organization { get; set; }
    public Location? Location { get; set; }
    public ICollection<Desk> Desks { get; set; }
    public ICollection<Room> Rooms { get; set; }
    public Team? Team { get; set; }
}
