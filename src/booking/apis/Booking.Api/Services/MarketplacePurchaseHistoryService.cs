using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;

namespace Booking.Api.Services;

public interface IMarketplacePurchaseHistoryService
{
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
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILogger<MarketplacePurchaseHistoryService> logger) : IMarketplacePurchaseHistoryService
{
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
        if (!await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
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
            var payment = row.PaymentStatus.ToPaymentStatus();
            var lifecycle = row.IsDeleted
                ? MarketplacePurchaseLifecycleState.Deleted
                : row.SourceType == MarketplacePurchaseSourceType.Subscription
                    ? GetSubscriptionLifecycle(row, logger)
                    : MarketplacePurchaseLifecycleState.Active;
            var bookingId = row.SourceType == MarketplacePurchaseSourceType.Booking
                ? await marketplaceBookingService.GetBookingIdAsync(row.Id, cancellationToken)
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
                row.Currency is null ? null : Enum.TryParse<Currency>(row.Currency, true, out var currency) ? currency : null,
                row.CustomerId,
                row.IsDeleted,
                row.IsDeleted ? row.DeletedByCustomerId : null,
                row.CancellationReason,
                row.RefundId,
                bookingId), edge.Cursor));
        }

        return (paginatedInfo, entries, totalCount);
    }

    private static MarketplacePurchaseLifecycleState GetSubscriptionLifecycle(
        MarketplacePurchaseHistoryRow row,
        ILogger<MarketplacePurchaseHistoryService> logger)
    {
        if (string.IsNullOrWhiteSpace(row.SubscriptionStatus))
        {
            logger.LogWarning(
                "Marketplace purchase {PurchaseId} has no subscription status; returning pending lifecycle for legacy data",
                row.Id);
            return MarketplacePurchaseLifecycleState.Pending;
        }

        MarketplaceBookingSubscriptionStatus subscriptionStatus;
        try
        {
            subscriptionStatus = row.SubscriptionStatus.ToMarketplaceBookingSubscriptionStatus();
        }
        catch (ArgumentOutOfRangeException)
        {
            logger.LogWarning(
                "Marketplace purchase {PurchaseId} has an unknown subscription status {SubscriptionStatus}; returning pending lifecycle",
                row.Id,
                row.SubscriptionStatus);
            return MarketplacePurchaseLifecycleState.Pending;
        }

        return subscriptionStatus switch
        {
            MarketplaceBookingSubscriptionStatus.Cancelled => MarketplacePurchaseLifecycleState.Cancelled,
            MarketplaceBookingSubscriptionStatus.Expired => MarketplacePurchaseLifecycleState.Expired,
            MarketplaceBookingSubscriptionStatus.RenewalFailed => MarketplacePurchaseLifecycleState.PaymentFailed,
            MarketplaceBookingSubscriptionStatus.Active => MarketplacePurchaseLifecycleState.Active,
            _ => MarketplacePurchaseLifecycleState.Pending,
        };
    }
}
