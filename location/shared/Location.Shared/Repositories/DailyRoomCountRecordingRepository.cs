using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Location.Shared.Database;
using Location.Shared.Database.Entities;

namespace Location.Shared.Repositories;

public interface IDailyRoomCountRecordingRepository : IRepository<DailyRoomCountRecording>
{
    DailyRoomCountRecording Add(DailyRoomCountRecording dailyRoomCountRecording);
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
}
