using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Organization Add(Organization team);
    Organization Update(Organization team);
    Organization Remove(Organization team);
}

public class OrganizationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Organization>(dbContext), IOrganizationRepository
{
    public async Task<Organization>
        UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Organization.Add(new Organization { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => query.Id == id)
            .Include(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Teams.Where(location => !location.DeletedAt.HasValue))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Organization Add(Organization team)
    {
        var now = timeProvider.GetUtcNow();
        team.CreatedAt = now;
        return DbContext.Organization.Add(team).Entity;
    }

    public Organization Remove(Organization team)
    {
        var now = timeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Organization.Update(team).Entity;
    }

    public Organization Update(Organization team)
    {
        var now = timeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Organization.Update(team).Entity;
    }
}
