using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IDailyResourceAvailabilitySnapshotRepository : IRepository<DailyResourceAvailabilitySnapshot>
{
    DailyResourceAvailabilitySnapshot Add(DailyResourceAvailabilitySnapshot snapshot);
    Task DeleteByLocationAndDateAsync(string locationId, DateTimeOffset date, CancellationToken cancellationToken);

    Task<IReadOnlyList<DailyResourceAvailabilitySnapshot>> GetByLocationIdAndDateRangeAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        string? resourceType,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DailyResourceAvailabilitySnapshot>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        string? resourceType,
        CancellationToken cancellationToken);
}

public class DailyResourceAvailabilitySnapshotRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyResourceAvailabilitySnapshot>(dbContext, timeProvider),
        IDailyResourceAvailabilitySnapshotRepository
{
    public DailyResourceAvailabilitySnapshot Add(DailyResourceAvailabilitySnapshot snapshot)
    {
        var now = TimeProvider.GetUtcNow();
        snapshot.CreatedAt = now;
        return DbContext.DailyResourceAvailabilitySnapshot.Add(snapshot).Entity;
    }

    public async Task DeleteByLocationAndDateAsync(string locationId, DateTimeOffset date, CancellationToken cancellationToken)
    {
        var existing = await DbContext.DailyResourceAvailabilitySnapshot
            .Where(item => !item.DeletedAt.HasValue && item.LocationId == locationId && item.Date == date)
            .ToListAsync(cancellationToken);

        var now = TimeProvider.GetUtcNow();
        foreach (var item in existing)
        {
            item.DeletedAt = now;
        }
    }

    public async Task<IReadOnlyList<DailyResourceAvailabilitySnapshot>> GetByLocationIdAndDateRangeAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        string? resourceType,
        CancellationToken cancellationToken) =>
        await DbContext.DailyResourceAvailabilitySnapshot
            .Include(s => s.Resource)
            .ThenInclude(r => r.OrganizationTags)
            .Where(item => !item.DeletedAt.HasValue && item.LocationId == locationId && item.Date >= from && item.Date <= until &&
                           (resourceType == null || item.Resource.OrganizationTags.Any(t => t.Type == resourceType)))
            .OrderBy(item => item.Date)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DailyResourceAvailabilitySnapshot>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        string? resourceType,
        CancellationToken cancellationToken) =>
        await DbContext.DailyResourceAvailabilitySnapshot
            .Include(s => s.Resource)
            .ThenInclude(r => r.OrganizationTags)
            .Where(item => !item.DeletedAt.HasValue && locationIds.Contains(item.LocationId) && item.Date >= from && item.Date <= until &&
                           (resourceType == null || item.Resource.OrganizationTags.Any(t => t.Type == resourceType)))
            .OrderBy(item => item.Date)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
}
