using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingCheckoutSessionDetails")]
public class BookingCheckoutSessionDetails
{
    [GraphQLName("uniqueId")]
    public required string UniqueId { get; set; }

    [GraphQLName("checkoutUrl")]
    public string CheckoutUrl { get; set; } = string.Empty;
}
