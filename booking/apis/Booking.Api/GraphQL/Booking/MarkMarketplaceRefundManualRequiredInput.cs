using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarkMarketplaceRefundManualRequiredInput")]
public class MarkMarketplaceRefundManualRequiredInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("reason")] public string? Reason { get; set; }
}
