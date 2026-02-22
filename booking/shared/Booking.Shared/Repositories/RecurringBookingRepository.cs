using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface IRecurringBookingRepository : IRepository<RecurringBooking>
{
    RecurringBooking Add(RecurringBooking recurringBooking);
    RecurringBooking Update(RecurringBooking recurringBooking);
    RecurringBooking Remove(RecurringBooking recurringBooking);
}

public class RecurringBookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, RecurringBooking>(dbContext, timeProvider), IRecurringBookingRepository
{
    public RecurringBooking Add(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.CreatedAt = now;
        return DbContext.RecurringBooking.Add(recurringBooking).Entity;
    }

    public RecurringBooking Update(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.ModifiedAt = now;
        return DbContext.RecurringBooking.Update(recurringBooking).Entity;
    }

    public RecurringBooking Remove(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.DeletedAt = now;
        return DbContext.RecurringBooking.Update(recurringBooking).Entity;
    }
}
