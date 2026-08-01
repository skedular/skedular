using Api.Shared.Services.Models;

namespace Booking.Shared.Models;

public sealed class MarketplaceRefundPreviewModel
{
    public MarketplaceRefundEntityType LocalEntityType { get; init; }
    public string LocalEntityId { get; init; } = string.Empty;
    public DateTimeOffset RequestedAt { get; init; }
    public DateTimeOffset ReferenceTime { get; init; }
    public bool IsRefundable { get; init; }
    public int RefundPercentage { get; init; }
    public int? AppliedRuleMinutesBefore { get; init; }
    public decimal? BaseAmount { get; init; }
    public decimal? RefundAmount { get; init; }
    public Currency? Currency { get; init; }
}

public sealed class MarketplaceRefundReadModel
{
    public string Id { get; init; } = string.Empty;
    public MarketplaceRefundEntityType LocalEntityType { get; init; }
    public string LocalEntityId { get; init; } = string.Empty;
    public MarketplaceRefundStatus Status { get; init; }
    public DateTimeOffset RequestedAt { get; init; }
    public DateTimeOffset ReferenceTime { get; init; }
    public int RefundPercentage { get; init; }
    public int? AppliedRuleMinutesBefore { get; init; }
    public decimal? BaseAmount { get; init; }
    public decimal? RefundAmount { get; init; }
    public Currency? Currency { get; init; }
    public string? Reason { get; init; }
    public string? AccountingProvider { get; init; }
    public string? ExternalRefundId { get; init; }
    public string? ExternalRefundNumber { get; init; }
    public DateTimeOffset? LastProcessedAt { get; init; }
    public string? LastError { get; set; }
    public string? PaymentProvider { get; init; }
    public string? ExternalPaymentRefundId { get; init; }
    public string? PaymentRefundStatus { get; init; }
    public DateTimeOffset? PaymentRefundLastProcessedAt { get; init; }
    public string? PaymentRefundLastError { get; init; }
    public bool CanProcessInXero { get; set; }
    public IReadOnlyList<MarketplaceRefundEventModel> Events { get; set; } = [];
    public string? RequestedByCustomerId { get; init; }
    public string? RequestedByCustomerName { get; set; }
    public string? XeroProcessingBlockedReason { get; set; }
    public MarketplaceRefundKind RefundKind { get; init; }
    public string IdempotencyKey { get; init; } = string.Empty;
    public string? PolicySnapshotJson { get; init; }
    public string? CalculationResultJson { get; init; }
    public string? TimezoneId { get; init; }
    public int RetryCount { get; init; }
    public DateTimeOffset? ApprovedAt { get; init; }
    public string? ApprovedByCustomerId { get; init; }
    public DateTimeOffset? RejectedAt { get; init; }
    public string? RejectedByCustomerId { get; init; }
    public string? RejectionReason { get; init; }
    public DateTimeOffset? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public string? BankTransferReference { get; init; }
    public DateTimeOffset? BankTransferSentAt { get; init; }
    public DateTimeOffset? ReconciledAt { get; init; }
    public MarketplaceExternalRefundReconciliationStatus? ReconciliationStatus { get; init; }
    public MarketplaceRefundStatus? LastNotificationStatus { get; init; }
    public bool PostPayoutRefund { get; init; }
    public string? StripeRefundPath { get; init; }
    public string? StripeAccountId { get; init; }
    public string? StripeChargeType { get; init; }
    public string? StripeTransferId { get; init; }
    public string? StripeChargeId { get; init; }
    public string? StripePaymentIntentId { get; init; }
    public DateTimeOffset? StripeRefundPathSelectedAt { get; init; }
    public string? ReconciliationLeaseOwner { get; init; }
    public DateTimeOffset? ReconciliationLeaseExpiresAt { get; init; }
    public DateTimeOffset? ReconciliationLeaseRenewedAt { get; init; }
    public IReadOnlyList<MarketplaceRefundPaymentAllocationModel> PaymentAllocations { get; init; } = [];
}

public sealed class MarketplaceRefundEventModel
{
    public string Id { get; init; } = string.Empty;
    public MarketplaceRefundEventType EventType { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public decimal? RefundAmount { get; init; }
    public Currency? Currency { get; init; }
    public string? Reason { get; init; }
    public string? AccountingProvider { get; init; }
    public string? ExternalRefundId { get; init; }
    public string? ExternalRefundNumber { get; init; }
    public string? LastError { get; init; }
    public string? ActorCustomerId { get; init; }
    public string? ActorName { get; set; }
    public MarketplaceRefundStatus? PreviousStatus { get; init; }
    public MarketplaceRefundStatus? NewStatus { get; init; }
    public string? CorrelationId { get; init; }
}

public sealed class MarketplaceRefundPaymentAllocationModel
{
    public string SourcePaymentProvider { get; init; } = string.Empty;
    public string SourcePaymentReference { get; init; } = string.Empty;
    public decimal SourcePaymentAmount { get; init; }
    public decimal AllocatedRefundAmount { get; init; }
    public Currency Currency { get; init; }
}
