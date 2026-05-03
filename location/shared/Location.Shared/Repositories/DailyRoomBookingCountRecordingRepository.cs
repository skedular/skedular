using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IDailyRoomBookingCountRecordingRepository : IRepository<DailyRoomBookingCountRecording>
{
    Task<IReadOnlyList<DailyRoomBookingCountRecording>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class DailyRoomBookingCountRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyRoomBookingCountRecording>(dbContext, timeProvider),
        IDailyRoomBookingCountRecordingRepository
{
    public async Task<IReadOnlyList<DailyRoomBookingCountRecording>> GetByLocationIdsAndDateRangeAsync(
        IReadOnlyList<string> locationIds,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyRoomBookingCountRecording
            .Where(item => !item.DeletedAt.HasValue && locationIds.Contains(item.Location.Id) && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
}
