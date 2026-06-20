using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IDailyBookingCountRecordingRepository : IRepository<DailyBookingCountRecording>
{
    Task<IReadOnlyList<DailyBookingCountRecording>> GetByOrganizationIdAndDateRangeAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);

    Task ReplaceDailyRecordingsAsync(
        string organizationId,
        IReadOnlyList<DailyBookingCountRecording> recordings,
        CancellationToken cancellationToken);
}

public class DailyBookingCountRecordingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, DailyBookingCountRecording>(dbContext, timeProvider),
        IDailyBookingCountRecordingRepository
{
    public async Task<IReadOnlyList<DailyBookingCountRecording>> GetByOrganizationIdAndDateRangeAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await DbContext.DailyBookingCountRecording
            .AsNoTrackingWithIdentityResolution()
            .Where(item =>
                !item.DeletedAt.HasValue &&
                item.Organization.Id == organizationId &&
                item.Date >= from &&
                item.Date <= until)
            .OrderBy(item => item.Date)
            .ToListAsync(cancellationToken);

    public async Task ReplaceDailyRecordingsAsync(
        string organizationId,
        IReadOnlyList<DailyBookingCountRecording> recordings,
        CancellationToken cancellationToken)
    {
        var existingRecordings = await DbContext.DailyBookingCountRecording
            .Where(item => item.Organization.Id == organizationId)
            .ToListAsync(cancellationToken);

        DbContext.DailyBookingCountRecording.RemoveRange(existingRecordings);
        await DbContext.DailyBookingCountRecording.AddRangeAsync(recordings, cancellationToken);
    }
}
