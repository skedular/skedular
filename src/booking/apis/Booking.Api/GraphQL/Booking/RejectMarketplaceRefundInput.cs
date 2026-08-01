using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("RejectMarketplaceRefundInput")]
public class RejectMarketplaceRefundInput
{
    public string Id { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string? ClientMutationId { get; set; }
}
