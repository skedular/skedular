using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Resource : ReplicatedModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailableHoursOverridden { get; set; }
    public OpeningHours? AvailableHours { get; set; }

    public Location? Location { get; set; }
    public IReadOnlyList<OrganizationTag> OrganizationTags { get; set; } = [];
    public IReadOnlyList<Customer> PreferredByCustomers { get; set; } = [];
    public IReadOnlyList<Booking> InvolvedBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> RequestedByRecurringBookings { get; set; } = [];
    public IReadOnlyList<MarketplaceBookingSubscription> RequestedByMarketplaceBookingSubscriptions { get; set; } = [];
}
