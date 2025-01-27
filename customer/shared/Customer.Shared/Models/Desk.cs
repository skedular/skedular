using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Desk : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }

    public Location Location { get; set; }
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
