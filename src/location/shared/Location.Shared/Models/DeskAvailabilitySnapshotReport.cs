using Enterprise.Shared.Models;
using Location.Shared.Database.Entities;

namespace Location.Shared.Models;

public class ResourceAvailabilitySnapshotReport : ModelBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public int AvailableCount { get; set; }
    public int UnavailableCount { get; set; }
    public int BookedCount { get; set; }
    public IReadOnlyList<string> AvailableResourceNames { get; set; } = [];
    public IReadOnlyList<string> UnavailableResourceNames { get; set; } = [];
    public IReadOnlyList<string> BookedResourceNames { get; set; } = [];

    public static ResourceAvailabilitySnapshotReport FromSnapshots(
        DateTimeOffset date,
        string resourceType,
        IReadOnlyList<DailyResourceAvailabilitySnapshot> snapshots) =>
        new()
        {
            Date = date,
            ResourceType = resourceType,
            AvailableCount = snapshots.Count(s => s.Classification == ResourceAvailabilityClassificationConstants.Available),
            UnavailableCount = snapshots.Count(s => s.Classification == ResourceAvailabilityClassificationConstants.Unavailable),
            BookedCount = snapshots.Count(item => item.Classification == ResourceAvailabilityClassificationConstants.Booked),
            AvailableResourceNames =
                snapshots
                    .Where(item => item.Classification == ResourceAvailabilityClassificationConstants.Available)
                    .Select(s => s.Resource.Name)
                    .ToList(),
            UnavailableResourceNames =
                snapshots
                    .Where(s => s.Classification == ResourceAvailabilityClassificationConstants.Unavailable)
                    .Select(s => s.Resource.Name)
                    .ToList(),
            BookedResourceNames = snapshots
                .Where(s => s.Classification == ResourceAvailabilityClassificationConstants.Booked)
                .Select(s => s.Resource.Name)
                .ToList(),
        };
}
