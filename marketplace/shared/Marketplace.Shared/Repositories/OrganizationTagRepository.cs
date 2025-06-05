using Enterprise.Shared;
using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationTagRepository : IRepository<OrganizationTag>
{
    OrganizationTag Add(OrganizationTag organizationTag);
    OrganizationTag Update(OrganizationTag organizationTag);
    void RemoveRange(ICollection<OrganizationTag> organizationTags);
}

public class OrganizationTagRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationTag>(dbContext, timeProvider), IOrganizationTagRepository
{
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
