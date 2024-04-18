using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Booking Add(Booking booking);
    Booking Update(Booking booking);
    Booking Remove(Booking booking);
}

public class BookingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Booking>(dbContext), IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => query.Id == id)
            .Include(query => query.Organization)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Booking Add(Booking booking)
    {
        var now = timeProvider.GetUtcNow();
        booking.CreatedAt = now;
        return DbContext.Booking.Add(booking).Entity;
    }

    public Booking Update(Booking booking)
    {
        var now = timeProvider.GetUtcNow();
        booking.ModifiedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public Booking Remove(Booking booking)
    {
        var now = timeProvider.GetUtcNow();
        booking.DeletedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }
}
