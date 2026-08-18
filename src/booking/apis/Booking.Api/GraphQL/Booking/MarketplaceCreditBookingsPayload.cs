using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceCreditBookingsPayload")]
public class MarketplaceCreditBookingsPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("bookings")]
    public IReadOnlyList<BookingDetails> Bookings { get; set; } = [];
}
