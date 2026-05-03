using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing booking resource slots.
///     Provides functionality to remove all resource slots from a booking.
/// </summary>
public interface IBookingResourceSlotsHelperService
{
    /// <summary>
    ///     Removes all resource slots from the specified booking.
    ///     Clears customers from each slot and removes the slots from the booking.
    /// </summary>
    /// <param name="booking">The booking from which to remove all resource slots.</param>
    void RemoveAllSlotsFromBooking(Database.Entities.Booking booking);
}

/// <summary>
///     Implementation of the booking resource slots helper service.
/// </summary>
public class BookingResourceSlotsHelperService(IRepositoryFactory repositoryFactory) : IBookingResourceSlotsHelperService
{
    /// <summary>
    ///     Removes all resource slots from the specified booking.
    ///     Clears customers from each slot and removes the slots from the booking.
    /// </summary>
    /// <param name="booking">The booking from which to remove all resource slots.</param>
    public void RemoveAllSlotsFromBooking(Database.Entities.Booking booking)
    {
        foreach (var slot in booking.ResourceBookingSlots)
        {
            slot.Customers.Clear();
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(booking.ResourceBookingSlots.ToList());
        booking.ResourceBookingSlots.Clear();
        repositoryFactory.BookingRepository.Update(booking);
    }
}
