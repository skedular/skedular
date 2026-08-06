using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("DeletePrivateBookingInput")]
public class DeletePrivateBookingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public required string Id { get; set; }
}
