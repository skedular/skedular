using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingFailureRepository : IRepository<MarketplaceBookingFailure>
{
    MarketplaceBookingFailure Add(MarketplaceBookingFailure failure);
    MarketplaceBookingFailure Update(MarketplaceBookingFailure failure);
    Task<MarketplaceBookingFailure?> GetByFailureKeyAsync(string failureKey, CancellationToken cancellationToken);
    Task<MarketplaceBookingFailure?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceBookingFailure?> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken);
    Task<MarketplaceBookingFailure?> GetByRecurringBookingIdAsync(string recurringBookingId, CancellationToken cancellationToken);

    Task<MarketplaceBookingFailure?> GetByMarketplaceBookingSubscriptionIdAsync(string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceBookingFailure>> GetVisibleToCustomerAsync(string customerId, CancellationToken cancellationToken);
}

public class MarketplaceBookingFailureRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBookingFailure>(dbContext, timeProvider), IMarketplaceBookingFailureRepository
{
    public MarketplaceBookingFailure Add(MarketplaceBookingFailure failure)
    {
        failure.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingFailure.Add(failure).Entity;
    }

    public MarketplaceBookingFailure Update(MarketplaceBookingFailure failure)
    {
        failure.ModifiedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceBookingFailure.Update(failure).Entity;
    }

    public async Task<MarketplaceBookingFailure?> GetByFailureKeyAsync(string failureKey, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Include(item => item.Events)
            .Include(item => item.Deliveries)
            .FirstOrDefaultAsync(item => item.FailureKey == failureKey, cancellationToken);

    public async Task<MarketplaceBookingFailure?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Include(item => item.Events)
            .Include(item => item.Deliveries)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<MarketplaceBookingFailure?> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Include(item => item.Events)
            .Include(item => item.Deliveries)
            .OrderByDescending(item => item.FinalizedAt)
            .FirstOrDefaultAsync(item => item.BookingId == bookingId, cancellationToken);

    public async Task<MarketplaceBookingFailure?> GetByRecurringBookingIdAsync(string recurringBookingId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Include(item => item.Events)
            .Include(item => item.Deliveries)
            .OrderByDescending(item => item.FinalizedAt)
            .FirstOrDefaultAsync(item => item.RecurringBookingId == recurringBookingId, cancellationToken);

    public async Task<MarketplaceBookingFailure?> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Include(item => item.Events)
            .Include(item => item.Deliveries)
            .OrderByDescending(item => item.FinalizedAt)
            .FirstOrDefaultAsync(item => item.MarketplaceBookingSubscriptionId == marketplaceBookingSubscriptionId, cancellationToken);

    public async Task<IReadOnlyList<MarketplaceBookingFailure>> GetVisibleToCustomerAsync(
        string customerId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Include(item => item.Deliveries)
            .Where(item => item.Deliveries.Any(delivery =>
                delivery.RecipientCustomerId == customerId &&
                delivery.Channel == MarketplaceBookingFailureDeliveryChannelConstants.InApplication))
            .OrderByDescending(item => item.FinalizedAt)
            .ToListAsync(cancellationToken);
}
