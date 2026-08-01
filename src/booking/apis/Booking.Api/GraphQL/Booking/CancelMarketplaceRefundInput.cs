using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CancelMarketplaceRefundInput")]
public class CancelMarketplaceRefundInput
{
    public string Id { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string? ClientMutationId { get; set; }
}
