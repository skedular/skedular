using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public ICollection<Resource> Resources { get; set; } = [];
    public Organization Organization { get; set; }
    public ICollection<LocationMember> LocationMembers { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Customer> DefaultedByCustomers { get; set; } = [];
}
