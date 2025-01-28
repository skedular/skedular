using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationTagType? Type { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Desk> TaggedDesks { get; set; } = [];
    public ICollection<Room> TaggedRooms { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
