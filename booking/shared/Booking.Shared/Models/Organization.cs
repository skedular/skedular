using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }
    public OrganizationType Type { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<Booking> InvolvedBookings { get; set; } = [];
    public ICollection<Booking> PaidBookings { get; set; } = [];
    public ICollection<StripeCustomer> StripeCustomers { get; set; } = [];
    public ICollection<OrganizationInvoiceCounter> OrganizationInvoiceCounters { get; set; } = [];
}
