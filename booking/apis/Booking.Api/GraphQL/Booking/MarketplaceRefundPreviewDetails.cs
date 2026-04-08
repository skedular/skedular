using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundPreviewDetails")]
public class MarketplaceRefundPreviewDetails
{
    [GraphQLName("localEntityType")] public string LocalEntityType { get; set; } = string.Empty;
    [GraphQLName("localEntityId")] public string LocalEntityId { get; set; } = string.Empty;
    [GraphQLName("requestedAt")] public DateTimeOffset RequestedAt { get; set; }
    [GraphQLName("referenceTime")] public DateTimeOffset ReferenceTime { get; set; }
    [GraphQLName("isRefundable")] public bool IsRefundable { get; set; }
    [GraphQLName("refundPercentage")] public int RefundPercentage { get; set; }

    [GraphQLName("appliedRuleMinutesBefore")]
    public int? AppliedRuleMinutesBefore { get; set; }

    [GraphQLName("baseAmount")] public decimal? BaseAmount { get; set; }
    [GraphQLName("refundAmount")] public decimal? RefundAmount { get; set; }
    [GraphQLName("currency")] public CurrencyDetails? Currency { get; set; }
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
}
