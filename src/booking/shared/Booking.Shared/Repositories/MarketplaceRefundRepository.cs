using Api.Shared.Services;
using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public sealed record MarketplaceRefundOperationsMetric(
    string Provider,
    string Status,
    string? OrganizationId,
    long Count);

public sealed record MarketplaceRefundOperationsSnapshot(
    IReadOnlyList<MarketplaceRefundOperationsMetric> Refunds,
    IReadOnlyList<MarketplaceRefundOperationsMetric> OverdueBankTransfers,
    IReadOnlyList<MarketplaceRefundOperationsMetric> CancelledWithoutDecision);

public interface IMarketplaceRefundRepository : IRepository<MarketplaceRefund>
{
    MarketplaceRefund Add(MarketplaceRefund marketplaceRefund);
    MarketplaceRefund Update(MarketplaceRefund marketplaceRefund);
    Task<MarketplaceRefund?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceRefund?> GetByExternalPaymentRefundIdAsync(string externalRefundId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefund>> GetByStripePaymentContextAsync(
        string? stripeTransferId, string? stripeChargeId, string? stripePaymentIntentId, CancellationToken cancellationToken);

    Task<MarketplaceRefund?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken);

    Task<MarketplaceRefund?> GetActiveCancellationAsync(string organizationId, string localEntityType, string localEntityId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefund>> GetRefundsForReconciliationAsync(
        DateTimeOffset threshold,
        int maxCount,
        CancellationToken cancellationToken);

    Task<decimal> GetAllocatedRefundTotalAsync(string sourcePaymentProvider, string sourcePaymentReference, CancellationToken cancellationToken);
    Task<IReadOnlyList<MarketplaceRefund>> GetApprovedBankTransferRefundsBeforeAsync(DateTimeOffset threshold, CancellationToken cancellationToken);
    Task<MarketplaceRefundOperationsSnapshot> GetOperationsSnapshotAsync(DateTimeOffset overdueThreshold, CancellationToken cancellationToken);
    Task<MarketplaceRefundPaymentAllocation?> GetAllocationByIdAsync(string id, CancellationToken cancellationToken);

    Task<MarketplaceRefundPaymentAllocation?> GetSourceAllocationAsync(
        string sourcePaymentProvider,
        string sourcePaymentReference,
        CancellationToken cancellationToken);

    MarketplaceRefundPaymentAllocation AddAllocation(MarketplaceRefundPaymentAllocation allocation);

    Task<MarketplaceRefundPaymentAllocation> ReserveAllocationAsync(string refundId, string allocationId, decimal amount,
        CancellationToken cancellationToken);

    Task<MarketplaceRefund?> GetByLocalEntityAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<MarketplaceRefund?> GetLatestByLocalEntityAsync(
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefund>> GetByOrganizationIdAsync(
        string organizationId,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceRefund>>, int)> GetPaginatedByOrganizationIdAsync(
        string organizationId,
        IReadOnlyList<string>? statuses,
        DateTimeOffset? requestedAtFrom,
        DateTimeOffset? requestedAtTo,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken);

    Task<bool> TryClaimReconciliationAsync(string refundId, string workerId, DateTimeOffset now, TimeSpan leaseDuration,
        CancellationToken cancellationToken);

    Task<bool> RenewReconciliationLeaseAsync(string refundId, string workerId, DateTimeOffset now, TimeSpan leaseDuration,
        CancellationToken cancellationToken);

    Task ReleaseReconciliationLeaseAsync(string refundId, string workerId, CancellationToken cancellationToken);

    Task<MarketplaceRefundNotificationDelivery?> GetNotificationDeliveryAsync(string refundId, string eventType,
        string recipientId, CancellationToken cancellationToken);

    MarketplaceRefundNotificationDelivery AddNotificationDelivery(MarketplaceRefundNotificationDelivery delivery);

    Task<MarketplaceExternalRefundReconciliation?> GetExternalReconciliationAsync(
        string provider,
        string externalRefundId,
        string? organizationId,
        CancellationToken cancellationToken);

    MarketplaceExternalRefundReconciliation AddExternalReconciliation(MarketplaceExternalRefundReconciliation reconciliation);

    Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceExternalRefundReconciliation>>, int)> GetExternalReconciliationsAsync(
        string organizationId,
        string? provider,
        string? status,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceExternalRefundReconciliation>>, int)> GetUnassignedExternalReconciliationsAsync(
        string? provider,
        string? status,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceExternalRefundReconciliation>> GetOpenStripePayoutReconciliationsAsync(
        DateTimeOffset now,
        int maxCount,
        CancellationToken cancellationToken);

    MarketplaceExternalRefundReconciliation UpdateExternalReconciliation(
        MarketplaceExternalRefundReconciliation reconciliation);
}

public class MarketplaceRefundRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceRefund>(dbContext, timeProvider), IMarketplaceRefundRepository
{
    public MarketplaceRefund Add(MarketplaceRefund marketplaceRefund)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(marketplaceRefund.IdempotencyKey);
        marketplaceRefund.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefund.Add(marketplaceRefund).Entity;
    }

    public MarketplaceRefund Update(MarketplaceRefund marketplaceRefund)
    {
        marketplaceRefund.ModifiedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefund.Update(marketplaceRefund).Entity;
    }

    public async Task<MarketplaceRefund?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund
            .Include(query => query.PaymentAllocations)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<MarketplaceRefund?> GetByExternalPaymentRefundIdAsync(string externalRefundId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(query => query.ExternalPaymentRefundId == externalRefundId, cancellationToken);

    public async Task<IReadOnlyList<MarketplaceRefund>> GetByStripePaymentContextAsync(
        string? stripeTransferId, string? stripeChargeId, string? stripePaymentIntentId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund
            .Where(query => (stripeTransferId != null && query.StripeTransferId == stripeTransferId) ||
                            (stripeChargeId != null && query.StripeChargeId == stripeChargeId) ||
                            (stripePaymentIntentId != null && query.StripePaymentIntentId == stripePaymentIntentId))
            .ToListAsync(cancellationToken);

    public async Task<MarketplaceExternalRefundReconciliation?> GetExternalReconciliationAsync(
        string provider,
        string externalRefundId,
        string? organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceExternalRefundReconciliation.FirstOrDefaultAsync(
            query => query.Provider == provider && query.ExternalRefundId == externalRefundId &&
                     (organizationId == null || query.OrganizationId == organizationId),
            cancellationToken);

    public MarketplaceExternalRefundReconciliation AddExternalReconciliation(
        MarketplaceExternalRefundReconciliation reconciliation)
    {
        reconciliation.CreatedAt = TimeProvider.GetUtcNow();
        reconciliation.FirstSeenAt = reconciliation.CreatedAt;
        reconciliation.LastSeenAt = reconciliation.CreatedAt;
        return DbContext.MarketplaceExternalRefundReconciliation.Add(reconciliation).Entity;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceExternalRefundReconciliation>>, int)> GetExternalReconciliationsAsync(
        string organizationId,
        string? provider,
        string? status,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceExternalRefundReconciliation
            .Where(item => item.OrganizationId == organizationId)
            .Where(item => string.IsNullOrWhiteSpace(provider) || item.Provider == provider)
            .Where(item => string.IsNullOrWhiteSpace(status) || item.Status == status)
            .AsNoTracking()
            .ToPaginatedAsync(
                paginationInputParam,
                [
                    KeysetPaginationField<MarketplaceExternalRefundReconciliation>.Create(
                        nameof(MarketplaceExternalRefundReconciliation.FirstSeenAt),
                        item => item.FirstSeenAt,
                        OrderDirection.Ascending),
                ],
                cancellationToken);

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceExternalRefundReconciliation>>, int)> GetUnassignedExternalReconciliationsAsync(
        string? provider,
        string? status,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceExternalRefundReconciliation
            .Where(item => item.OrganizationId == null)
            .Where(item => string.IsNullOrWhiteSpace(provider) || item.Provider == provider)
            .Where(item => string.IsNullOrWhiteSpace(status) || item.Status == status)
            .AsNoTracking()
            .ToPaginatedAsync(
                paginationInputParam,
                [
                    KeysetPaginationField<MarketplaceExternalRefundReconciliation>.Create(
                        nameof(MarketplaceExternalRefundReconciliation.FirstSeenAt),
                        item => item.FirstSeenAt,
                        OrderDirection.Ascending),
                ],
                cancellationToken);

    public async Task<IReadOnlyList<MarketplaceExternalRefundReconciliation>> GetOpenStripePayoutReconciliationsAsync(
        DateTimeOffset now,
        int maxCount,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceExternalRefundReconciliation
            .Where(item => item.Provider == MarketplaceExternalRefundReconciliationProviderConstants.StripePayout
                           && item.Status == MarketplaceExternalRefundReconciliationStatusConstants.Open
                           && item.StripeAccountId != null
                           && item.StripeAccountId != "" && item.RetryCount < 3
                           && (item.NextRetryAt == null || item.NextRetryAt <= now))
            .OrderBy(item => item.LastSeenAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);

    public MarketplaceExternalRefundReconciliation UpdateExternalReconciliation(
        MarketplaceExternalRefundReconciliation reconciliation)
    {
        reconciliation.ModifiedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceExternalRefundReconciliation.Update(reconciliation).Entity;
    }

    public async Task<MarketplaceRefund?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(query => query.IdempotencyKey == idempotencyKey, cancellationToken);

    public async Task<MarketplaceRefund?> GetActiveCancellationAsync(string organizationId, string localEntityType, string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(query =>
                query.OrganizationId == organizationId && query.LocalEntityType == localEntityType && query.LocalEntityId == localEntityId &&
                query.RefundKind == MarketplaceRefundKindConstants.Cancellation &&
                query.Status != MarketplaceRefundStatusConstants.Completed &&
                query.Status != MarketplaceRefundStatusConstants.Rejected &&
                query.Status != MarketplaceRefundStatusConstants.Cancelled,
            cancellationToken);

    public async Task<IReadOnlyList<MarketplaceRefund>> GetRefundsForReconciliationAsync(
        DateTimeOffset threshold,
        int maxCount,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxCount);

        return await DbContext.MarketplaceRefund
            .Where(query =>
                ((query.Status == MarketplaceRefundStatusConstants.ProviderPending ||
                  query.Status == MarketplaceRefundStatusConstants.Processing) &&
                 (query.LastProcessedAt ?? query.RequestedAt) < threshold) ||
                (query.AccountingProvider == AccountingProviderConstants.Xero &&
                 query.ExternalRefundId != null &&
                 query.Status != MarketplaceRefundStatusConstants.Rejected &&
                 query.Status != MarketplaceRefundStatusConstants.Cancelled &&
                 (query.LastReconciledAt == null || query.LastReconciledAt < threshold)))
            .OrderBy(query => query.LastProcessedAt ?? query.RequestedAt)
            .ThenBy(query => query.Id)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetAllocatedRefundTotalAsync(string sourcePaymentProvider, string sourcePaymentReference,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefundPaymentAllocation
            .Where(item => !item.IsSourcePayment &&
                           item.SourcePaymentProvider == sourcePaymentProvider &&
                           item.SourcePaymentReference == sourcePaymentReference)
            .Select(item => (decimal?)item.AllocatedRefundAmount)
            .SumAsync(cancellationToken) ?? 0m;

    public async Task<IReadOnlyList<MarketplaceRefund>> GetApprovedBankTransferRefundsBeforeAsync(DateTimeOffset threshold,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund
            .Where(item => item.Status == MarketplaceRefundStatusConstants.Approved &&
                           item.BankTransferSentAt == null && item.RequestedAt < threshold)
            .OrderBy(item => item.RequestedAt)
            .ToListAsync(cancellationToken);

    public async Task<MarketplaceRefundOperationsSnapshot> GetOperationsSnapshotAsync(
        DateTimeOffset overdueThreshold, CancellationToken cancellationToken)
    {
        var refundCounts = await DbContext.MarketplaceRefund
            .GroupBy(item => new
            {
                Provider = item.PaymentProvider ?? "unknown",
                item.Status,
                item.OrganizationId,
            })
            .Select(group => new MarketplaceRefundOperationsMetric(
                group.Key.Provider,
                group.Key.Status,
                group.Key.OrganizationId,
                group.LongCount()))
            .ToListAsync(cancellationToken);
        var overdueBankTransfers = await DbContext.MarketplaceRefund
            .Where(item =>
                item.Status == MarketplaceRefundStatusConstants.Approved &&
                item.BankTransferSentAt == null && item.RequestedAt < overdueThreshold)
            .GroupBy(item => new
            {
                Provider = item.PaymentProvider ?? "unknown",
                item.Status,
                item.OrganizationId,
            })
            .Select(group => new MarketplaceRefundOperationsMetric(
                group.Key.Provider,
                group.Key.Status,
                group.Key.OrganizationId,
                group.LongCount()))
            .ToListAsync(cancellationToken);
        var cancelledWithoutDecision = await DbContext.MarketplaceBooking
            .Where(item => item.Booking != null && item.Booking.DeletedAt != null)
            .Where(item => !DbContext.MarketplaceRefund.Any(refund =>
                refund.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBooking &&
                refund.LocalEntityId == item.Id))
            .GroupBy(item => item.PaidByOrganization == null ? "unknown" : item.PaidByOrganization.Id)
            .Select(group => new MarketplaceRefundOperationsMetric(
                "none",
                "NoRefundDecision",
                group.Key,
                group.LongCount()))
            .ToListAsync(cancellationToken);

        return new MarketplaceRefundOperationsSnapshot(
            refundCounts,
            overdueBankTransfers,
            cancelledWithoutDecision);
    }

    public async Task<MarketplaceRefundPaymentAllocation?> GetAllocationByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefundPaymentAllocation.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<MarketplaceRefundPaymentAllocation?> GetSourceAllocationAsync(
        string sourcePaymentProvider,
        string sourcePaymentReference,
        CancellationToken cancellationToken) =>
        // Reservation updates this aggregate's balance later in the same transaction. Lock an
        // existing source row before returning it so simultaneous refund requests serialize.
        await DbContext.MarketplaceRefundPaymentAllocation
            .TagWith(EntityFrameworkInterceptorTags.ForUpdate)
            .FirstOrDefaultAsync(item =>
                item.IsSourcePayment &&
                item.SourcePaymentProvider == sourcePaymentProvider &&
                item.SourcePaymentReference == sourcePaymentReference, cancellationToken);

    public MarketplaceRefundPaymentAllocation AddAllocation(MarketplaceRefundPaymentAllocation allocation)
    {
        allocation.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefundPaymentAllocation.Add(allocation).Entity;
    }

    public async Task<MarketplaceRefundPaymentAllocation> ReserveAllocationAsync(string refundId, string allocationId, decimal amount,
        CancellationToken cancellationToken)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        var source = DbContext.MarketplaceRefundPaymentAllocation.Local
                         .FirstOrDefault(item => item.Id == allocationId && item.IsSourcePayment)
                     ?? await DbContext.MarketplaceRefundPaymentAllocation
                         .FirstOrDefaultAsync(item => item.Id == allocationId && item.IsSourcePayment, cancellationToken)
                     ?? throw new InvalidOperationException("The source payment allocation was not found.");
        if (source.AllocatedRefundAmount + amount > source.SourceCapturedAmount)
        {
            throw new InvalidOperationException("The refund exceeds the remaining source-payment balance.");
        }

        // Updating the canonical source row makes EntityFrameworkVersion participate in
        // optimistic concurrency. Competing reservations cannot both commit.
        source.AllocatedRefundAmount += amount;
        source.ModifiedAt = TimeProvider.GetUtcNow();
        // `source` is either query-tracked or was just added above. Calling Update here changes
        // a newly added source row into Modified, causing EF to issue an UPDATE for a row that
        // has not been inserted yet and report a false concurrency conflict.

        var result = AddAllocation(new MarketplaceRefundPaymentAllocation
        {
            Id = Guid.NewGuid().ToString("N"),
            MarketplaceRefundId = refundId,
            SourcePaymentProvider = source.SourcePaymentProvider,
            SourcePaymentReference = source.SourcePaymentReference,
            SourceCapturedAmount = source.SourceCapturedAmount,
            AllocatedRefundAmount = amount,
            IsSourcePayment = false,
            Currency = source.Currency,
        });
        return result;
    }

    public async Task<MarketplaceRefund?> GetByLocalEntityAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund.FirstOrDefaultAsync(
            query =>
                query.OrganizationId == organizationId &&
                query.LocalEntityType == localEntityType &&
                query.LocalEntityId == localEntityId,
            cancellationToken);

    public async Task<MarketplaceRefund?> GetLatestByLocalEntityAsync(
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund
            .Include(query => query.PaymentAllocations)
            .OrderByDescending(query => query.RequestedAt).FirstOrDefaultAsync(
                query =>
                    query.LocalEntityType == localEntityType &&
                    query.LocalEntityId == localEntityId,
                cancellationToken);

    public async Task<IReadOnlyList<MarketplaceRefund>> GetByOrganizationIdAsync(
        string organizationId,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken)
    {
        var query = DbContext.MarketplaceRefund.Where(item => item.OrganizationId == organizationId);
        if (statuses is { Count: > 0 })
        {
            query = query.Where(item => statuses.Contains(item.Status));
        }

        return await query
            .Include(item => item.PaymentAllocations)
            .OrderByDescending(item => item.RequestedAt)
            // Keep the legacy list-shaped GraphQL field bounded. The operations
            // queue uses the cursor-based external reconciliation connection; a
            // refund list must never materialize an unbounded organization table.
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceRefund>>, int)> GetPaginatedByOrganizationIdAsync(
        string organizationId,
        IReadOnlyList<string>? statuses,
        DateTimeOffset? requestedAtFrom,
        DateTimeOffset? requestedAtTo,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken)
    {
        var query = DbContext.MarketplaceRefund
            .Where(item => item.OrganizationId == organizationId);
        if (statuses is { Count: > 0 })
        {
            query = query.Where(item => statuses.Contains(item.Status));
        }

        if (requestedAtFrom is not null)
        {
            query = query.Where(item => item.RequestedAt >= requestedAtFrom);
        }

        if (requestedAtTo is not null)
        {
            query = query.Where(item => item.RequestedAt <= requestedAtTo);
        }

        return await query
            .Include(item => item.PaymentAllocations)
            .ToPaginatedAsync(
                paginationInputParam,
                [
                    KeysetPaginationField<MarketplaceRefund>.Create(
                        nameof(MarketplaceRefund.RequestedAt), item => item.RequestedAt, OrderDirection.Descending),
                    KeysetPaginationField<MarketplaceRefund>.Create(
                        nameof(MarketplaceRefund.Id), item => item.Id, OrderDirection.Descending),
                ],
                cancellationToken);
    }

    public async Task<bool> TryClaimReconciliationAsync(string refundId, string workerId, DateTimeOffset now,
        TimeSpan leaseDuration, CancellationToken cancellationToken)
    {
        var updated = await DbContext.MarketplaceRefund
            .Where(item => item.Id == refundId &&
                           (item.ReconciliationLeaseOwner == null || item.ReconciliationLeaseExpiresAt <= now))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.ReconciliationLeaseOwner, workerId)
                .SetProperty(item => item.ReconciliationLeaseExpiresAt, now.Add(leaseDuration))
                .SetProperty(item => item.ReconciliationLeaseRenewedAt, now), cancellationToken);
        return updated == 1;
    }

    public async Task<bool> RenewReconciliationLeaseAsync(string refundId, string workerId, DateTimeOffset now,
        TimeSpan leaseDuration, CancellationToken cancellationToken)
    {
        var updated = await DbContext.MarketplaceRefund
            .Where(item => item.Id == refundId && item.ReconciliationLeaseOwner == workerId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.ReconciliationLeaseExpiresAt, now.Add(leaseDuration))
                .SetProperty(item => item.ReconciliationLeaseRenewedAt, now), cancellationToken);
        return updated == 1;
    }

    public async Task ReleaseReconciliationLeaseAsync(string refundId, string workerId, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceRefund
            .Where(item => item.Id == refundId && item.ReconciliationLeaseOwner == workerId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.ReconciliationLeaseOwner, (string?)null)
                .SetProperty(item => item.ReconciliationLeaseExpiresAt, (DateTimeOffset?)null)
                .SetProperty(item => item.ReconciliationLeaseRenewedAt, (DateTimeOffset?)null), cancellationToken);

    public Task<MarketplaceRefundNotificationDelivery?> GetNotificationDeliveryAsync(string refundId, string eventType,
        string recipientId, CancellationToken cancellationToken) =>
        DbContext.MarketplaceRefundNotificationDelivery.FirstOrDefaultAsync(item =>
                item.MarketplaceRefundId == refundId && item.EventType == eventType && item.RecipientId == recipientId,
            cancellationToken);

    public MarketplaceRefundNotificationDelivery AddNotificationDelivery(MarketplaceRefundNotificationDelivery delivery)
    {
        delivery.CreatedAt = TimeProvider.GetUtcNow();
        return DbContext.MarketplaceRefundNotificationDelivery.Add(delivery).Entity;
    }
}
