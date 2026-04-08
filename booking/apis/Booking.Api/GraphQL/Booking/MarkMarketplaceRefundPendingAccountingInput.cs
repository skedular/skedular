using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarkMarketplaceRefundPendingAccountingInput")]
public class MarkMarketplaceRefundPendingAccountingInput
{
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("refundAmount")] public decimal? RefundAmount { get; set; }
    [GraphQLName("reason")] public string? Reason { get; set; }
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
