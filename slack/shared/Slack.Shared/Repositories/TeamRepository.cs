using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
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
