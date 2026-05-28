using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Resource : ReplicatedModelBaseWithDeleted
{
    public Location? Location { get; set; }
    public IReadOnlyList<Customer> PreferredByCustomers { get; set; } = [];
}
