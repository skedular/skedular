using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ProcessMarketplaceRefundInXeroInput")]
public class ProcessMarketplaceRefundInXeroInput
{
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
