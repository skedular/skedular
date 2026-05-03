using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IDailyBookingCountRecordingRepository : IRepository<DailyBookingCountRecording>
{
    Task<IReadOnlyList<DailyBookingCountRecording>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class DailyBookingCountRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyBookingCountRecording>(dbContext, timeProvider),
        IDailyBookingCountRecordingRepository
{
    public async Task<IReadOnlyList<DailyBookingCountRecording>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyBookingCountRecording
            .Where(item => !item.DeletedAt.HasValue && locationIds.Contains(item.Location.Id) && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
}
