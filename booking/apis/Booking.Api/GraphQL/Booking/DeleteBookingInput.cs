using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("DeleteBookingInput")]
public class DeleteBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}
