using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Notification.Shared.Database;
using Notification.Shared.Database.Entities;

namespace Notification.Shared.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Team Add(Team team);
    Team Update(Team team);
    Team Remove(Team team);
}

public class TeamRepository(NotificationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<NotificationDbContext, Team>(dbContext, timeProvider), ITeamRepository
{
    public async Task<Team> UpsertNakedAsync(
        string id,
        Organization? organization, 
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team.FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
}
