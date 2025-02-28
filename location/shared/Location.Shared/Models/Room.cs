using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Room : ModelBaseWithDeleted
{
    public string Name { get; set; }
    public bool Deactivated { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }

    public Location Location { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<OrganizationTag> CustomTags { get; set; } = [];
    public ICollection<OrganizationTag> Zones { get; set; } = [];
}
