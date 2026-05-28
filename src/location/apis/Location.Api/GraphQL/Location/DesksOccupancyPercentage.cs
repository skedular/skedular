using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("DesksOccupancyPercentage")]
public class DesksOccupancyPercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("percentage")] public float Percentage { get; set; }
}
