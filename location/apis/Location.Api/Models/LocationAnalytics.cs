using Location.Shared.Models;

namespace Location.Api.Models;

public record LocationAnalytics(
    string Id,
    string Name,
    ICollection<LocationDesksOccupancyPercentage> DesksOccupancyPercentage,
    ICollection<LocationDailyBookingsTotal> DailyBookingsTotal,
    ICollection<LocationRoomsOccupancyPercentage> RoomsOccupancyPercentage);
