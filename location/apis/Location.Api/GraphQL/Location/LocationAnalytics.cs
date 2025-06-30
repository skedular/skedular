using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationAnalytics")]
public class LocationAnalytics
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("desksOccupancyPercentage")]
    public IEnumerable<DesksOccupancyPercentage> DesksOccupancyPercentage { get; set; } = [];

    [GraphQLName("roomsOccupancyPercentage")]
    public IEnumerable<RoomsOccupancyPercentage> RoomsOccupancyPercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public IEnumerable<LocationDailyBookingsTotal> DailyBookingsTotals { get; set; } = [];
}
