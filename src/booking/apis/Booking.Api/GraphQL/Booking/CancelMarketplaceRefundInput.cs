using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CancelMarketplaceRefundInput")]
public class CancelMarketplaceRefundInput
{
    public string Id { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
