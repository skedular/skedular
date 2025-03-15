using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Resource : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public bool IsAvailableHoursOverridden { get; set; }
    public OpeningHours? AvailableHours { get; set; }

    public Location? Location { get; set; }
    public ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
