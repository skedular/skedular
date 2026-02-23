using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingFrequencyDetails")]
public class BookingFrequencyDetails
{
    [GraphQLName("frequency")] public BookingFrequency Frequency { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
