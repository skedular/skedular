using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public record ResourceCustomersPair(Resource Resource, List<Customer> Customers);

public class Booking : ModelBaseWithDeleted
{
    private ICollection<ResourceBookingSlot> _resourceBookingSlots = [];

    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public BookingType Type { get; set; }
    public ICollection<BookingSchedule> Schedules { get; set; } = [];
    public BookingPaymentStatus PaymentStatus { get; set; }
    public bool IsPaymentRequired { get; set; }
    public ICollection<ProductVersionLineItem> LineItems { get; set; } = [];
    public bool BookedOnMarketplace { get; set; }
    public BookingPaymentMethod? PaymentMethod { get; set; }
    public bool? SendInvoice { get; set; }
    public string? InvoiceUrl { get; set; }

    public ICollection<ResourceBookingSlot> ResourceBookingSlots
    {
        get => _resourceBookingSlots;
        set
        {
            _resourceBookingSlots = value;
            Resources = ResourceBookingSlots
                .GroupBy(item => item.Resource.Id)
                .Select(item =>
                {
                    var slots = ResourceBookingSlots.Where(slot => slot.Resource.Id == item.Key).ToList();
                    var allCustomersIncludingDuplicated = slots.SelectMany(slot => slot.Customers).ToList();
                    var customers = allCustomersIncludingDuplicated
                        .GroupBy(customer => customer.Id)
                        .Select(customer => allCustomersIncludingDuplicated.First(x => x.Id == customer.Key))
                        .ToList();

                    return new ResourceCustomersPair(slots.First().Resource, customers);
                }).ToList();
        }
    }

    public ICollection<ResourceCustomersPair> Resources { get; set; } = [];
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
    public ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public ICollection<Location> InvolvedLocations { get; set; } = [];
    public ICollection<Team> InvolvedTeams { get; set; } = [];
    public Customer? PaidByCustomer { get; set; }
    public Organization? PaidByOrganization { get; set; }
    public Customer? CreatedByCustomer { get; set; }
    public Customer? LastModifiedByCustomer { get; set; }
    public Customer? DeletedByCustomer { get; set; }
    public StripeCheckoutSession? StripeCheckoutSession { get; set; }
    public DateTimeOffset BookingCheckoutSessionExpiry { get; set; }
}
