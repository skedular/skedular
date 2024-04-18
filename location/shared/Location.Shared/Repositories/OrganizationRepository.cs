using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Organization Add(Organization location);
    Organization Update(Organization location);
    Organization Remove(Organization location);
}

public class OrganizationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Organization>(dbContext), IOrganizationRepository
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
            .Include(query => query.Locations.Where(location => !location.DeletedAt.HasValue))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Organization Add(Organization location)
    {
        var now = timeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Organization.Add(location).Entity;
    }

    public Organization Remove(Organization location)
    {
        var now = timeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Organization.Update(location).Entity;
    }

    public Organization Update(Organization location)
    {
        var now = timeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Organization.Update(location).Entity;
    }
}
