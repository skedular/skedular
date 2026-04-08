using Booking.Api.Services;
using Booking.Shared.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[QueryType]
public class MarketplaceRefundRootQuery
{
    [UseResolverScope]
    public Task<MarketplaceRefundPreviewDetails> MarketplaceBookingRefundPreviewAsync(
        string bookingId,
        [Service] IMarketplaceRefundPreviewService marketplaceRefundPreviewService,
        CancellationToken cancellationToken) =>
        marketplaceRefundPreviewService.GetByBookingIdAsync(bookingId, cancellationToken);

    [UseResolverScope]
    public Task<MarketplaceRefundPreviewDetails> MarketplaceBookingSubscriptionRefundPreviewAsync(
        string subscriptionId,
        [Service] IMarketplaceRefundPreviewService marketplaceRefundPreviewService,
        CancellationToken cancellationToken) =>
        marketplaceRefundPreviewService.GetByMarketplaceBookingSubscriptionIdAsync(subscriptionId, cancellationToken);

    [UseResolverScope]
    public Task<MarketplaceRefundDetails?> MarketplaceRefundAsync(
        string id,
        [Service] IMarketplaceRefundReadService marketplaceRefundReadService,
        CancellationToken cancellationToken) =>
        marketplaceRefundReadService.GetByIdAsync(id, cancellationToken);

    [UseResolverScope]
    public Task<ICollection<MarketplaceRefundDetails>> MarketplaceRefundsAsync(
        string organizationCustomDomain,
        ICollection<string>? statuses,
        [Service] IMarketplaceRefundReadService marketplaceRefundReadService,
        CancellationToken cancellationToken) =>
        marketplaceRefundReadService.GetByOrganizationCustomDomainAsync(organizationCustomDomain, statuses, cancellationToken);

    [UseResolverScope]
    public ICollection<MarketplaceRefundStatusDetails> MarketplaceRefundStatuses() =>
    [
        new()
        {
            Type = MarketplaceRefundStatusConstants.Requested, Name = MarketplaceRefundStatusConstants.Requested.ToMarketplaceRefundStatusName()
        },
        new()
        {
            Type = MarketplaceRefundStatusConstants.PendingAccounting,
            Name = MarketplaceRefundStatusConstants.PendingAccounting.ToMarketplaceRefundStatusName()
        },
        new()
        {
            Type = MarketplaceRefundStatusConstants.ManualRequired,
            Name = MarketplaceRefundStatusConstants.ManualRequired.ToMarketplaceRefundStatusName()
        },
        new()
        {
            Type = MarketplaceRefundStatusConstants.ManualCompleted,
            Name = MarketplaceRefundStatusConstants.ManualCompleted.ToMarketplaceRefundStatusName()
        },
        new()
        {
            Type = MarketplaceRefundStatusConstants.Completed, Name = MarketplaceRefundStatusConstants.Completed.ToMarketplaceRefundStatusName()
        },
        new() { Type = MarketplaceRefundStatusConstants.Failed, Name = MarketplaceRefundStatusConstants.Failed.ToMarketplaceRefundStatusName() }
    ];
}
