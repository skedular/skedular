using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IDailyRoomCountRecordingRepository : IRepository<DailyRoomCountRecording>
{
    DailyRoomCountRecording Add(DailyRoomCountRecording dailyRoomCountRecording);

    Task<ICollection<DailyRoomCountRecording>> GetByLocationIdsAndDateRangeAsync(
        ICollection<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class DailyRoomCountRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyRoomCountRecording>(dbContext, timeProvider),
        IDailyRoomCountRecordingRepository
{
    public DailyRoomCountRecording Add(DailyRoomCountRecording dailyRoomCountRecording)
    {
        var now = TimeProvider.GetUtcNow();
        dailyRoomCountRecording.CreatedAt = now;
        return DbContext.DailyRoomCountRecording.Add(dailyRoomCountRecording).Entity;
    }

    public async Task<ICollection<DailyRoomCountRecording>> GetByLocationIdsAndDateRangeAsync(
        ICollection<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyRoomCountRecording
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
