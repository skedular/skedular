using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationTagRepository : IRepository<OrganizationTag>
{
    Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationTag Add(OrganizationTag organizationTag);
    OrganizationTag Update(OrganizationTag organizationTag);
    void RemoveRange(ICollection<OrganizationTag> organizationTags);
}

public class OrganizationTagRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationTag>(dbContext, timeProvider), IOrganizationTagRepository
{
    public async Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationTag
            .AsSingleQuery()
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
}
