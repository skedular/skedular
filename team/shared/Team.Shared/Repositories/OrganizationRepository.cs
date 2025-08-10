using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, bool includeDeletedOrganizationMembers, CancellationToken cancellationToken);
    Organization Add(Organization team);
    Organization Update(Organization team);
    Organization Remove(Organization team);
}

public class OrganizationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, true, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdAsync(string id, bool includeDeletedOrganizationMembers, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.OrganizationMembers.Where(organizationMember =>
                includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Teams.Where(location => !location.DeletedAt.HasValue))
            .FirstOrDefaultAsync(
                query => query.Id == id || (query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == id),
                cancellationToken);

    public Organization Add(Organization team)
    {
        var now = TimeProvider.GetUtcNow();
        team.CreatedAt = now;
        return DbContext.Organization.Add(team).Entity;
    }

    public Organization Remove(Organization team)
    {
        var now = TimeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Organization.Update(team).Entity;
    }

    public Organization Update(Organization team)
    {
        var now = TimeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Organization.Update(team).Entity;
    }
}
