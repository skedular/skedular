using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("DeletePrivateRecurringBookingInput")]
public class DeletePrivateRecurringBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}
