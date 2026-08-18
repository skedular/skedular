using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("RetryMarketplaceRefundInput")]
public class RetryMarketplaceRefundInput
{
    public string Id { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
