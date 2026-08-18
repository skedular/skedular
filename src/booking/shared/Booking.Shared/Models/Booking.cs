using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public record ResourceCustomersPair(Resource Resource, IReadOnlyList<Customer> Customers);

public class Booking : ModelBaseWithDeleted
{
    public uint EntityFrameworkVersion { get; set; }
    public string? ConsumingCreditLedgerEntryId { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public BookingCategory Category { get; set; }
    public BookingChannel Channel { get; set; }
    public IReadOnlyList<BookingSchedule> Schedules { get; set; } = [];
    public MarketplaceBooking? MarketplaceBooking { get; set; }
    public IReadOnlyList<ResourceCustomersPair> Resources { get; set; } = [];
    public IReadOnlyList<Customer> InvolvedCustomers { get; set; } = [];
    public IReadOnlyList<Organization> InvolvedOrganizations { get; set; } = [];
    public IReadOnlyList<Location> InvolvedLocations { get; set; } = [];
    public IReadOnlyList<Team> InvolvedTeams { get; set; } = [];
    public IReadOnlyList<Resource> InvolvedResources { get; set; } = [];
    public Customer? CreatedByCustomer { get; set; }
    public Customer? LastModifiedByCustomer { get; set; }
    public Customer? DeletedByCustomer { get; set; }
    public RecurringBooking? RecurringBooking { get; set; }
    public bool? HasRecurringInstanceOverrides { get; set; }
    public bool CancellationPolicyOverridden { get; set; }
    public string? CancellationOverrideReason { get; set; }

    public IReadOnlyList<ResourceBookingSlot> ResourceBookingSlots
    {
        set
        {
            field = value;
            Resources =
            [
                .. field
                    .GroupBy(item => item.Resource.Id)
                    .Select(item =>
                    {
                        var slots = field.Where(slot => slot.Resource.Id == item.Key).ToList();
                        var allCustomersIncludingDuplicated = slots.SelectMany(slot => slot.Customers).ToList();
                        var customers = allCustomersIncludingDuplicated
                            .GroupBy(customer => customer.Id)
                            .Select(customer => allCustomersIncludingDuplicated.First(x => x.Id == customer.Key))
                            .ToList();

                        return new ResourceCustomersPair(slots.First().Resource, customers);
                    }),
            ];
        }
    } = [];
}
