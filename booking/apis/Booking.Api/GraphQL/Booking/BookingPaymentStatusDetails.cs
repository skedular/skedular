using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingPaymentStatusDetails")]
public class BookingPaymentStatusDetails
{
    [GraphQLName("type")] public BookingPaymentStatus Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
