using Location.Shared.Models;

namespace Location.Api.Models;

public record LocationAnalytics(
    string Id,
    string Name,
    IReadOnlyList<LocationDesksOccupancyPercentage> DesksOccupancyPercentage,
    IReadOnlyList<LocationDailyBookingsTotal> DailyBookingsTotal,
    IReadOnlyList<LocationRoomsOccupancyPercentage> RoomsOccupancyPercentage,
    IReadOnlyList<ResourceAvailabilitySnapshotReport> ResourceAvailabilitySnapshots);
