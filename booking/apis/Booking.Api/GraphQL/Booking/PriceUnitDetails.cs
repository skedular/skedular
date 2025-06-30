using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("Booking_PriceUnitDetails")]
public class PriceUnitDetails
{
    [GraphQLName("type")] public PriceUnit Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
