using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.Repositories;

public interface IMarketplacePurchaseHistoryRepository
{
    Task<MarketplacePurchaseHistoryEventModel> AppendEventAsync(
        MarketplacePurchaseHistoryEventModel eventModel,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplacePurchaseHistoryEventModel>> GetEventsAsync(
        string sourceType,
        string sourceId,
        CancellationToken cancellationToken);

    /// <summary>Creates or refreshes the one history row for a standalone marketplace booking.</summary>
    Task UpsertMarketplaceBookingAsync(Database.Entities.Booking booking, MarketplaceRefund? latestRefund,
        CancellationToken cancellationToken);

    /// <summary>Creates or refreshes the one history row for a marketplace subscription root.</summary>
    Task UpsertMarketplaceBookingSubscriptionAsync(MarketplaceBookingSubscriptionEntity subscription, MarketplaceRefund? latestRefund,
        CancellationToken cancellationToken);

    Task RefreshForEntitlementPurchaseAsync(string purchaseId, CancellationToken cancellationToken);

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
public class MarketplacePurchaseHistoryRepository(BookingDbContext dbContext, TimeProvider? timeProvider, IRandomHelper randomHelper)
    : IMarketplacePurchaseHistoryRepository
{
    public async Task<MarketplacePurchaseHistoryEventModel> AppendEventAsync(
        MarketplacePurchaseHistoryEventModel eventModel,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(eventModel);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);
        var sourceType = eventModel.SourceType switch
        {
            MarketplacePurchaseHistoryEligibleSourceType.Subscription => MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
            MarketplacePurchaseHistoryEligibleSourceType.Entitlement => MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
            _ => throw new ArgumentOutOfRangeException(nameof(eventModel.SourceType)),
        };

        var existing = await dbContext.MarketplacePurchaseHistory.AsNoTracking()
            .SingleOrDefaultAsync(item => item.SourceType == sourceType
                                          && item.SourceId == eventModel.SourceId
                                          && item.IdempotencyKey == idempotencyKey,
                cancellationToken);
        if (existing is not null)
        {
            return ToEventModel(existing);
        }

        var entity = new MarketplacePurchaseHistory
        {
            Id = randomHelper.Generate(),
            EventId = eventModel.Id,
            SourceType = sourceType,
            SourceId = eventModel.SourceId,
            EventType = eventModel.Type switch
            {
                MarketplacePurchaseHistoryEventType.PurchaseCreated => MarketplacePurchaseHistoryEventTypeConstants.PurchaseCreated,
                MarketplacePurchaseHistoryEventType.SubscriptionStarted => MarketplacePurchaseHistoryEventTypeConstants.SubscriptionStarted,
                MarketplacePurchaseHistoryEventType.SubscriptionRenewed => MarketplacePurchaseHistoryEventTypeConstants.SubscriptionRenewed,
                MarketplacePurchaseHistoryEventType.CancellationScheduled => MarketplacePurchaseHistoryEventTypeConstants.CancellationScheduled,
                MarketplacePurchaseHistoryEventType.CancellationCompleted => MarketplacePurchaseHistoryEventTypeConstants.CancellationCompleted,
                MarketplacePurchaseHistoryEventType.EntitlementCreated => MarketplacePurchaseHistoryEventTypeConstants.EntitlementCreated,
                MarketplacePurchaseHistoryEventType.EntitlementExpired => MarketplacePurchaseHistoryEventTypeConstants.EntitlementExpired,
                MarketplacePurchaseHistoryEventType.CreditsConsumed => MarketplacePurchaseHistoryEventTypeConstants.CreditsConsumed,
                MarketplacePurchaseHistoryEventType.PaymentStateChanged => MarketplacePurchaseHistoryEventTypeConstants.PaymentStateChanged,
                MarketplacePurchaseHistoryEventType.RefundStateChanged => MarketplacePurchaseHistoryEventTypeConstants.RefundStateChanged,
                _ => throw new ArgumentOutOfRangeException(nameof(eventModel.Type)),
            },
            IdempotencyKey = idempotencyKey,
            OccurredAt = eventModel.OccurredAt,
            RecordedAt = eventModel.RecordedAt,
            PreviousPaymentStatus = eventModel.PreviousPaymentStatus?.ToPaymentStatus(),
            PaymentStatus = eventModel.PaymentStatus?.ToPaymentStatus(),
            PreviousRefundStatus = eventModel.PreviousRefundStatus?.ToMarketplaceRefundStatusName(),
            RefundStatus = eventModel.RefundStatus?.ToMarketplaceRefundStatusName(),
            LatestRefundId = eventModel.RefundId,
            EventCreditQuantity = eventModel.CreditQuantity,
            EventRemainingCreditQuantity = eventModel.RemainingCreditQuantity,
            EventAmount = eventModel.Amount,
            EventCurrency = eventModel.Currency.ToNullableCurrency(),
            CancellationRequestedAt = eventModel.CancellationRequestedAt,
            CancellationEffectiveAt = eventModel.CancellationEffectiveAt,
            RenewalAt = eventModel.RenewalAt,
            EventReason = eventModel.Reason,
            EventSubscriptionStatus = eventModel.SubscriptionStatus?.ToMarketplaceBookingSubscriptionStatus(),
            EventEntitlementStatus = eventModel.EntitlementStatus?.ToPersistedValue(),
            EventAutoRenew = eventModel.AutoRenew,
            EventCancelAtPeriodEnd = eventModel.CancelAtPeriodEnd,
            EventIsDeleted = eventModel.IsDeleted,
            CorrelationId = eventModel.CorrelationId,
            CreatedAt = (timeProvider ?? TimeProvider.System).GetUtcNow(),
        };
        // A lifecycle projection may append its first event in the same unit of work
        // that creates the snapshot. Prefer the tracked snapshot so the append does
        // not depend on an earlier SaveChanges call.
        var sourceSnapshot = dbContext.MarketplacePurchaseHistory.Local
                                 .FirstOrDefault(item =>
                                     item.SourceType == sourceType && item.SourceId == eventModel.SourceId && item.EventType == null)
                             ?? await FindBySourceAsync(sourceType, eventModel.SourceId, cancellationToken);
        if (sourceSnapshot is not null)
        {
            entity.OrganizationId = sourceSnapshot.OrganizationId;
        }
        else
        {
            throw new InvalidOperationException(
                $"Cannot append marketplace purchase history event because source {eventModel.SourceId} has no purchase snapshot.");
        }

        await dbContext.MarketplacePurchaseHistory.AddAsync(entity, cancellationToken);
        await RebuildSnapshotFromEventsAsync(sourceSnapshot, eventModel, cancellationToken);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            var replay = await dbContext.MarketplacePurchaseHistory.AsNoTracking()
                .SingleOrDefaultAsync(item => item.SourceType == entity.SourceType && item.SourceId == entity.SourceId
                                                                                   && item.IdempotencyKey == idempotencyKey, cancellationToken);
            if (replay is not null)
            {
                return ToEventModel(replay);
            }

            throw;
        }

        return eventModel;
    }

