using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public ICollection<LocationResource> Resources { get; set; } = [];
    public ICollection<Desk> Desks { get; set; } = [];
    public ICollection<Room> Rooms { get; set; } = [];
    public Organization? Organization { get; set; }
    public ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public ICollection<LocationMember> LocationMembers { get; set; } = [];
}
