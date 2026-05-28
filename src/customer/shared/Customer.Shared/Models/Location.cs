using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public LocationType? Type { get; set; }
    public IReadOnlyList<Resource> Resources { get; set; } = [];
    public Organization? Organization { get; set; }
    public IReadOnlyList<Customer> PreferredByCustomers { get; set; } = [];
    public IReadOnlyList<Customer> FavouredByCustomers { get; set; } = [];
}
