using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("DayOfWeekDetails")]
public class DayOfWeekDetails
{
    [GraphQLName("dayOfWeek")] public DayOfWeek DayOfWeek { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
