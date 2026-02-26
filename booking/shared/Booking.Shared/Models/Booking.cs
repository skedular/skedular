using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public record ResourceCustomersPair(Resource Resource, List<Customer> Customers);

public class Booking : ModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public BookingCategory Category { get; set; }
    public BookingChannel Channel { get; set; }
    public ICollection<BookingSchedule> Schedules { get; set; } = [];
    public MarketplaceBooking? MarketplaceBooking { get; set; }
    public ICollection<ResourceCustomersPair> Resources { get; set; } = [];
    public ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public ICollection<Location> InvolvedLocations { get; set; } = [];
    public ICollection<Team> InvolvedTeams { get; set; } = [];
    public ICollection<Resource> InvolvedResources { get; set; } = [];
    public Customer? CreatedByCustomer { get; set; }
    public Customer? LastModifiedByCustomer { get; set; }
    public Customer? DeletedByCustomer { get; set; }

    public ICollection<ResourceBookingSlot> ResourceBookingSlots
    {
        set
        {
            field = value;
            Resources = field
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
                }).ToList();
        }
    } = [];
}
