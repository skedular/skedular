using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationDailyBookingsTotal")]
public class LocationDailyBookingsTotal
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("total")] public int Total { get; set; }
}
