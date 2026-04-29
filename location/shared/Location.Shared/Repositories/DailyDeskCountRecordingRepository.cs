using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IDailyDeskCountRecordingRepository : IRepository<DailyDeskCountRecording>
{
    DailyDeskCountRecording Add(DailyDeskCountRecording dailyDeskCountRecording);

    Task<ICollection<DailyDeskCountRecording>> GetByLocationIdsAndDateRangeAsync(
        ICollection<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class DailyDeskCountRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyDeskCountRecording>(dbContext, timeProvider),
        IDailyDeskCountRecordingRepository
{
    public DailyDeskCountRecording Add(DailyDeskCountRecording dailyDeskCountRecording)
    {
        var now = TimeProvider.GetUtcNow();
        dailyDeskCountRecording.CreatedAt = now;
        return DbContext.DailyDeskCountRecording.Add(dailyDeskCountRecording).Entity;
    }

    public async Task<ICollection<DailyDeskCountRecording>> GetByLocationIdsAndDateRangeAsync(
        ICollection<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyDeskCountRecording
            .Where(item =>
                !item.DeletedAt.HasValue &&
                locationIds.Contains(item.Location.Id) &&
                item.Date >= from &&
                item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
}
