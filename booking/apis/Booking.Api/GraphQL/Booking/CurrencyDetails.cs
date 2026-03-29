using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CurrencyDetails")]
public class CurrencyDetails
{
    [GraphQLName("type")] public Currency Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
