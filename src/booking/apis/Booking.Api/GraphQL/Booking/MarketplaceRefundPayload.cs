using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundPayload")]
public class MarketplaceRefundPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("marketplaceRefund")]
    public MarketplaceRefundDetails MarketplaceRefund { get; set; } = new();
}
