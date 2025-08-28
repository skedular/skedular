using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        CancellationToken cancellationToken);

    Organization Update(Organization team);
    Organization Remove(Organization team);
}

public class OrganizationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrUniqueAlphanumericNameAsync(id, null, true, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .Include(query => query.OrganizationSsoSettings)
                .Include(query => query.OrganizationMembers.Where(organizationMember =>
                    includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
                .Include(query => query.Teams.Where(location => !location.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .Include(query => query.OrganizationSsoSettings)
                .Include(query => query.OrganizationMembers.Where(organizationMember =>
                    includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
                .Include(query => query.Teams.Where(location => !location.DeletedAt.HasValue))
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
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
