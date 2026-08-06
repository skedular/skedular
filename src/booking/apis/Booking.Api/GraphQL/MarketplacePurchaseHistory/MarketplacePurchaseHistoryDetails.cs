using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[GraphQLName("MarketplacePurchaseHistoryDetails")]
public sealed class MarketplacePurchaseHistoryDetails
{
    public required string Id { get; init; }
    public required string SourceId { get; init; }
    public required MarketplacePurchaseSourceType SourceType { get; init; }
    public required string SourceTypeName { get; init; }
    public required MarketplacePurchaseLifecycleState LifecycleState { get; init; }
    public required string LifecycleStateName { get; init; }
    public required MarketplacePurchaseRenewalState RenewalState { get; init; }
    public required string RenewalStateName { get; init; }
    public required DateTimeOffset PurchasedAt { get; init; }
    public required DateTimeOffset ActivityAt { get; init; }
    public DateTimeOffset? BookingFrom { get; init; }
    public DateTimeOffset? BookingUntil { get; init; }
    public required PaymentStatus PaymentStatus { get; init; }
    public string? ProductVersionId { get; init; }
    public string? ProductTitle { get; init; }
    public decimal? TotalAmount { get; init; }
    public Currency? Currency { get; init; }
    public string? CustomerId { get; init; }
    public string? DeletedByCustomerId { get; init; }
    public string? CancellationReason { get; init; }
    public string? RefundId { get; init; }
    public string? BookingId { get; init; }
    public required bool IsDeleted { get; init; }

    public async Task<MarketplaceRefundDetails?> GetRefund(
        [Service]
        IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(RefundId))
        {
            return null;
        }

        try
        {
            var refund = await marketplaceRefundReadService.GetByIdAsync(RefundId, cancellationToken);
            return refund is null ? null : graphQlMapper.MapTo(refund);
        }
        catch (UnauthorizedAccessException)
        {
            // A history row remains useful when its refund projection is no longer
            // readable. Treat the nested field as partial data instead of failing
            // the entire purchase-history page.
            return null;
        }
    }
}
