using Api.Shared.Services.Models;
using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[GraphQLName("MarketplacePurchaseHistoryEventDetails")]
public sealed class MarketplacePurchaseHistoryEventDetails
{
    public required string Id { get; init; }
    public required string SourceId { get; init; }
    public required MarketplacePurchaseHistoryEligibleSourceType SourceType { get; init; }
    public required MarketplacePurchaseHistoryEventType Type { get; init; }
    public required string Name { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required DateTimeOffset RecordedAt { get; init; }
    public DateTimeOffset? CancellationRequestedAt { get; init; }
    public DateTimeOffset? CancellationEffectiveAt { get; init; }
    public PaymentStatus? PreviousPaymentStatus { get; init; }
    public PaymentStatus? PaymentStatus { get; init; }
    public string? RefundId { get; init; }
    public MarketplaceRefundStatus? PreviousRefundStatus { get; init; }
    public MarketplaceRefundStatus? RefundStatus { get; init; }
    public int? CreditQuantity { get; init; }
    public int? RemainingCreditQuantity { get; init; }
    public decimal? Amount { get; init; }
    public Currency? Currency { get; init; }
    public string? Reason { get; init; }

    // Kept for resolver-local projections that do not have a mapper service in scope.
    // The canonical service-layer mapping is GraphQlMapper.MapTo.
    public static MarketplacePurchaseHistoryEventDetails From(MarketplacePurchaseHistoryEventModel source) => new()
    {
        Id = source.Id,
        SourceId = source.SourceId,
        SourceType = source.SourceType,
        Type = source.Type,
        Name = source.Type switch
        {
            MarketplacePurchaseHistoryEventType.PurchaseCreated => "Purchase created",
            MarketplacePurchaseHistoryEventType.SubscriptionStarted => "Subscription started",
            MarketplacePurchaseHistoryEventType.SubscriptionRenewed => "Subscription renewed",
            MarketplacePurchaseHistoryEventType.CancellationScheduled => "Cancellation scheduled",
            MarketplacePurchaseHistoryEventType.CancellationCompleted => "Cancellation completed",
            MarketplacePurchaseHistoryEventType.EntitlementCreated => "Entitlement created",
            MarketplacePurchaseHistoryEventType.EntitlementExpired => "Entitlement expired",
            MarketplacePurchaseHistoryEventType.CreditsConsumed => "Credits consumed",
            MarketplacePurchaseHistoryEventType.PaymentStateChanged => "Payment state changed",
            MarketplacePurchaseHistoryEventType.RefundStateChanged => "Refund state changed",
            _ => throw new ArgumentOutOfRangeException(nameof(source.Type)),
        },
        OccurredAt = source.OccurredAt,
        RecordedAt = source.RecordedAt,
        CancellationRequestedAt = source.CancellationRequestedAt,
        CancellationEffectiveAt = source.CancellationEffectiveAt,
        PreviousPaymentStatus = source.PreviousPaymentStatus,
        PaymentStatus = source.PaymentStatus,
        RefundId = source.RefundId,
        PreviousRefundStatus = source.PreviousRefundStatus,
        RefundStatus = source.RefundStatus,
        CreditQuantity = source.CreditQuantity,
        RemainingCreditQuantity = source.RemainingCreditQuantity,
        Amount = source.Amount,
        Currency = source.Currency,
        Reason = source.Reason,
    };
}
