using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Location>> GetActiveByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Location>> GetDueForDailyUpdateAsync(DateTimeOffset now, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
}

public class LocationRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Include(query => query.DailyUpdateChannel)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    /// <summary>
    ///     Returns the active Slack locations for the supplied identifiers.
    /// </summary>
    /// <param name="ids">The location identifiers to resolve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The non-deleted Slack locations that match the supplied identifiers.</returns>
    /// <remarks>
    ///     This repository-owned lookup replaced the shared specification used by Slack actions when they validate a set of active locations.
    /// </remarks>
    public async Task<ICollection<Location>> GetActiveByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .Include(query => query.DailyUpdateChannel)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Returns the Slack locations that are currently due for an automatic daily update.
    /// </summary>
    /// <param name="now">The current timestamp used to evaluate the daily update schedule.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The Slack locations that satisfy the daily update timing and workspace requirements.</returns>
    /// <remarks>
    ///     This keeps the daily-update scheduling criteria in the repository so jobs can fetch only locations that are ready for another Slack update.
    /// </remarks>
    public async Task<ICollection<Location>> GetDueForDailyUpdateAsync(DateTimeOffset now, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query =>
                !query.DeletedAt.HasValue &&
                (now - query.CreatedAt).TotalHours >= 24 &&
                query.DailyUpdateChannel != null &&
                !query.DailyUpdateChannel.Workspace.DeletedAt.HasValue &&
                (!query.SlackChannelDailyUpdateLastSentAt.HasValue ||
                 (now - query.SlackChannelDailyUpdateLastSentAt.Value).TotalHours >= 23))
            .ToListAsync(cancellationToken);

    public Location Add(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
    }

    public Location Update(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public Location Remove(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public override async Task<Location> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }
}
