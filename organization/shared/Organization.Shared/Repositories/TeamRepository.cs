using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team> UpsertNakedAsync(
        string id,
        Database.Entities.Organization organization,
        CancellationToken cancellationToken);

    Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Team Add(Team team);
    Team Update(Team team);
    Team Remove(Team team);
}

public class TeamRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Team>(dbContext, timeProvider), ITeamRepository
{
    public async Task<Team> UpsertNakedAsync(
        string id,
        Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Database.Entities.Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => query.Id == id)
            .Include(query => query.Organization)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

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
