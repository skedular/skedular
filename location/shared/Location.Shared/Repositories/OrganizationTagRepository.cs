using Enterprise.Shared;
using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IOrganizationTagRepository : IRepository<OrganizationTag>
{
    Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationTag Add(OrganizationTag organizationTag);
    OrganizationTag Update(OrganizationTag organizationTag);
    void RemoveRange(ICollection<OrganizationTag> organizationTags);
}

public class OrganizationTagRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, OrganizationTag>(dbContext, timeProvider), IOrganizationTagRepository
{
    public async Task<OrganizationTag> UpsertNakedAsync(
        string id,
        Organization organization,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public OrganizationTag Add(OrganizationTag organizationTag)
    {
        var now = TimeProvider.GetUtcNow();
        organizationTag.CreatedAt = now;
        return DbContext.OrganizationTag.Add(organizationTag).Entity;
    }

    public void RemoveRange(ICollection<OrganizationTag> organizationTags)
    {
        var now = TimeProvider.GetUtcNow();
        organizationTags.ForEach(organizationTag => organizationTag.DeletedAt = now);
        DbContext.OrganizationTag.UpdateRange(organizationTags);
    }

    public OrganizationTag Update(OrganizationTag organizationTag)
    {
        var now = TimeProvider.GetUtcNow();
        organizationTag.ModifiedAt = now;
        return DbContext.OrganizationTag.Update(organizationTag).Entity;
    }

    public async Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationTag
            .Where(query => query.Id == id)
            .Include(query => query.Organization)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);
}
