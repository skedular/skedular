using Booking.Shared.Database.Entities;
using RecurringBooking = Booking.Shared.Models.RecurringBooking;

namespace Booking.Shared.Services;

public interface IPrivateRecurringBookingService
{
    Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<RecurringBooking> UpdateAsync(
        RecurringBooking booking,
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<RecurringBooking> DeleteAsync(
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);
}

public class PrivateRecurringBookingService : IPrivateRecurringBookingService
{
    public async Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task<RecurringBooking> UpdateAsync(
        RecurringBooking booking,
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task<RecurringBooking> DeleteAsync(
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
