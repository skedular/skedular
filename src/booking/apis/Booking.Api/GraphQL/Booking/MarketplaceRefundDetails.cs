using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundStatusDetails")]
public class MarketplaceRefundStatusDetails
{
    [GraphQLName("type")] public MarketplaceRefundStatus Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("MarketplaceRefundDetails")]
public class MarketplaceRefundDetails
{
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("localEntityType")] public string LocalEntityType { get; set; } = string.Empty;
    [GraphQLName("localEntityId")] public string LocalEntityId { get; set; } = string.Empty;
    [GraphQLName("status")] public MarketplaceRefundStatusDetails Status { get; set; } = new();
    [GraphQLName("requestedAt")] public DateTimeOffset RequestedAt { get; set; }
    [GraphQLName("referenceTime")] public DateTimeOffset ReferenceTime { get; set; }
    [GraphQLName("refundPercentage")] public int RefundPercentage { get; set; }

    [GraphQLName("appliedRuleMinutesBefore")]
    public int? AppliedRuleMinutesBefore { get; set; }

    [GraphQLName("baseAmount")] public decimal? BaseAmount { get; set; }
    [GraphQLName("refundAmount")] public decimal? RefundAmount { get; set; }
    [GraphQLName("currency")] public CurrencyDetails? Currency { get; set; }
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
    [GraphQLName("reason")] public string? Reason { get; set; }
    [GraphQLName("accountingProvider")] public string? AccountingProvider { get; set; }
    [GraphQLName("externalRefundId")] public string? ExternalRefundId { get; set; }
    [GraphQLName("externalRefundNumber")] public string? ExternalRefundNumber { get; set; }
    [GraphQLName("lastProcessedAt")] public DateTimeOffset? LastProcessedAt { get; set; }
    [GraphQLName("lastError")] public string? LastError { get; set; }
    [GraphQLName("paymentProvider")] public string? PaymentProvider { get; set; }

    [GraphQLName("externalPaymentRefundId")]
    public string? ExternalPaymentRefundId { get; set; }

    [GraphQLName("paymentRefundStatus")] public string? PaymentRefundStatus { get; set; }

    [GraphQLName("paymentRefundLastProcessedAt")]
    public DateTimeOffset? PaymentRefundLastProcessedAt { get; set; }

    [GraphQLName("paymentRefundLastError")]
    public string? PaymentRefundLastError { get; set; }


    [GraphQLName("canProcessInXero")] public bool CanProcessInXero { get; set; }
    [GraphQLName("events")] public IEnumerable<MarketplaceRefundEventDetails> Events { get; set; } = [];
    [GraphQLName("requestedByCustomerId")] public string? RequestedByCustomerId { get; set; }

    [GraphQLName("requestedByCustomerName")]
    public string? RequestedByCustomerName { get; set; }

    [GraphQLName("xeroProcessingBlockedReason")]
    public string? XeroProcessingBlockedReason { get; set; }

    public string RefundKind { get; set; } = string.Empty;
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
    public string? ReconciliationStatus { get; set; }
    public IReadOnlyList<MarketplaceRefundPaymentAllocationDetails> PaymentAllocations { get; set; } = [];
}

[GraphQLName("MarketplaceRefundPaymentAllocationDetails")]
public class MarketplaceRefundPaymentAllocationDetails
{
    public string SourcePaymentProvider { get; set; } = string.Empty;
    public string SourcePaymentReference { get; set; } = string.Empty;
    public decimal SourcePaymentAmount { get; set; }
    public decimal AllocatedRefundAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
