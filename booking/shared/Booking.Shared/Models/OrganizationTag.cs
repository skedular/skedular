using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationTag : ReplicatedModelBase
{
    public string? Name { get; set; }
    public OrganizationTagType? Type { get; set; }
    public Organization Organization { get; set; }
    public ICollection<Desk> TaggedDesks { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
