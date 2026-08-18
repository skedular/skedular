using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ApproveMarketplaceRefundInput")]
public class ApproveMarketplaceRefundInput
{
    public string Id { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
