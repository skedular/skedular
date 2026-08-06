using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("RoomsOccupancyPercentage")]
public class RoomsOccupancyPercentage
{
    [GraphQLName("date")]
    public DateTimeOffset Date { get; set; }

    [GraphQLName("percentage")]
    public float Percentage { get; set; }
}
