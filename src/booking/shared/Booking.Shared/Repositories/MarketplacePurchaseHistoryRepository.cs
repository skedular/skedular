using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.Repositories;

public interface IMarketplacePurchaseHistoryRepository
{
    /// <summary>Creates or refreshes the one history row for a standalone marketplace booking.</summary>
    Task UpsertMarketplaceBookingAsync(Database.Entities.Booking booking, MarketplaceRefund? latestRefund,
        CancellationToken cancellationToken);

    /// <summary>Creates or refreshes the one history row for a marketplace subscription root.</summary>
    Task UpsertMarketplaceBookingSubscriptionAsync(MarketplaceBookingSubscriptionEntity subscription, MarketplaceRefund? latestRefund,
        CancellationToken cancellationToken);

    /// <summary>Refreshes the root purchase affected by a refund transition.</summary>
    Task RefreshForRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken);

    /// <summary>Refreshes the root purchase affected by a marketplace payment update.</summary>
    Task RefreshForMarketplaceBookingAsync(string marketplaceBookingId, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplacePurchaseHistoryRow>>, int)> GetPaginatedRowsAsync(
        PaginationInputParam paginationInputParam,
        MarketplacePurchaseHistorySearchCriteria searchCriteria,
        IReadOnlyList<MarketplacePurchaseHistoryOrder>? orderBy,
        CancellationToken cancellationToken);
}

