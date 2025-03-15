using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Booking : ModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public BookingType Type { get; set; }
    public Customer Customer { get; set; }
    public Organization? Organization { get; set; }
    public Location? Location { get; set; }
    public ICollection<Desk> Desks { get; set; }
    public ICollection<Room> Rooms { get; set; }
    public ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
    public Team? Team { get; set; }

    public ICollection<(Resource, List<Customer>)> Resources =>
        ResourceBookingSlots
            .GroupBy(item => item.Resource.Id)
            .Select(item =>
            {
                var slots = ResourceBookingSlots.Where(slot => slot.Resource.Id == item.Key).ToList();
                var allCustomersIncludingDuplicated = slots.SelectMany(slot => slot.Customers).ToList();
                var customers = allCustomersIncludingDuplicated
                    .GroupBy(customer => customer.Id)
                    .Select(customer => allCustomersIncludingDuplicated.First(x => x.Id == customer.Key))
                    .ToList();

                return (slots.First().Resource, customers);
            })
            .ToList();
}
