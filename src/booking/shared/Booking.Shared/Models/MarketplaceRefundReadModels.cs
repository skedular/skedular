using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public sealed class MarketplaceRefundPreviewModel
{
    public MarketplaceRefundEntityType LocalEntityType { get; set; }
    public string LocalEntityId { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset ReferenceTime { get; set; }
    public bool IsRefundable { get; set; }
    public int RefundPercentage { get; set; }
    public int? AppliedRuleMinutesBefore { get; set; }
    public decimal? BaseAmount { get; set; }
    public decimal? RefundAmount { get; set; }
    public Currency? Currency { get; set; }
}

public sealed class MarketplaceRefundReadModel : ModelBase
{
    public MarketplaceRefundEntityType LocalEntityType { get; set; }
    public string LocalEntityId { get; set; } = string.Empty;
    public MarketplaceRefundStatus Status { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset ReferenceTime { get; set; }
    public int RefundPercentage { get; set; }
    public int? AppliedRuleMinutesBefore { get; set; }
    public decimal? BaseAmount { get; set; }
    public decimal? RefundAmount { get; set; }
    public Currency? Currency { get; set; }
    public string? Reason { get; set; }
    public string? AccountingProvider { get; set; }
    public string? ExternalRefundId { get; set; }
    public string? ExternalRefundNumber { get; set; }
    public DateTimeOffset? LastProcessedAt { get; set; }
    public string? LastError { get; set; }
    public string? PaymentProvider { get; set; }
    public string? ExternalPaymentRefundId { get; set; }
    public string? PaymentRefundStatus { get; set; }
    public DateTimeOffset? PaymentRefundLastProcessedAt { get; set; }
    public string? PaymentRefundLastError { get; set; }
    public bool CanProcessInXero { get; set; }
    public IReadOnlyList<MarketplaceRefundEventModel> Events { get; set; } = [];
    public string? RequestedByCustomerId { get; set; }
    public string? RequestedByCustomerName { get; set; }
    public string? XeroProcessingBlockedReason { get; set; }
    public MarketplaceRefundKind RefundKind { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? PolicySnapshotJson { get; set; }
    public string? CalculationResultJson { get; set; }
    public string? TimezoneId { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string? ApprovedByCustomerId { get; set; }
    public DateTimeOffset? RejectedAt { get; set; }
    public string? RejectedByCustomerId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? BankTransferReference { get; set; }
    public DateTimeOffset? BankTransferSentAt { get; set; }
    public DateTimeOffset? ReconciledAt { get; set; }
    public MarketplaceExternalRefundReconciliationStatus? ReconciliationStatus { get; set; }
    public MarketplaceRefundStatus? LastNotificationStatus { get; set; }
    public bool PostPayoutRefund { get; set; }
    public string? StripeRefundPath { get; set; }
    public string? StripeAccountId { get; set; }
    public string? StripeChargeType { get; set; }
    public string? StripeTransferId { get; set; }
    public string? StripeChargeId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTimeOffset? StripeRefundPathSelectedAt { get; set; }
    public string? ReconciliationLeaseOwner { get; set; }
    public DateTimeOffset? ReconciliationLeaseExpiresAt { get; set; }
    public DateTimeOffset? ReconciliationLeaseRenewedAt { get; set; }
    public IReadOnlyList<MarketplaceRefundPaymentAllocationModel> PaymentAllocations { get; set; } = [];
}

public sealed class MarketplaceRefundEventModel : ModelBase
{
    public MarketplaceRefundEventType EventType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public decimal? RefundAmount { get; set; }
    public Currency? Currency { get; set; }
    public string? Reason { get; set; }
    public string? AccountingProvider { get; set; }
    public string? ExternalRefundId { get; set; }
    public string? ExternalRefundNumber { get; set; }
    public string? LastError { get; set; }
    public string? ActorCustomerId { get; set; }
    public string? ActorName { get; set; }
    public MarketplaceRefundStatus? PreviousStatus { get; set; }
    public MarketplaceRefundStatus? NewStatus { get; set; }
    public string? CorrelationId { get; set; }
}

public sealed class MarketplaceRefundPaymentAllocationModel
{
    public string SourcePaymentProvider { get; set; } = string.Empty;
    public string SourcePaymentReference { get; set; } = string.Empty;
    public decimal SourcePaymentAmount { get; set; }
    public decimal AllocatedRefundAmount { get; set; }
    public Currency Currency { get; set; }
}
