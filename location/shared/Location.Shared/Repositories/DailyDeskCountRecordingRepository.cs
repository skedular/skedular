using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;

namespace Location.Shared.Repositories;

public interface IDailyDeskCountRecordingRepository : IRepository<DailyDeskCountRecording>
{
    DailyDeskCountRecording Add(DailyDeskCountRecording dailyDeskCountRecording);
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
}
