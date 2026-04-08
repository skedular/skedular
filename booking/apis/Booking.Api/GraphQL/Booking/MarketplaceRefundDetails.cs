using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundStatusDetails")]
public class MarketplaceRefundStatusDetails
{
    [GraphQLName("type")] public string Type { get; set; } = string.Empty;
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
    [GraphQLName("canProcessInXero")] public bool CanProcessInXero { get; set; }
    [GraphQLName("events")] public ICollection<MarketplaceRefundEventDetails> Events { get; set; } = [];
    [GraphQLName("requestedByCustomerId")] public string? RequestedByCustomerId { get; set; }

    [GraphQLName("requestedByCustomerName")]
    public string? RequestedByCustomerName { get; set; }

    [GraphQLName("xeroProcessingBlockedReason")]
    public string? XeroProcessingBlockedReason { get; set; }
}
