using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("BookingPaymentMethodTypeDetails")]
public class BookingPaymentMethodTypeDetails
{
    [GraphQLName("type")] public BookingPaymentMethod Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
