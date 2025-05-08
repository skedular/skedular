using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IBookingResourceSlotsHelperService
{
    void RemoveAllSlotsFromBooking(Database.Entities.Booking booking);
}

public class BookingResourceSlotsHelperService(IRepositoryFactory repositoryFactory) : IBookingResourceSlotsHelperService
{
    public void RemoveAllSlotsFromBooking(Database.Entities.Booking booking)
    {
        foreach (var slot in booking.ResourceBookingSlots)
        {
            slot.Customers.Clear();
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(booking.ResourceBookingSlots);
        booking.ResourceBookingSlots.Clear();
        repositoryFactory.BookingRepository.Update(booking);
    }
}
