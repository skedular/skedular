using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Team>> GetActiveByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<Team>> GetDueForDailyUpdateAsync(DateTimeOffset now, CancellationToken cancellationToken);
    Team Add(Team team);
    Team Update(Team team);
    Team Remove(Team team);
}

public class TeamRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, Team>(dbContext, timeProvider), ITeamRepository
{
    public async Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Include(query => query.DailyUpdateChannel)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    /// <summary>
    ///     Returns the active Slack teams for the supplied identifiers.
    /// </summary>
    /// <param name="ids">The team identifiers to resolve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The non-deleted Slack teams that match the supplied identifiers.</returns>
    /// <remarks>
    ///     This repository-owned lookup replaced the shared specification used by Slack actions when they validate a set of active teams.
    /// </remarks>
    public async Task<IReadOnlyList<Team>> GetActiveByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await DbContext.Team
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .Include(query => query.DailyUpdateChannel)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Returns the Slack teams that are currently due for an automatic daily update.
    /// </summary>
    /// <param name="now">The current timestamp used to evaluate the daily update schedule.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The Slack teams that satisfy the daily update timing and workspace requirements.</returns>
    /// <remarks>
    ///     This keeps the daily-update scheduling criteria in the repository so jobs can fetch only teams that are ready for another Slack update.
    /// </remarks>
    public async Task<IReadOnlyList<Team>> GetDueForDailyUpdateAsync(DateTimeOffset now, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query =>
                !query.DeletedAt.HasValue &&
                (now - query.CreatedAt).TotalHours >= 24 &&
                query.DailyUpdateChannel != null &&
                !query.DailyUpdateChannel.Workspace.DeletedAt.HasValue &&
                (!query.SlackChannelDailyUpdateLastSentAt.HasValue ||
                 (now - query.SlackChannelDailyUpdateLastSentAt.Value).TotalHours >= 23))
            .ToListAsync(cancellationToken);

    public Team Add(Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.CreatedAt = now;
        return DbContext.Team.Add(team).Entity;
    }

    public Team Update(Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public Team Remove(Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public override async Task<Team> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }
}
