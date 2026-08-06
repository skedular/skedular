using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingChannelDetails")]
public class BookingChannelDetails
{
    [GraphQLName("channel")]
    public BookingChannel Channel { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
