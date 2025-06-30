using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingStatusDetails")]
public class BookingStatusDetails
{
    [GraphQLName("type")] public BookingStatus Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
