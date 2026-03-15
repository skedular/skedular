using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class MarketplaceBookingSubscription : ModelBaseWithDeleted
{
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public DateTimeOffset? NextRenewalAt { get; set; }
    public MarketplaceBookingSubscriptionStatus Status { get; set; }
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public MarketplaceBooking MarketplaceBooking { get; set; } = new();
    public ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public ICollection<Team> InvolvedTeams { get; set; } = [];
    public Customer? CreatedByCustomer { get; set; }
    public Customer? LastModifiedByCustomer { get; set; }
    public Customer? DeletedByCustomer { get; set; }
    public ICollection<RecurringBooking> RecurringBookings { get; set; } = [];
}
