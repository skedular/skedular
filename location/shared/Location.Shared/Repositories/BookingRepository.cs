using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Booking Add(Booking booking);
    Booking Update(Booking booking);
    Booking Remove(Booking booking);
}

public class BookingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Booking>(dbContext, timeProvider), IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Include(query => query.Location)
            .Include(query => query.Desks)
            .Include(query => query.Rooms)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public Booking Add(Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.CreatedAt = now;
        return DbContext.Booking.Add(booking).Entity;
    }

    public Booking Update(Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.ModifiedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public Booking Remove(Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.DeletedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }
}
