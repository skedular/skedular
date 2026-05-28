using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IDailyDeskBookingCountRecordingRepository : IRepository<DailyDeskBookingCountRecording>
{
    Task<IReadOnlyList<DailyDeskBookingCountRecording>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class DailyDeskBookingCountRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyDeskBookingCountRecording>(dbContext, timeProvider),
        IDailyDeskBookingCountRecordingRepository
{
    public async Task<IReadOnlyList<DailyDeskBookingCountRecording>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyDeskBookingCountRecording
            .Where(item => !item.DeletedAt.HasValue && locationIds.Contains(item.Location.Id) && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
}
