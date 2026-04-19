using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IDailyMemberCountRecordingRepository : IRepository<DailyMemberCountRecording>
{
    Task<ICollection<DailyMemberCountRecording>> GetByOrganizationIdAndDateRangeAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);

    DailyMemberCountRecording Add(DailyMemberCountRecording dailyMemberCountRecording);
}

public class DailyMemberCountRecordingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, DailyMemberCountRecording>(dbContext, timeProvider),
        IDailyMemberCountRecordingRepository
{
    /// <summary>
    ///     Returns daily member count recordings for an organization within the requested date range.
    /// </summary>
    /// <param name="organizationId">The organization identifier that owns the recordings.</param>
    /// <param name="from">The inclusive lower bound for the recording date.</param>
    /// <param name="until">The inclusive upper bound for the recording date.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching recordings ordered by date.</returns>
    /// <remarks>
    ///     This range query keeps analytics-specific filtering inside the repository and preserves the earlier untracked behavior for read-only chart
    ///     generation.
    /// </remarks>
    public async Task<ICollection<DailyMemberCountRecording>> GetByOrganizationIdAndDateRangeAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyMemberCountRecording
            .AsNoTracking()
            .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId && query.Date >= from && query.Date <= until)
            .OrderBy(query => query.Date)
            .ToListAsync(cancellationToken);

    public DailyMemberCountRecording Add(DailyMemberCountRecording dailyMemberCountRecording)
    {
        var now = TimeProvider.GetUtcNow();
        dailyMemberCountRecording.CreatedAt = now;
        return DbContext.DailyMemberCountRecording.Add(dailyMemberCountRecording).Entity;
    }
}
