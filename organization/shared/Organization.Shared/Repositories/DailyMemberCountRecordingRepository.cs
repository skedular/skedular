using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IDailyMemberCountRecordingRepository : IRepository<DailyMemberCountRecording>
{
    DailyMemberCountRecording Add(DailyMemberCountRecording dailyMemberCountRecording);
}

public class DailyMemberCountRecordingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, DailyMemberCountRecording>(dbContext, timeProvider),
        IDailyMemberCountRecordingRepository
{
    public DailyMemberCountRecording Add(DailyMemberCountRecording dailyMemberCountRecording)
    {
        var now = TimeProvider.GetUtcNow();
        dailyMemberCountRecording.CreatedAt = now;
        return DbContext.DailyMemberCountRecording.Add(dailyMemberCountRecording).Entity;
    }
}
