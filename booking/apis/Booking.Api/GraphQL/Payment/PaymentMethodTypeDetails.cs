using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("PaymentMethodTypeDetails")]
public class PaymentMethodTypeDetails
{
    [GraphQLName("type")] public PaymentMethod Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
