using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("DeleteMarketplaceBookingInput")]
public class DeleteMarketplaceBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}
