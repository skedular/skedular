using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;

namespace Booking.Api.Services;

public interface IMarketplacePurchaseHistoryService
{
    Task<IReadOnlyList<MarketplacePurchaseHistoryEventModel>> GetEventsAsync(
        MarketplacePurchaseHistoryEligibleSourceType sourceType,
        string sourceId,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplacePurchaseHistoryEntry>>, int)> GetPaginatedAsync(
        PaginationInputParam paginationInputParam,
        string? organizationCustomDomain,
        MarketplacePurchaseHistorySearchCriteria searchCriteria,
        IReadOnlyList<MarketplacePurchaseHistoryOrder>? orderBy,
        CancellationToken cancellationToken);
}

public class MarketplacePurchaseHistoryService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceBookingService marketplaceBookingService,
    IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
    IEntitlementPurchaseReadService entitlementPurchaseReadService,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILogger<MarketplacePurchaseHistoryService> logger) : IMarketplacePurchaseHistoryService
{
    public async Task<IReadOnlyList<MarketplacePurchaseHistoryEventModel>> GetEventsAsync(
        MarketplacePurchaseHistoryEligibleSourceType sourceType,
        string sourceId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceId);
        var persistedSourceType = sourceType switch
        {
            MarketplacePurchaseHistoryEligibleSourceType.Subscription => MarketplacePurchaseHistorySourceTypeConstants
                .MarketplaceBookingSubscription,
            MarketplacePurchaseHistoryEligibleSourceType.Entitlement => MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
            _ => throw new ArgumentOutOfRangeException(nameof(sourceType)),
        };

        switch (sourceType)
        {
            case MarketplacePurchaseHistoryEligibleSourceType.Subscription:
                await marketplaceBookingSubscriptionService.GetByIdAsync(sourceId, cancellationToken);
                break;
            case MarketplacePurchaseHistoryEligibleSourceType.Entitlement:
                var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
                var purchase = await entitlementPurchaseReadService.GetAuthorizedAsync(sourceId, customerId, cancellationToken);
                if (purchase is null)
                {
                    throw new UnauthorizedAccessException();
                }

                break;
        }

        return await repositoryFactory.MarketplacePurchaseHistoryRepository.GetEventsAsync(
            persistedSourceType, sourceId, cancellationToken);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplacePurchaseHistoryEntry>>, int)> GetPaginatedAsync(
        PaginationInputParam paginationInputParam,
        string? organizationCustomDomain,
        MarketplacePurchaseHistorySearchCriteria searchCriteria,
        IReadOnlyList<MarketplacePurchaseHistoryOrder>? orderBy,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(searchCriteria);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            throw new InvalidOperationException("Purchase history must be scoped to an organization.");
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            null, organizationCustomDomain, false, false, cancellationToken) ?? throw new OrganizationNotFound();
        if (searchCriteria.IncludeMineOnly)
        {
            searchCriteria = searchCriteria with
            {
                CustomerId = customerId,
            };
        }
        else if (!await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var (paginatedInfo, rows, totalCount) = await repositoryFactory.MarketplacePurchaseHistoryRepository
            .GetPaginatedRowsAsync(paginationInputParam, searchCriteria with
                {
                    OrganizationCustomDomain = organization.CustomDomain,
                }, orderBy,
                cancellationToken);
        logger.LogInformation(
            "Loading retained marketplace purchase history for organization {OrganizationCustomDomain}: {PurchaseCount} scalar purchase rows",
            organizationCustomDomain,
            totalCount);
        var entries = new List<Edge<MarketplacePurchaseHistoryEntry>>(rows.Count);
        foreach (var edge in rows)
        {
            var row = edge.Node;
            var payment = row.PaymentStatus;
            var lifecycle = row.IsDeleted
                ? MarketplacePurchaseLifecycleState.Deleted
                : row.SourceType == MarketplacePurchaseSourceType.Subscription
                    ? GetSubscriptionLifecycle(row, logger)
                    : row.SourceType == MarketplacePurchaseSourceType.Entitlement
                        ? GetEntitlementLifecycle(row)
                        : MarketplacePurchaseLifecycleState.Active;
            var bookingId = row.SourceType == MarketplacePurchaseSourceType.Booking
                ? await marketplaceBookingService.GetBookingIdAsync(row.Id, cancellationToken)
                : null;
            var paymentMethod = row.SourceType == MarketplacePurchaseSourceType.Entitlement
                ? (await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(row.Id, cancellationToken))?.PaymentMethod
                : null;

            entries.Add(new Edge<MarketplacePurchaseHistoryEntry>(new MarketplacePurchaseHistoryEntry(
                    row.Id,
                    row.SourceType,
                    lifecycle,
                    row.SourceType == MarketplacePurchaseSourceType.Subscription && row.AutoRenew && !row.CancelAtPeriodEnd
                        ? MarketplacePurchaseRenewalState.Renews
                        : row.SourceType == MarketplacePurchaseSourceType.Subscription
                            ? MarketplacePurchaseRenewalState.DoesNotRenew
                            : MarketplacePurchaseRenewalState.NotApplicable,
                    row.PurchasedAt,
                    row.ActivityAt,
                    row.BookingFrom,
                    row.BookingUntil,
                    payment,
                    row.ProductVersionId,
                    row.ProductTitle,
                    row.TotalAmount,
                    row.Currency,
                    row.CustomerId,
                    row.IsDeleted,
                    row.IsDeleted ? row.DeletedByCustomerId : null,
                    row.CancellationReason,
                    row.RefundId,
                    bookingId, null, null, row.EntitlementStatus, row.CreditQuantity, row.GrantedQuantity, row.AvailableQuantity, paymentMethod),
                edge.Cursor));
        }

        return (paginatedInfo, entries, totalCount);
    }

    private static MarketplacePurchaseLifecycleState GetSubscriptionLifecycle(
        MarketplacePurchaseHistoryRow row,
        ILogger<MarketplacePurchaseHistoryService> logger)
    {
        if (row.SubscriptionStatus is null)
        {
            logger.LogWarning(
                "Marketplace purchase {PurchaseId} has no subscription status; returning pending lifecycle for legacy data",
                row.Id);
            return MarketplacePurchaseLifecycleState.Pending;
        }

        return row.SubscriptionStatus.Value switch
        {
            MarketplaceBookingSubscriptionStatus.Cancelled => MarketplacePurchaseLifecycleState.Cancelled,
            MarketplaceBookingSubscriptionStatus.Expired => MarketplacePurchaseLifecycleState.Expired,
            MarketplaceBookingSubscriptionStatus.RenewalFailed => MarketplacePurchaseLifecycleState.PaymentFailed,
            MarketplaceBookingSubscriptionStatus.Active => MarketplacePurchaseLifecycleState.Active,
            _ => MarketplacePurchaseLifecycleState.Pending,
        };
    }

    private static MarketplacePurchaseLifecycleState GetEntitlementLifecycle(MarketplacePurchaseHistoryRow row) =>
        row.PaymentStatus switch
        {
            PaymentStatus.Rejected => MarketplacePurchaseLifecycleState.PaymentFailed,
            PaymentStatus.Expired => MarketplacePurchaseLifecycleState.Expired,
            PaymentStatus.Pending => MarketplacePurchaseLifecycleState.Pending,
            PaymentStatus.Confirmed when row.EntitlementStatus == EntitlementStatus.Active => MarketplacePurchaseLifecycleState.Active,
            _ when row.EntitlementStatus == EntitlementStatus.Cancelled => MarketplacePurchaseLifecycleState.Cancelled,
            _ when row.EntitlementStatus == EntitlementStatus.Expired => MarketplacePurchaseLifecycleState.Expired,
            _ => MarketplacePurchaseLifecycleState.Pending,
        };
}