    public async Task<IReadOnlyList<MarketplacePurchaseHistoryEventModel>> GetEventsAsync(
        string sourceType,
        string sourceId,
        CancellationToken cancellationToken)
    {
        _ = sourceType.ToEligibleSourceType();
        var rows = await dbContext.MarketplacePurchaseHistory.AsNoTracking()
            .Where(item => item.SourceType == sourceType && item.SourceId == sourceId && item.EventType != null)
            .OrderByDescending(item => item.OccurredAt)
            .ThenByDescending(item => item.RecordedAt)
            .ThenByDescending(item => item.Id)
            .ToListAsync(cancellationToken);
        return rows.Select(ToEventModel).ToList();
    }

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
        row.EntitlementPurchaseId = null;
        row.SubscriptionStatus = null;
        row.EntitlementStatus = null;
        row.CreditQuantity = 0;
        row.GrantedQuantity = 0;
        row.AvailableQuantity = 0;
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
        var created = row is null;
        var previousSubscriptionStatus = row?.SubscriptionStatus;
        var previousPaymentStatus = row?.PaymentStatus;
        var previousRenewalAt = row?.RenewalAt;
        var previousCancelAtPeriodEnd = row?.CancelAtPeriodEnd;
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
        row.RenewalAt = subscription.NextRenewalAt;
        row.AutoRenew = subscription.AutoRenew;
        row.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;
        row.EntitlementPurchaseId = null;
        row.EntitlementStatus = null;
        row.CreditQuantity = 0;
        row.GrantedQuantity = 0;
        row.AvailableQuantity = 0;
        if (created)
        {
            var occurredAt = subscription.CreatedAt;
            await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                    $"marketplace-purchase-history-event:subscription:{subscription.Id}:created",
                    subscription.Id,
                    MarketplacePurchaseHistoryEligibleSourceType.Subscription,
                    MarketplacePurchaseHistoryEventType.PurchaseCreated,
                    occurredAt,
                    (timeProvider ?? TimeProvider.System).GetUtcNow(),
                    null, null, null, null, null, null, null, null, null, null, null, null, null,
                    subscription.Status.ToMarketplaceBookingSubscriptionStatus(), null, subscription.AutoRenew,
                    subscription.CancelAtPeriodEnd, subscription.DeletedAt.HasValue),
                $"subscription:{subscription.Id}:purchase-created", cancellationToken);
            if (subscription.Status == MarketplaceBookingSubscriptionStatusConstants.Active)
            {
                await AppendAsync(subscription.Id, MarketplacePurchaseHistoryEventType.SubscriptionStarted,
                    subscription.StartedAt, row.PaymentStatus, null, null, null, null,
                    $"subscription:{subscription.Id}:started:{subscription.StartedAt:O}", cancellationToken);
            }
        }
        else
        {
            if (previousPaymentStatus != row.PaymentStatus)
            {
                await AppendAsync(subscription.Id, MarketplacePurchaseHistoryEventType.PaymentStateChanged,
                    row.ActivityAt, row.PaymentStatus, null, null, null,
                    null,
                    $"subscription:{subscription.Id}:payment:{row.ActivityAt:O}", cancellationToken);
            }

            if (previousSubscriptionStatus != row.SubscriptionStatus &&
                row.SubscriptionStatus == MarketplaceBookingSubscriptionStatusConstants.Active)
            {
                await AppendAsync(subscription.Id, MarketplacePurchaseHistoryEventType.SubscriptionStarted,
                    row.ActivityAt, row.PaymentStatus, null, null,
                    null, null, $"subscription:{subscription.Id}:started:{row.ActivityAt:O}", cancellationToken);
            }

            if (previousSubscriptionStatus != row.SubscriptionStatus &&
                row.SubscriptionStatus == MarketplaceBookingSubscriptionStatusConstants.Cancelled)
            {
                var scheduled = (await GetEventsAsync(
                        MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription, subscription.Id, cancellationToken))
                    .FirstOrDefault(item => item.Type == MarketplacePurchaseHistoryEventType.CancellationScheduled);
                await AppendAsync(subscription.Id, MarketplacePurchaseHistoryEventType.CancellationCompleted,
                    row.ActivityAt, null, scheduled?.CancellationRequestedAt,
                    scheduled?.CancellationEffectiveAt ?? row.ActivityAt, null, row.CancellationReason,
                    $"subscription:{subscription.Id}:cancelled:{row.ActivityAt:O}", cancellationToken);
            }

            if (previousRenewalAt != row.RenewalAt && row.RenewalAt.HasValue)
            {
                await AppendAsync(subscription.Id, MarketplacePurchaseHistoryEventType.SubscriptionRenewed,
                    row.RenewalAt.Value, row.PaymentStatus, null, null, row.RenewalAt,
                    null, $"subscription:{subscription.Id}:renewed:{row.RenewalAt:O}", cancellationToken);
            }

            if (previousCancelAtPeriodEnd != row.CancelAtPeriodEnd && row.CancelAtPeriodEnd)
            {
                await AppendAsync(subscription.Id, MarketplacePurchaseHistoryEventType.CancellationScheduled,
                    row.ActivityAt, null, row.ActivityAt, row.RenewalAt, row.RenewalAt, row.CancellationReason,
                    $"subscription:{subscription.Id}:cancellation-scheduled", cancellationToken);
            }
        }
    }

    public async Task RefreshForEntitlementPurchaseAsync(string purchaseId, CancellationToken cancellationToken)
    {
        var purchase = dbContext.EntitlementPurchase.Local.FirstOrDefault(item => item.Id == purchaseId)
                       ?? await dbContext.EntitlementPurchase.AsTracking()
                           .Include(item => item.ProductVersion).ThenInclude(item => item.Product)
                           .Include(item => item.Entitlement).ThenInclude(item => item!.LedgerEntries)
                           .SingleOrDefaultAsync(item => item.Id == purchaseId, cancellationToken);
        if (purchase is null)
        {
            return;
        }

        // The purchase can already be tracked by the confirmation service before its
        // entitlement relationship is assigned. In that case EF returns the tracked
        // instance above and does not apply the Include query, leaving the projection
        // with a null entitlement status and zero credit counts.
        if (purchase.EntitlementId is not null && purchase.Entitlement is null)
        {
            await dbContext.Entry(purchase).Reference(item => item.Entitlement).LoadAsync(cancellationToken);
        }

        if (purchase.Entitlement is not null && !dbContext.Entry(purchase.Entitlement).Collection(item => item.LedgerEntries).IsLoaded)
        {
            await dbContext.Entry(purchase.Entitlement).Collection(item => item.LedgerEntries).LoadAsync(cancellationToken);
        }

        var row = await FindBySourceAsync(MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase, purchase.Id, cancellationToken);
        var created = row is null;
        var previousPaymentStatus = row?.PaymentStatus;
        var previousEntitlementStatus = row?.EntitlementStatus;
        var previousAvailableQuantity = row?.AvailableQuantity;
        if (row is null)
        {
            row = (await dbContext.MarketplacePurchaseHistory.AddAsync(new MarketplacePurchaseHistory
            {
                Id = CreateHistoryId(MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase, purchase.Id),
                SourceType = MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
                SourceId = purchase.Id,
                EntitlementPurchaseId = purchase.Id,
                CreatedAt = (timeProvider ?? TimeProvider.System).GetUtcNow(),
            }, cancellationToken)).Entity;
        }

        var entitlement = purchase.Entitlement;
        row.OrganizationId = purchase.OrganizationId;
        row.ProductVersionId = purchase.ProductVersionId;
        row.ProductTitle = purchase.ProductVersion?.ListingMetadata?.Title;
        row.CustomerId = purchase.CustomerId;
        row.PurchasedAt = purchase.CreatedAt;
        row.ActivityAt = new[]
        {
            purchase.CreatedAt, purchase.ModifiedAt ?? purchase.CreatedAt, purchase.PaymentConfirmedAt ?? DateTimeOffset.MinValue,
            entitlement?.ModifiedAt ?? DateTimeOffset.MinValue,
        }.Max();
        row.PaymentStatus = purchase.PaymentStatus;
        row.TotalAmount = purchase.Amount;
        row.Currency = purchase.Currency;
        row.EntitlementStatus = entitlement?.Status.ToPersistedValue();
        row.CreditQuantity = purchase.ProductPricing.EntitlementCreditQuantity ?? 0;
        row.GrantedQuantity = entitlement?.GrantedQuantity ?? 0;
        row.AvailableQuantity = entitlement is null
            ? 0
            : entitlement.GrantedQuantity + entitlement.LedgerEntries
                .Where(item => item.TransactionType is "RELEASED" or "ADJUSTED").Sum(item => item.Quantity) - entitlement.LedgerEntries
                .Where(item => item.TransactionType is "CONSUMED" or "FORFEITED" or "EXPIRED").Sum(item => item.Quantity);
        row.SubscriptionStatus = null;
        row.AutoRenew = false;
        row.CancelAtPeriodEnd = false;
        row.IsDeleted = false;
        row.MarketplaceBookingId = null;
        row.MarketplaceBookingSubscriptionId = null;
        if (created)
        {
            await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                    $"marketplace-purchase-history-event:entitlement:{purchase.Id}:created",
                    purchase.Id,
                    MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                    MarketplacePurchaseHistoryEventType.PurchaseCreated,
                    purchase.CreatedAt,
                    (timeProvider ?? TimeProvider.System).GetUtcNow(),
                    null, purchase.PaymentStatus.ToPaymentStatus(), null, null, null,
                    purchase.ProductPricing.EntitlementCreditQuantity, entitlement?.GrantedQuantity,
                    purchase.Amount, purchase.Currency switch
                    {
                        CurrencyConstants.Nzd => Currency.Nzd,
                        CurrencyConstants.Usd => Currency.Usd,
                        _ => null,
                    }, null, null, null, "Entitlement purchase created", null,
                    entitlement?.Status is null ? null : ToNullableEntitlementStatus(entitlement.Status)),
                $"entitlement:{purchase.Id}:purchase-created", cancellationToken);

            if (entitlement is not null)
            {
                await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                        $"marketplace-purchase-history-event:entitlement:{purchase.Id}:entitlement-created",
                        purchase.Id, MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                        MarketplacePurchaseHistoryEventType.EntitlementCreated,
                        entitlement.CreatedAt, (timeProvider ?? TimeProvider.System).GetUtcNow(),
                        null, purchase.PaymentStatus.ToPaymentStatus(), null, null, null,
                        purchase.ProductPricing.EntitlementCreditQuantity, entitlement.GrantedQuantity,
                        purchase.Amount, null, null, null, null, "Entitlement created", null,
                        ToNullableEntitlementStatus(entitlement.Status)),
                    $"entitlement:{purchase.Id}:entitlement-created", cancellationToken);
            }
        }
        else if (previousPaymentStatus != row.PaymentStatus)
        {
            await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                    $"marketplace-purchase-history-event:entitlement:{purchase.Id}:payment:{row.ActivityAt:O}",
                    purchase.Id, MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                    MarketplacePurchaseHistoryEventType.PaymentStateChanged,
                    row.ActivityAt, (timeProvider ?? TimeProvider.System).GetUtcNow(),
                    ToNullablePaymentStatus(previousPaymentStatus), ToNullablePaymentStatus(row.PaymentStatus), null, null, null, null, null,
                    row.TotalAmount, row.Currency switch
                    {
                        CurrencyConstants.Nzd => Currency.Nzd,
                        CurrencyConstants.Usd => Currency.Usd,
                        _ => null,
                    }, null, null, null, "Entitlement payment state changed"),
                $"entitlement:{purchase.Id}:payment:{row.ActivityAt:O}", cancellationToken);
        }

        if (!created && previousEntitlementStatus is null && row.EntitlementStatus is not null)
        {
            await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                    $"marketplace-purchase-history-event:entitlement:{purchase.Id}:entitlement-created:{row.ActivityAt:O}",
                    purchase.Id, MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                    MarketplacePurchaseHistoryEventType.EntitlementCreated,
                    entitlement?.CreatedAt ?? row.ActivityAt, (timeProvider ?? TimeProvider.System).GetUtcNow(),
                    null, ToNullablePaymentStatus(row.PaymentStatus), null, null, null,
                    row.CreditQuantity, row.GrantedQuantity, row.TotalAmount, row.Currency switch
                    {
                        CurrencyConstants.Nzd => Currency.Nzd,
                        CurrencyConstants.Usd => Currency.Usd,
                        _ => null,
                    }, null, null, null, "Entitlement created", null,
                    entitlement?.Status is null ? null : ToNullableEntitlementStatus(entitlement.Status)),
                $"entitlement:{purchase.Id}:entitlement-created:{row.ActivityAt:O}", cancellationToken);
        }

        if (!created && previousEntitlementStatus != row.EntitlementStatus &&
            row.EntitlementStatus is not null &&
            EntitlementLifecycleStateExtensions.EntitlementStatusFromPersistedValue(row.EntitlementStatus) == EntitlementStatus.Expired)
        {
            await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                    $"marketplace-purchase-history-event:entitlement:{purchase.Id}:expired:{row.ActivityAt:O}",
                    purchase.Id, MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                    MarketplacePurchaseHistoryEventType.EntitlementExpired,
                    row.ActivityAt, (timeProvider ?? TimeProvider.System).GetUtcNow(),
                    null, ToNullablePaymentStatus(row.PaymentStatus), null, null, null, row.CreditQuantity,
                    row.AvailableQuantity, row.TotalAmount, null, null, null, null, "Entitlement expired", null,
                    EntitlementStatus.Expired),
                $"entitlement:{purchase.Id}:expired:{row.ActivityAt:O}", cancellationToken);
        }

        if (!created && previousAvailableQuantity.HasValue && row.AvailableQuantity < previousAvailableQuantity.Value)
        {
            await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                    $"marketplace-purchase-history-event:entitlement:{purchase.Id}:consumed:{row.ActivityAt:O}",
                    purchase.Id, MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                    MarketplacePurchaseHistoryEventType.CreditsConsumed,
                    row.ActivityAt, (timeProvider ?? TimeProvider.System).GetUtcNow(),
                    null, ToNullablePaymentStatus(row.PaymentStatus), null, null, null, previousAvailableQuantity - row.AvailableQuantity,
                    row.AvailableQuantity, null, null, null, null, null, "Credits consumed"),
                $"entitlement:{purchase.Id}:consumed:{row.ActivityAt:O}", cancellationToken);
        }
    }

    public async Task RefreshForRefundAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        // Refund transitions can be retried or delivered out of order. Always project
        // the durable latest refund for the source rather than whichever event invoked
        // this refresh, otherwise an older transition could regress the read model.
        var latestRefund = dbContext.MarketplaceRefund.Local
                               .Where(item => item.LocalEntityType == refund.LocalEntityType && item.LocalEntityId == refund.LocalEntityId)
                               .OrderByDescending(item => item.RequestedAt)
                               .ThenByDescending(item => item.Id)
                               .FirstOrDefault()
                           ?? await dbContext.MarketplaceRefund.AsNoTracking()
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
            case MarketplaceRefundEntityTypeConstants.EntitlementPurchase:
                {
                    await RefreshForEntitlementPurchaseAsync(refund.LocalEntityId, cancellationToken);
                    var history = await FindBySourceAsync(
                        MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
                        refund.LocalEntityId,
                        cancellationToken);
                    if (history is not null)
                    {
                        history.LatestRefundId = latestRefund?.Id;
                        history.LatestRefundStatus = latestRefund?.Status;
                    }

                    break;
                }
        }

        if (latestRefund is not null && refund.LocalEntityType is
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription or
                MarketplaceRefundEntityTypeConstants.EntitlementPurchase)
        {
            var sourceType = refund.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription
                ? MarketplacePurchaseHistoryEligibleSourceType.Subscription
                : MarketplacePurchaseHistoryEligibleSourceType.Entitlement;
            var history = await FindBySourceAsync(
                sourceType == MarketplacePurchaseHistoryEligibleSourceType.Subscription
                    ? MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription
                    : MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
                refund.LocalEntityId, cancellationToken);
            if (history is not null)
            {
                var occurredAt = latestRefund.LastProcessedAt ?? latestRefund.RequestedAt;
                await AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
                        $"marketplace-purchase-history-event:{refund.LocalEntityType}:{refund.LocalEntityId}:refund:{latestRefund.Id}:{latestRefund.Status}",
                        refund.LocalEntityId, sourceType, MarketplacePurchaseHistoryEventType.RefundStateChanged,
                        occurredAt, (timeProvider ?? TimeProvider.System).GetUtcNow(),
                        null, null, null, latestRefund.Status.ToMarketplaceRefundStatus(), latestRefund.Id, null, null,
                        latestRefund.RefundAmount, latestRefund.Currency switch
                        {
                            CurrencyConstants.Nzd => Currency.Nzd,
                            CurrencyConstants.Usd => Currency.Usd,
                            _ => null,
                        }, null, null, null, latestRefund.Reason),
                    $"{refund.LocalEntityType}:{refund.LocalEntityId}:refund:{latestRefund.Id}:{latestRefund.Status}", cancellationToken);
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
        // Event rows are read through the detail-history query. The purchases list retains
        // exactly one current snapshot row per source and must not expose lifecycle events as
        // duplicate purchases.
        var query = dbContext.MarketplacePurchaseHistory.AsNoTracking()
            .Where(item => item.EventType == null)
            .AsQueryable();
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
            var includeActiveEntitlement = states.Contains(MarketplacePurchaseLifecycleState.Active);
            var includePendingEntitlement = states.Contains(MarketplacePurchaseLifecycleState.Pending);
            var includePaymentFailedEntitlement = states.Contains(MarketplacePurchaseLifecycleState.PaymentFailed);
            var includeExpiredEntitlement = states.Contains(MarketplacePurchaseLifecycleState.Expired);
            var includeCancelledEntitlement = states.Contains(MarketplacePurchaseLifecycleState.Cancelled);
            var nonPendingSubscriptionStatuses = new[]
            {
                MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus(),
                MarketplaceBookingSubscriptionStatus.Expired.ToMarketplaceBookingSubscriptionStatus(),
                MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus(),
                MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatus(),
            };
            query = query.Where(item =>
                // A credit-funded booking is an implementation detail of the entitlement
                // usage ledger. The entitlement purchase remains the customer-facing
                // retained purchase, so do not list its deleted child booking separately.
                (includeDeleted && item.IsDeleted &&
                 (item.SourceType != MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking ||
                  item.MarketplaceBooking == null ||
                  (item.MarketplaceBooking.EntitlementId == null &&
                   item.MarketplaceBooking.MarketplaceBookingSubscriptionId == null &&
                   (item.MarketplaceBooking.Booking == null ||
                    item.MarketplaceBooking.Booking.RecurringBooking == null ||
                    item.MarketplaceBooking.Booking.RecurringBooking.MarketplaceBookingSubscription == null)))) ||
                (item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking && includeActiveBooking && !item.IsDeleted) ||
                (item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription && !item.IsDeleted &&
                 (subscriptionStatuses.Contains(item.SubscriptionStatus!) ||
                  (includePendingSubscription && !nonPendingSubscriptionStatuses.Contains(item.SubscriptionStatus!)))) ||
                (item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase && !item.IsDeleted &&
                 ((includeActiveEntitlement && item.PaymentStatus == PaymentStatusConstants.Confirmed &&
                   item.EntitlementStatus == nameof(EntitlementStatus.Active)) ||
                  (includePendingEntitlement && (item.PaymentStatus == PaymentStatusConstants.Pending ||
                                                 item.EntitlementStatus == nameof(EntitlementStatus.Pending))) ||
                  (includePaymentFailedEntitlement && item.PaymentStatus == PaymentStatusConstants.Rejected) ||
                  (includeExpiredEntitlement && (item.PaymentStatus == PaymentStatusConstants.Expired ||
                                                 item.EntitlementStatus == nameof(EntitlementStatus.Expired))) ||
                  (includeCancelledEntitlement && item.EntitlementStatus == nameof(EntitlementStatus.Cancelled)))));
        }

        var page = await query.ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderBy), item => item.SourceId, cancellationToken);
        var sourceIds = page.Item2.Select(edge => edge.Node.SourceId).ToList();
        var derivedStates = sourceIds.Count == 0
            ? new Dictionary<(string SourceType, string SourceId), MarketplacePurchaseHistoryCurrentState>()
            : (await dbContext.MarketplacePurchaseHistory.AsNoTracking()
                .Where(item => sourceIds.Contains(item.SourceId) && item.EventType != null)
                .OrderBy(item => item.OccurredAt)
                .ThenBy(item => item.RecordedAt)
                .ThenBy(item => item.Id)
                .ToListAsync(cancellationToken))
            .GroupBy(item => (item.SourceType, item.SourceId))
            .ToDictionary(group => group.Key, group => MarketplacePurchaseHistoryReducer.Reduce(group.Select(ToEventModel)));
        var latestRefunds = sourceIds.Count == 0
            ? new Dictionary<(string EntityType, string EntityId), (string Id, string Status)>()
            : await dbContext.MarketplaceRefund.AsNoTracking()
                .Where(item => sourceIds.Contains(item.LocalEntityId))
                .GroupBy(item => new
                {
                    item.LocalEntityType,
                    item.LocalEntityId,
                })
                .Select(group => new
                {
                    EntityType = group.Key.LocalEntityType,
                    EntityId = group.Key.LocalEntityId,
                    Refund = group.OrderByDescending(item => item.RequestedAt).ThenByDescending(item => item.Id).Select(item => new
                    {
                        item.Id,
                        item.Status,
                    }).First(),
                })
                .ToDictionaryAsync(item => (item.EntityType, item.EntityId), item => (item.Refund.Id, item.Refund.Status), cancellationToken);
        return (page.Item1, [
                .. page.Item2.Select(edge => new Edge<MarketplacePurchaseHistoryRow>(
                    ToRow(edge.Node, latestRefunds, derivedStates.GetValueOrDefault((edge.Node.SourceType, edge.Node.SourceId))), edge.Cursor)),
            ],
            page.Item3);
    }

    private async Task RebuildSnapshotFromEventsAsync(
        MarketplacePurchaseHistory snapshot,
        MarketplacePurchaseHistoryEventModel appendedEvent,
        CancellationToken cancellationToken)
    {
        var sourceType = appendedEvent.SourceType switch
        {
            MarketplacePurchaseHistoryEligibleSourceType.Subscription => MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
            MarketplacePurchaseHistoryEligibleSourceType.Entitlement => MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
            _ => throw new ArgumentOutOfRangeException(nameof(appendedEvent.SourceType)),
        };
        var persistedEvents = await dbContext.MarketplacePurchaseHistory.AsNoTracking()
            .Where(item => item.SourceType == sourceType && item.SourceId == appendedEvent.SourceId && item.EventType != null)
            .ToListAsync(cancellationToken);
        var state = MarketplacePurchaseHistoryReducer.Reduce(
            persistedEvents.Select(ToEventModel).Append(appendedEvent));

        snapshot.PurchasedAt = state.PurchasedAt ?? snapshot.PurchasedAt;
        snapshot.ActivityAt = state.ActivityAt ?? snapshot.ActivityAt;
        if (state.PaymentStatus.HasValue)
        {
            snapshot.PaymentStatus = state.PaymentStatus.Value.ToPaymentStatus();
        }

        if (state.RefundStatus.HasValue)
        {
            snapshot.LatestRefundId = state.RefundId;
            snapshot.LatestRefundStatus = state.RefundStatus.Value.ToMarketplaceRefundStatusName();
        }

        if (state.CreditQuantity.HasValue)
        {
            snapshot.CreditQuantity = state.CreditQuantity.Value;
        }

        if (state.RemainingCreditQuantity.HasValue)
        {
            snapshot.AvailableQuantity = state.RemainingCreditQuantity.Value;
        }

        if (state.SubscriptionStatus.HasValue)
        {
            snapshot.SubscriptionStatus = state.SubscriptionStatus.Value.ToMarketplaceBookingSubscriptionStatus();
        }

        if (state.EntitlementStatus.HasValue)
        {
            snapshot.EntitlementStatus = state.EntitlementStatus.Value.ToPersistedValue();
        }

        if (state.AutoRenew.HasValue)
        {
            snapshot.AutoRenew = state.AutoRenew.Value;
        }

        if (state.CancelAtPeriodEnd.HasValue)
        {
            snapshot.CancelAtPeriodEnd = state.CancelAtPeriodEnd.Value;
        }

        if (state.IsDeleted.HasValue)
        {
            snapshot.IsDeleted = state.IsDeleted.Value;
        }
    }

    private Task<MarketplacePurchaseHistoryEventModel> AppendAsync(
        string sourceId,
        MarketplacePurchaseHistoryEventType type,
        DateTimeOffset occurredAt,
        string? paymentStatus,
        DateTimeOffset? cancellationRequestedAt,
        DateTimeOffset? cancellationEffectiveAt,
        DateTimeOffset? renewalAt,
        string? reason,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        AppendEventAsync(new MarketplacePurchaseHistoryEventModel(
            $"marketplace-purchase-history-event:subscription:{sourceId}:{type}:{occurredAt:O}",
            sourceId,
            MarketplacePurchaseHistoryEligibleSourceType.Subscription,
            type,
            occurredAt,
            (timeProvider ?? TimeProvider.System).GetUtcNow(),
            null,
            ToNullablePaymentStatus(paymentStatus),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            cancellationRequestedAt,
            cancellationEffectiveAt,
            renewalAt,
            reason,
            type switch
            {
                MarketplacePurchaseHistoryEventType.SubscriptionStarted => MarketplaceBookingSubscriptionStatus.Active,
                MarketplacePurchaseHistoryEventType.CancellationCompleted => MarketplaceBookingSubscriptionStatus.Cancelled,
                _ => null,
            }, null, type == MarketplacePurchaseHistoryEventType.CancellationScheduled ? false : null,
            type == MarketplacePurchaseHistoryEventType.CancellationScheduled ? true : null), idempotencyKey, cancellationToken);

    private static MarketplacePurchaseHistoryEventModel ToEventModel(MarketplacePurchaseHistory item) =>
        new(item.EventId!, item.SourceId, item.SourceType.ToEligibleSourceType(), item.EventType!.ToEventType(), item.OccurredAt!.Value,
            item.RecordedAt!.Value, ToNullablePaymentStatus(item.PreviousPaymentStatus), ToNullablePaymentStatus(item.PaymentStatus),
            ToNullableRefundStatus(item.PreviousRefundStatus), ToNullableRefundStatus(item.RefundStatus),
            item.LatestRefundId, item.EventCreditQuantity, item.EventRemainingCreditQuantity, item.EventAmount,
            item.EventCurrency.ToNullableCurrency(), item.CancellationRequestedAt, item.CancellationEffectiveAt, item.RenewalAt, item.EventReason,
            item.EventSubscriptionStatus?.ToMarketplaceBookingSubscriptionStatus(), ToNullableEntitlementStatus(item.EventEntitlementStatus),
            item.EventAutoRenew, item.EventCancelAtPeriodEnd, item.EventIsDeleted, item.CorrelationId);

    private static string ToSourceType(MarketplacePurchaseSourceType sourceType) => sourceType switch
    {
        MarketplacePurchaseSourceType.Booking => MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBooking,
        MarketplacePurchaseSourceType.Subscription => MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
        MarketplacePurchaseSourceType.Entitlement => MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
        _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, "Unknown marketplace purchase source type."),
    };

    private static PaymentStatus? ToNullablePaymentStatus(string? value) =>
        value is null ? null : value.ToPaymentStatus();

    private static EntitlementStatus? ToNullableEntitlementStatus(string? value) => value switch
    {
        null => null,
        "PENDING" or nameof(EntitlementStatus.Pending) => EntitlementStatus.Pending,
        "ACTIVE" or nameof(EntitlementStatus.Active) => EntitlementStatus.Active,
        "EXPIRED" or nameof(EntitlementStatus.Expired) => EntitlementStatus.Expired,
        "CANCELLED" or nameof(EntitlementStatus.Cancelled) => EntitlementStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown entitlement status."),
    };

    private static EntitlementStatus? ToNullableEntitlementStatus(EntitlementStatus value) => value;

    private static MarketplaceRefundStatus? ToNullableRefundStatus(string? value) =>
        value is null ? null : value.ToMarketplaceRefundStatus();

    private async Task<MarketplacePurchaseHistory?> FindBySourceAsync(string sourceType, string sourceId, CancellationToken cancellationToken) =>
        dbContext.MarketplacePurchaseHistory.Local.FirstOrDefault(item =>
            item.SourceType == sourceType && item.SourceId == sourceId && item.EventType == null)
        ?? await dbContext.MarketplacePurchaseHistory.SingleOrDefaultAsync(
            item => item.SourceType == sourceType && item.SourceId == sourceId && item.EventType == null,
            cancellationToken);

    private async Task<MarketplaceBookingEntity?> GetCurrentCycleMarketplaceBookingAsync(
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var now = (timeProvider ?? TimeProvider.System).GetUtcNow();
        var recurringBookingQuery = dbContext.RecurringBooking.AsTracking()
            .Include(item => item.MarketplaceBooking).ThenInclude(item => item!.PaidByCustomer)
            .Where(item => item.MarketplaceBookingSubscription != null &&
                           item.MarketplaceBookingSubscription.Id == subscriptionId &&
                           item.MarketplaceBooking != null)
            // Prefer the active cycle. When reconciliation is running ahead of the cycle,
            // retain the most recently started bill rather than allowing a future pending
            // bill to hide a confirmed current payment.
            .OrderByDescending(item => !item.DeletedAt.HasValue && item.StartDate <= now &&
                                       (!item.EndDate.HasValue || item.EndDate.Value >= now))
            .ThenByDescending(item => !item.DeletedAt.HasValue)
            .ThenByDescending(item => item.StartDate);

        var recurringBooking = await recurringBookingQuery.FirstOrDefaultAsync(cancellationToken);

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

    private static MarketplacePurchaseHistoryRow ToRow(
        MarketplacePurchaseHistory item,
        IReadOnlyDictionary<(string EntityType, string EntityId), (string Id, string Status)>? latestRefunds = null,
        MarketplacePurchaseHistoryCurrentState? derivedState = null)
    {
        var refund = item.LatestRefundId is not null
            ? (item.LatestRefundId, item.LatestRefundStatus ?? string.Empty)
            : latestRefunds?.GetValueOrDefault((item.SourceType, item.SourceId));
        var paymentStatus = derivedState?.PaymentStatus ?? item.PaymentStatus?.ToPaymentStatus() ?? PaymentStatus.NotSet;
        return new MarketplacePurchaseHistoryRow(
            item.SourceId,
            item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription
                ? MarketplacePurchaseSourceType.Subscription
                : item.SourceType == MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase
                    ? MarketplacePurchaseSourceType.Entitlement
                    : MarketplacePurchaseSourceType.Booking,
            derivedState?.PurchasedAt ?? item.PurchasedAt, derivedState?.ActivityAt ?? item.ActivityAt, item.BookingFrom, item.BookingUntil,
            paymentStatus,
            item.ProductVersionId, item.ProductTitle, item.TotalAmount, item.Currency?.ToCurrency(), item.CustomerId, item.OrganizationId,
            item.DeletedByCustomerId, item.CancellationReason,
            derivedState?.SubscriptionStatus ?? item.SubscriptionStatus?.ToMarketplaceBookingSubscriptionStatus(),
            derivedState?.AutoRenew ?? item.AutoRenew, derivedState?.CancelAtPeriodEnd ?? item.CancelAtPeriodEnd,
            derivedState?.IsDeleted ?? item.IsDeleted, derivedState?.RefundId ?? refund?.Id,
            derivedState?.EntitlementStatus ?? ToNullableEntitlementStatus(item.EntitlementStatus),
            derivedState?.CreditQuantity ?? item.CreditQuantity, item.GrantedQuantity,
            derivedState?.RemainingCreditQuantity ?? item.AvailableQuantity);
    }

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
