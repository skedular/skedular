using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Booking Update(Booking booking);
    Booking Remove(Booking booking);
}

public class BookingRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Booking>(dbContext, timeProvider), IBookingRepository
{
    public override async Task<Booking> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
