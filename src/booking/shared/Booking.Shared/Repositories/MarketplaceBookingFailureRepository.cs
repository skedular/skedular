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
    Task<IReadOnlyList<MarketplaceBookingFailure>> GetCleanupCandidatesAsync(int maxCount, CancellationToken cancellationToken);
    Task<IReadOnlyList<MarketplaceBookingFailure>> GetAccountingCleanupCandidatesAsync(int maxCount, CancellationToken cancellationToken);

    Task<bool> TryClaimCleanupAsync(string failureId, string workerId, DateTimeOffset now, TimeSpan leaseDuration,
        CancellationToken cancellationToken);

    Task ReleaseCleanupLeaseAsync(string failureId, string workerId, CancellationToken cancellationToken);
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

    public async Task<IReadOnlyList<MarketplaceBookingFailure>> GetCleanupCandidatesAsync(int maxCount, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Where(item => item.ResourceReleaseStatus == MarketplaceBookingFailureResourceReleaseStatusConstants.Pending &&
                           (item.Category == MarketplaceBookingFailureCategoryConstants.PaymentFailed ||
                            item.Category == MarketplaceBookingFailureCategoryConstants.PaymentExpired) &&
                           (item.BookingId != null || item.RecurringBookingId != null))
            .OrderBy(item => item.FinalizedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MarketplaceBookingFailure>>
        GetAccountingCleanupCandidatesAsync(int maxCount, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Where(item => item.ResourceReleaseStatus == MarketplaceBookingFailureResourceReleaseStatusConstants.Released &&
                           (item.AccountingCleanupStatus == MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending ||
                            item.AccountingCleanupStatus == MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired) &&
                           (item.BookingId != null || item.RecurringBookingId != null))
            .OrderBy(item => item.FinalizedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);

    public async Task<bool> TryClaimCleanupAsync(
        string failureId,
        string workerId,
        DateTimeOffset now,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken)
    {
        var updated = await DbContext.MarketplaceBookingFailure
            .Where(item => item.Id == failureId && item.ResourceReleaseStatus == MarketplaceBookingFailureResourceReleaseStatusConstants.Pending &&
                           (item.CleanupLeaseOwner == null || item.CleanupLeaseExpiresAt <= now))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.CleanupLeaseOwner, workerId)
                .SetProperty(item => item.CleanupLeaseExpiresAt, now.Add(leaseDuration))
                .SetProperty(item => item.CleanupLeaseRenewedAt, now)
                .SetProperty(item => item.CleanupLastAttemptAt, now)
                .SetProperty(item => item.CleanupAttemptCount, item => item.CleanupAttemptCount + 1), cancellationToken);
        return updated == 1;
    }

    public async Task ReleaseCleanupLeaseAsync(string failureId, string workerId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingFailure
            .Where(item => item.Id == failureId && item.CleanupLeaseOwner == workerId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.CleanupLeaseOwner, (string?)null)
                .SetProperty(item => item.CleanupLeaseExpiresAt, (DateTimeOffset?)null)
                .SetProperty(item => item.CleanupLeaseRenewedAt, (DateTimeOffset?)null), cancellationToken);
}
