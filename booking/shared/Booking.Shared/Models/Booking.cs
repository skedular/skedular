using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Booking : ModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public string? Notes { get; set; }
    public BookingType Type { get; set; }
    public Customer Customer { get; set; }
    public Organization? Organization { get; set; }
    public Location? Location { get; set; }
    public ICollection<Desk> Desks { get; set; }
    public ICollection<Room> Rooms { get; set; }
    public Team? Team { get; set; }
}
