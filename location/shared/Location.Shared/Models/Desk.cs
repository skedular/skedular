using Enterprise.Shared.Database;

namespace Location.Shared.Models;

public class Desk : EntityBase
{
    public string Name { get; set; }
    public bool Deactivated { get; set; }
    public bool RequireBookingApproval { get; set; }

    public Location Location { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<OrganizationTag> DeskTypes { get; set; } = [];
    public ICollection<OrganizationTag> Zones { get; set; } = [];
}
