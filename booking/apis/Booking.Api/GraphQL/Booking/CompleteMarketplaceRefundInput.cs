using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CompleteMarketplaceRefundInput")]
public class CompleteMarketplaceRefundInput
{
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("reason")] public string? Reason { get; set; }
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
