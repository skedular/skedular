using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingRecurrenceEndTypeDetails")]
public class BookingRecurrenceEndTypeDetails
{
    [GraphQLName("endType")] public RecurringBookingEndType EndType { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
