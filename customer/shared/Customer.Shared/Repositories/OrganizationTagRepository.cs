using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IOrganizationTagRepository : IRepository<OrganizationTag>
{
    Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationTag Add(OrganizationTag organizationTag);
    OrganizationTag Update(OrganizationTag organizationTag);
    void RemoveRange(ICollection<OrganizationTag> organizationTags);
}

public class OrganizationTagRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, OrganizationTag>(dbContext, timeProvider), IOrganizationTagRepository
{
    public async Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
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
            .AsSingleQuery()
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
}
