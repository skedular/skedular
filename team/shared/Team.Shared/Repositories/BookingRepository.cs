using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Booking Add(Booking booking);
    Booking Update(Booking booking);
    Booking Remove(Booking booking);
}

public class BookingRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Booking>(dbContext, timeProvider), IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Include(query => query.Team)
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
