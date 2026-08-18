using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("RejectMarketplaceRefundInput")]
public class RejectMarketplaceRefundInput
{
    public string Id { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
