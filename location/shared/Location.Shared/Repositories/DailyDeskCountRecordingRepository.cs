using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;

namespace Location.Shared.Repositories;

public interface IDailyDeskCountRecordingRepository : IRepository<DailyDeskCountRecording>
{
    DailyDeskCountRecording Add(DailyDeskCountRecording dailyDeskCountRecording);
}

public class DailyDeskCountRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyDeskCountRecording>(dbContext), IDailyDeskCountRecordingRepository
{
    public DailyDeskCountRecording Add(DailyDeskCountRecording dailyDeskCountRecording)
    {
        var now = timeProvider.GetUtcNow();
        dailyDeskCountRecording.CreatedAt = now;
        return DbContext.DailyDeskCountRecording.Add(dailyDeskCountRecording).Entity;
    }
}
