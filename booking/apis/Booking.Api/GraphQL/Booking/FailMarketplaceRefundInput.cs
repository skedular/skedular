using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("FailMarketplaceRefundInput")]
public class FailMarketplaceRefundInput
{
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("reason")] public string? Reason { get; set; }
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