/// <summary>Pages the durable marketplace-purchase read projection without loading booking aggregates.</summary>
public class MarketplacePurchaseHistoryRepository(BookingDbContext dbContext, TimeProvider? timeProvider = null)
    : IMarketplacePurchaseHistoryRepository
{
    public async Task UpsertMarketplaceBookingAsync(Database.Entities.Booking booking, MarketplaceRefund? latestRefund,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        var marketplaceBooking = booking.MarketplaceBooking;
        if (marketplaceBooking.MarketplaceBookingSubscriptionId is not null)
        {
            return;
        }

        var row = await FindBySourceAsync(MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking, marketplaceBooking.Id, cancellationToken);
        if (row is null)
        {
            row = (await dbContext.MarketplacePurchaseHistory.AddAsync(
                new MarketplacePurchaseHistory
                {
                    Id = CreateHistoryId(MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking, marketplaceBooking.Id),
                    SourceType = MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking,
                    SourceId = marketplaceBooking.Id,
                    MarketplaceBookingId = marketplaceBooking.Id,
                    CreatedAt = (timeProvider ?? TimeProvider.System).GetUtcNow(),
                }, cancellationToken)).Entity;
        }

        var productVersion = marketplaceBooking.ProductVersion;
        var product = productVersion.Product;
        ApplyCommonValues(row, marketplaceBooking, productVersion, product.OrganizationId, marketplaceBooking.PaidByCustomer?.Id,
            booking.CreatedAt, booking.ModifiedAt ?? booking.CreatedAt, booking.From, booking.Until, booking.DeletedAt.HasValue,
            booking.DeletedByCustomer?.Id, booking.CancellationOverrideReason, latestRefund);
        row.MarketplaceBookingId = marketplaceBooking.Id;
        row.MarketplaceBookingSubscriptionId = null;
        row.SubscriptionStatus = null;
        row.AutoRenew = false;
        row.CancelAtPeriodEnd = false;
    }

    public async Task UpsertMarketplaceBookingSubscriptionAsync(MarketplaceBookingSubscriptionEntity subscription, MarketplaceRefund? latestRefund,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        // The subscription root's marketplace booking is a template. It intentionally starts
        // as NOT_SET and may not have a charge amount. Once a recurring booking exists, its
        // marketplace booking is the authoritative payment for the current purchase cycle.
        var marketplaceBooking = await GetCurrentCycleMarketplaceBookingAsync(subscription.Id, cancellationToken)
                                 ?? subscription.MarketplaceBooking;
        var row = await FindBySourceAsync(MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription, subscription.Id,
            cancellationToken);
        if (row is null)
        {
            row = (await dbContext.MarketplacePurchaseHistory.AddAsync(
                new MarketplacePurchaseHistory
                {
                    Id = CreateHistoryId(MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription, subscription.Id),
                    SourceType = MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
                    SourceId = subscription.Id,
                    MarketplaceBookingSubscriptionId = subscription.Id,
                    CreatedAt = (timeProvider ?? TimeProvider.System).GetUtcNow(),
                }, cancellationToken)).Entity;
        }

        var productVersion = subscription.ProductVersion;
        var product = productVersion.Product;
        var activityAt = new[] { subscription.ModifiedAt ?? subscription.StartedAt, marketplaceBooking.ModifiedAt ?? marketplaceBooking.CreatedAt }
            .Max();
        ApplyCommonValues(row, marketplaceBooking, productVersion, product.OrganizationId,
            marketplaceBooking.PaidByCustomer?.Id ?? subscription.MarketplaceBooking.PaidByCustomer?.Id,
            subscription.StartedAt, activityAt, subscription.StartedAt, subscription.NextRenewalAt,
            subscription.DeletedAt.HasValue, subscription.DeletedByCustomer?.Id, subscription.CancellationOverrideReason, latestRefund);
        row.MarketplaceBookingId = null;
        row.MarketplaceBookingSubscriptionId = subscription.Id;
        row.SubscriptionStatus = subscription.Status;
        row.AutoRenew = subscription.AutoRenew;
        row.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;
    }

    public async Task RefreshForRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        // Refund transitions can be retried or delivered out of order. Always project
        // the durable latest refund for the source rather than whichever event invoked
        // this refresh, otherwise an older transition could regress the read model.
        var latestRefund = await dbContext.MarketplaceRefund.AsNoTracking()
            .Where(item => item.LocalEntityType == refund.LocalEntityType && item.LocalEntityId == refund.LocalEntityId)
            .OrderByDescending(item => item.RequestedAt)
            .ThenByDescending(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        switch (refund.LocalEntityType)
        {
            case MarketplaceRefundEntityTypeConstants.MarketplaceBooking:
                {
                    var booking = await dbContext.Booking.AsTracking()
                        .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.ProductVersion).ThenInclude(item => item.Product)
                        .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
                        .Include(item => item.DeletedByCustomer)
                        .SingleOrDefaultAsync(item => item.MarketplaceBooking != null && item.MarketplaceBooking.Id == refund.LocalEntityId,
                            cancellationToken);
                    if (booking is not null)
                    {
                        await UpsertMarketplaceBookingAsync(booking, latestRefund, cancellationToken);
                    }

                    break;
                }
            case MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription:
                {
                    var subscription = await dbContext.MarketplaceBookingSubscription.AsTracking()
                        .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.ProductVersion).ThenInclude(item => item.Product)
                        .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
                        .Include(item => item.ProductVersion).ThenInclude(item => item.Product)
                        .Include(item => item.DeletedByCustomer)
                        .SingleOrDefaultAsync(item => item.Id == refund.LocalEntityId, cancellationToken);
                    if (subscription is not null)
                    {
                        await UpsertMarketplaceBookingSubscriptionAsync(subscription, latestRefund, cancellationToken);
                    }

                    break;
                }
        }
    }

    public async Task RefreshForMarketplaceBookingAsync(string marketplaceBookingId, CancellationToken cancellationToken)
    {
        var subscription = await dbContext.MarketplaceBookingSubscription.AsTracking()
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.ProductVersion).ThenInclude(item => item.Product)
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
            .Include(item => item.ProductVersion).ThenInclude(item => item.Product)
            .Include(item => item.DeletedByCustomer)
            .SingleOrDefaultAsync(item => item.MarketplaceBooking.Id == marketplaceBookingId, cancellationToken);
        if (subscription is not null)
        {
            await UpsertMarketplaceBookingSubscriptionAsync(subscription, null, cancellationToken);
            return;
        }

        var booking = await dbContext.Booking.AsTracking()
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.ProductVersion).ThenInclude(item => item.Product)
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
            .Include(item => item.DeletedByCustomer)
            .SingleOrDefaultAsync(item => item.MarketplaceBooking != null && item.MarketplaceBooking.Id == marketplaceBookingId, cancellationToken);
        if (booking is not null)
        {
            await UpsertMarketplaceBookingAsync(booking, null, cancellationToken);
            return;
        }

        // Recurring marketplace bookings are owned by a subscription purchase rather than a
        // standalone booking. Payment activities update the child booking, so resolve its
        // subscription root and refresh that one history row in the same transaction.
        subscription = await dbContext.MarketplaceBookingSubscription.AsTracking()
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.ProductVersion).ThenInclude(item => item.Product)
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
            .Include(item => item.ProductVersion).ThenInclude(item => item.Product)
            .Include(item => item.DeletedByCustomer)
            .SingleOrDefaultAsync(item => item.RecurringBookings.Any(recurringBooking =>
                recurringBooking.MarketplaceBooking != null && recurringBooking.MarketplaceBooking.Id == marketplaceBookingId), cancellationToken);
        if (subscription is not null)
        {
            await UpsertMarketplaceBookingSubscriptionAsync(subscription, null, cancellationToken);
        }
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplacePurchaseHistoryRow>>, int)> GetPaginatedRowsAsync(
        PaginationInputParam paginationInputParam,
        MarketplacePurchaseHistorySearchCriteria searchCriteria,
        IReadOnlyList<MarketplacePurchaseHistoryOrder>? orderBy,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MarketplacePurchaseHistory.AsNoTracking().AsQueryable();
        if (searchCriteria.OrganizationCustomDomain is not null)
        {
            query = query.Where(item => dbContext.Organization.Any(organization =>
                organization.Id == item.OrganizationId && organization.CustomDomain == searchCriteria.OrganizationCustomDomain));
        }

        if (searchCriteria.SourceTypes is { Count: > 0 })
        {
            var sourceTypes = searchCriteria.SourceTypes.Select(ToSourceType).ToList();
            query = query.Where(item => sourceTypes.Contains(item.SourceType));
        }

        if (searchCriteria.ActivityFrom.HasValue)
        {
            query = query.Where(item => item.ActivityAt >= searchCriteria.ActivityFrom.Value);
        }

        if (searchCriteria.ActivityUntil.HasValue)
        {
            query = query.Where(item => item.ActivityAt <= searchCriteria.ActivityUntil.Value);
        }

        if (searchCriteria.BookingFrom.HasValue)
        {
            query = query.Where(item => item.BookingFrom >= searchCriteria.BookingFrom.Value);
        }

        if (searchCriteria.BookingUntil.HasValue)
        {
            query = query.Where(item => item.BookingUntil <= searchCriteria.BookingUntil.Value);
        }

        if (searchCriteria.ProductVersionId is not null)
        {
            query = query.Where(item => item.ProductVersionId == searchCriteria.ProductVersionId);
        }

        if (searchCriteria.CustomerId is not null)
        {
            query = query.Where(item => item.CustomerId == searchCriteria.CustomerId);
        }

        if (searchCriteria.PaymentStatuses is { Count: > 0 })
        {
            var statuses = searchCriteria.PaymentStatuses.Select(item => item.ToPaymentStatus()).ToList();
            query = query.Where(item => item.PaymentStatus != null && statuses.Contains(item.PaymentStatus));
        }

        if (searchCriteria.LifecycleStates is { Count: > 0 })
        {
            var states = searchCriteria.LifecycleStates;
            var subscriptionStatuses = states.Where(state => state is not MarketplacePurchaseLifecycleState.Deleted)
                .Select(state => state switch
                {
                    MarketplacePurchaseLifecycleState.Cancelled => MarketplaceBookingSubscriptionStatus.Cancelled
                        .ToMarketplaceBookingSubscriptionStatus(),
                    MarketplacePurchaseLifecycleState.Expired =>
                        MarketplaceBookingSubscriptionStatus.Expired.ToMarketplaceBookingSubscriptionStatus(),
                    MarketplacePurchaseLifecycleState.PaymentFailed => MarketplaceBookingSubscriptionStatus.RenewalFailed
                        .ToMarketplaceBookingSubscriptionStatus(),
                    MarketplacePurchaseLifecycleState.Active => MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatus(),
                    _ => string.Empty,
                }).Where(status => status.Length > 0).ToList();
            var includeDeleted = states.Contains(MarketplacePurchaseLifecycleState.Deleted);
            var includeActiveBooking = states.Contains(MarketplacePurchaseLifecycleState.Active);
            var includePendingSubscription = states.Contains(MarketplacePurchaseLifecycleState.Pending);
            var nonPendingSubscriptionStatuses = new[]
            {
                MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus(),
                MarketplaceBookingSubscriptionStatus.Expired.ToMarketplaceBookingSubscriptionStatus(),
                MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus(),
                MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatus(),
            };
            query = query.Where(item =>
                (includeDeleted && item.IsDeleted) ||
                (item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking && includeActiveBooking && !item.IsDeleted) ||
                (item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription && !item.IsDeleted &&
                 (subscriptionStatuses.Contains(item.SubscriptionStatus!) ||
                  (includePendingSubscription && !nonPendingSubscriptionStatuses.Contains(item.SubscriptionStatus!)))));
        }

        var page = await query.ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderBy), item => item.SourceId, cancellationToken);
        return (page.Item1, page.Item2.Select(edge => new Edge<MarketplacePurchaseHistoryRow>(ToRow(edge.Node), edge.Cursor)).ToList(), page.Item3);
    }

    private static string ToSourceType(MarketplacePurchaseSourceType sourceType) => sourceType switch
    {
        MarketplacePurchaseSourceType.Booking => MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking,
        MarketplacePurchaseSourceType.Subscription => MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
        _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, "Unknown marketplace purchase source type."),
    };

    private async Task<MarketplacePurchaseHistory?> FindBySourceAsync(string sourceType, string sourceId, CancellationToken cancellationToken) =>
        dbContext.MarketplacePurchaseHistory.Local.FirstOrDefault(item => item.SourceType == sourceType && item.SourceId == sourceId)
        ?? await dbContext.MarketplacePurchaseHistory.SingleOrDefaultAsync(item => item.SourceType == sourceType && item.SourceId == sourceId,
            cancellationToken);

    private async Task<MarketplaceBookingEntity?> GetCurrentCycleMarketplaceBookingAsync(
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var now = (timeProvider ?? TimeProvider.System).GetUtcNow();
        var recurringBooking = await dbContext.RecurringBooking.AsTracking()
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
            .Where(item => !item.DeletedAt.HasValue &&
                           item.MarketplaceBookingSubscription != null &&
                           item.MarketplaceBookingSubscription.Id == subscriptionId &&
                           item.MarketplaceBooking != null)
            // Prefer the active cycle. When reconciliation is running ahead of the cycle,
            // retain the most recently started bill rather than allowing a future pending
            // bill to hide a confirmed current payment.
            .OrderByDescending(item => item.StartDate <= now && (!item.EndDate.HasValue || item.EndDate.Value >= now))
            .ThenByDescending(item => item.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        return recurringBooking?.MarketplaceBooking;
    }

    private static string CreateHistoryId(string sourceType, string sourceId) => $"{sourceType}:{sourceId}";

    private static void ApplyCommonValues(MarketplacePurchaseHistory row, MarketplaceBookingEntity marketplaceBooking,
        ProductVersionEntity productVersion, string organizationId, string? customerId, DateTimeOffset purchasedAt, DateTimeOffset activityAt,
        DateTimeOffset? bookingFrom, DateTimeOffset? bookingUntil, bool isDeleted, string? deletedByCustomerId,
        string? cancellationReason, MarketplaceRefund? latestRefund)
    {
        row.OrganizationId = organizationId;
        row.ProductVersionId = productVersion.Id;
        row.ProductTitle = productVersion.ListingMetadata?.Title;
        row.CustomerId = customerId;
        row.PurchasedAt = purchasedAt;
        row.ActivityAt = new[]
        {
            activityAt, latestRefund?.RequestedAt ?? DateTimeOffset.MinValue, latestRefund?.LastProcessedAt ?? DateTimeOffset.MinValue,
        }.Max();
        row.BookingFrom = bookingFrom;
        row.BookingUntil = bookingUntil;
        row.PaymentStatus = marketplaceBooking.PaymentStatus;
        row.TotalAmount = marketplaceBooking.TotalAmount;
        row.Currency = marketplaceBooking.Currency ?? productVersion.Currency;
        row.IsDeleted = isDeleted;
        row.DeletedByCustomerId = deletedByCustomerId;
        row.CancellationReason = cancellationReason;
        if (latestRefund is not null)
        {
            row.LatestRefundId = latestRefund.Id;
            row.LatestRefundStatus = latestRefund.Status;
        }
    }

    private static MarketplacePurchaseHistoryRow ToRow(MarketplacePurchaseHistory item) => new(
        item.SourceId,
        item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription
            ? MarketplacePurchaseSourceType.Subscription
            : MarketplacePurchaseSourceType.Booking,
        item.PurchasedAt, item.ActivityAt, item.BookingFrom, item.BookingUntil, item.PaymentStatus ?? string.Empty,
        item.ProductVersionId, item.ProductTitle, item.TotalAmount, item.Currency, item.CustomerId, item.OrganizationId,
        item.DeletedByCustomerId, item.CancellationReason, item.SubscriptionStatus, item.AutoRenew, item.CancelAtPeriodEnd,
        item.IsDeleted, item.LatestRefundId);

    private static List<KeysetPaginationField<MarketplacePurchaseHistory>> GetPaginationFields(
        IReadOnlyList<MarketplacePurchaseHistoryOrder>? orderBy)
    {
        var orders = orderBy is { Count: > 0 }
            ? orderBy
            : [new MarketplacePurchaseHistoryOrder(OrderDirection.Descending, MarketplacePurchaseHistoryOrderField.ActivityAt)];
        var fields = orders.Select(order => order.Field switch
        {
            MarketplacePurchaseHistoryOrderField.PurchasedAt => KeysetPaginationField<MarketplacePurchaseHistory>.Create(
                nameof(MarketplacePurchaseHistory.PurchasedAt), item => item.PurchasedAt, order.Direction),
            MarketplacePurchaseHistoryOrderField.BookingFrom => KeysetPaginationField<MarketplacePurchaseHistory>.Create(
                nameof(MarketplacePurchaseHistory.BookingFrom), item => item.BookingFrom, order.Direction),
            MarketplacePurchaseHistoryOrderField.BookingUntil => KeysetPaginationField<MarketplacePurchaseHistory>.Create(
                nameof(MarketplacePurchaseHistory.BookingUntil), item => item.BookingUntil, order.Direction),
            _ => KeysetPaginationField<MarketplacePurchaseHistory>.Create(nameof(MarketplacePurchaseHistory.ActivityAt), item => item.ActivityAt,
                order.Direction),
        }).ToList();
        var direction = fields.LastOrDefault()?.Direction ?? OrderDirection.Descending;
        fields.Add(KeysetPaginationField<MarketplacePurchaseHistory>.Create(
            nameof(MarketplacePurchaseHistory.SourceType), item => item.SourceType, direction));
        return fields;
    }
}
