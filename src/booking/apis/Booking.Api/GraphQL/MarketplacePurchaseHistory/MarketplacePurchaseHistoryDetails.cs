using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[GraphQLName("MarketplacePurchaseHistoryDetails")]
public sealed class MarketplacePurchaseHistoryDetails
{
    public required string Id { get; set; }
    public required string SourceId { get; set; }
    public required MarketplacePurchaseSourceType SourceType { get; set; }
    public required string SourceTypeName { get; set; }
    public required MarketplacePurchaseLifecycleState LifecycleState { get; set; }
    public required string LifecycleStateName { get; set; }
    public required MarketplacePurchaseRenewalState RenewalState { get; set; }
    public required string RenewalStateName { get; set; }
    public required DateTimeOffset PurchasedAt { get; set; }
    public required DateTimeOffset ActivityAt { get; set; }
    public DateTimeOffset? BookingFrom { get; set; }
    public DateTimeOffset? BookingUntil { get; set; }
    public required PaymentStatus PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ProductVersionId { get; set; }
    public string? ProductTitle { get; set; }
    public decimal? TotalAmount { get; set; }
    public Currency? Currency { get; set; }
    public string? CustomerId { get; set; }
    public string? DeletedByCustomerId { get; set; }
    public string? CancellationReason { get; set; }
    public string? RefundId { get; set; }
    public string? BookingId { get; set; }
    public string? EntitlementStatus { get; set; }
    public int CreditQuantity { get; set; }
    public int GrantedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public required bool IsDeleted { get; set; }

    [UseResolverScope]
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
