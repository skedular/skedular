using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationBillingCycle BillingCycle { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public IReadOnlyList<string> RefundNotificationEmails { get; set; } = [];
    public bool? IsOwnershipVerified { get; set; }
    public IReadOnlyList<OrganizationTag> Tags { get; set; } = [];
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public IReadOnlyList<Team> Teams { get; set; } = [];
    public IReadOnlyList<Customer> DefaultedByCustomers { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public IReadOnlyList<Product> Products { get; set; } = [];
    public IReadOnlyList<Booking> InvolvedBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> InvolvedRecurringBooking { get; set; } = [];
    public IReadOnlyList<MarketplaceBooking> PaidMarketplaceBookings { get; set; } = [];
    public IReadOnlyList<StripeCustomer> StripeCustomers { get; set; } = [];
    public IReadOnlyList<OrganizationInvoiceCounter> OrganizationInvoiceCounters { get; set; } = [];
}
