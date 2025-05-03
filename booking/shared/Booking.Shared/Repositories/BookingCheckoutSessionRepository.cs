using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IBookingCheckoutSessionRepository : IRepository<BookingCheckoutSession>
{
    Task<BookingCheckoutSession> UpsertNakedAsync(string id, Database.Entities.Booking booking, CancellationToken cancellationToken);
    Task<BookingCheckoutSession?> GetByIdAsync(string id, CancellationToken cancellationToken);
    BookingCheckoutSession Update(BookingCheckoutSession bookingCheckoutSession);
    BookingCheckoutSession Remove(BookingCheckoutSession bookingCheckoutSession);
}

public class BookingCheckoutSessionRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, BookingCheckoutSession>(dbContext, timeProvider), IBookingCheckoutSessionRepository
{
    public async Task<BookingCheckoutSession> UpsertNakedAsync(string id, Database.Entities.Booking booking, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Database.Entities.Booking>(id, booking, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<BookingCheckoutSession?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.BookingCheckoutSession
            .Include(query => query.Booking)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public BookingCheckoutSession Update(BookingCheckoutSession bookingCheckoutSession)
    {
        var now = TimeProvider.GetUtcNow();
        bookingCheckoutSession.ModifiedAt = now;
        return DbContext.BookingCheckoutSession.Update(bookingCheckoutSession).Entity;
    }

    public BookingCheckoutSession Remove(BookingCheckoutSession bookingCheckoutSession)
    {
        var now = TimeProvider.GetUtcNow();
        bookingCheckoutSession.DeletedAt = now;
        return DbContext.BookingCheckoutSession.Update(bookingCheckoutSession).Entity;
    }
}
