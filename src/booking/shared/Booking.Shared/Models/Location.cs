using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Location : ReplicatedModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Timezone { get; set; }
    public LocationType Type { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public IReadOnlyList<Resource> Resources { get; set; } = [];
    public Organization? Organization { get; set; }
    public IReadOnlyList<Customer> DefaultedByCustomers { get; set; } = [];
    public IReadOnlyList<OrganizationTag> OrganizationTags { get; set; } = [];
    public IReadOnlyList<Booking> InvolvedBookings { get; set; } = [];
}
