using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public ICollection<Resource> Resources { get; set; } = [];
    public Organization? Organization { get; set; }
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
