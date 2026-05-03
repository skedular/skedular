using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted, ICustomerPersonalDetails
{
    public IReadOnlyList<Identity> Identities { get; set; } = [];
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public IReadOnlyList<TeamMember> TeamMembers { get; set; } = [];
    public Organization? DefaultOrganization { get; set; }
    public IReadOnlyList<Location> PreferredLocations { get; set; } = [];
    public IReadOnlyList<Resource> PreferredResources { get; set; } = [];
    public IReadOnlyList<OrganizationTag> PreferredOrganizationTags { get; set; } = [];
    public IReadOnlyList<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
    public IReadOnlyList<Booking> InvolvedBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> InvolvedRecurringBooking { get; set; } = [];
    public IReadOnlyList<MarketplaceBooking> PaidMarketplaceBookings { get; set; } = [];
    public IReadOnlyList<Booking> CreatedBookings { get; set; } = [];
    public IReadOnlyList<Booking> LastModifiedBookings { get; set; } = [];
    public IReadOnlyList<Booking> DeletedBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> CreatedRecurringBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> LastModifiedRecurringBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> DeletedRecurringBookings { get; set; } = [];
    public IReadOnlyList<StripeCustomer> StripeCustomers { get; set; } = [];
    public IReadOnlyList<MarketplaceBookingSubscription> CreatedMarketplaceBookingSubscriptions { get; set; } = [];
    public IReadOnlyList<MarketplaceBookingSubscription> LastModifiedMarketplaceBookingSubscriptions { get; set; } = [];
    public IReadOnlyList<MarketplaceBookingSubscription> DeletedMarketplaceBookingSubscriptions { get; set; } = [];
    public IReadOnlyList<MarketplaceBookingSubscription> InvolvedMarketplaceBookingSubscription { get; set; } = [];
    public string DisplayableName => this.ToDisplayableName();
    public CustomerType? Type { get; set; }
    public string? Designation { get; set; }
    public string? Title { get; set; }
    public string? Timezone { get; set; }
    public string? Locale { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }
    public string? PhoneNumber { get; set; }
}
