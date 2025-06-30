using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingTypeDetails")]
public class BookingTypeDetails
{
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
