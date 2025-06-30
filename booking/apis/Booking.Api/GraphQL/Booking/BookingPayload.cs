using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingPayload")]
public class BookingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("booking")] public BookingDetails Booking { get; set; } = new();
}
