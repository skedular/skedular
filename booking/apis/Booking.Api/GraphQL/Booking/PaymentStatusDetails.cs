using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("PaymentStatusDetails")]
public class PaymentStatusDetails
{
    [GraphQLName("type")] public PaymentStatus Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
