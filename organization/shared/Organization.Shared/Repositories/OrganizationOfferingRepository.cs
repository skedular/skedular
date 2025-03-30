using Enterprise.Shared;
using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    OrganizationOffering Add(OrganizationOffering organizationOffering);
    OrganizationOffering Update(OrganizationOffering organizationOffering);
    void UpdateRange(ICollection<OrganizationOffering> organizationOfferings);
    OrganizationOffering Remove(OrganizationOffering organizationOffering);
    void RemoveRange(ICollection<OrganizationOffering> organizationOfferings);
    OrganizationOffering Undelete(OrganizationOffering organizationOffering);
}

public class OrganizationOfferingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationOffering>(dbContext, timeProvider),
        IOrganizationOfferingRepository
{
    public OrganizationOffering Add(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.CreatedAt = now;
        return DbContext.OrganizationOffering.Add(organizationOffering).Entity;
    }

    public OrganizationOffering Update(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.ModifiedAt = now;
        return DbContext.OrganizationOffering.Update(organizationOffering).Entity;
    }

    public void UpdateRange(ICollection<OrganizationOffering> organizationOfferings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferings.ForEach(organizationOffering => organizationOffering.ModifiedAt = now);
        DbContext.OrganizationOffering.UpdateRange(organizationOfferings);
    }

    public OrganizationOffering Remove(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.DeletedAt = now;
        return DbContext.OrganizationOffering.Update(organizationOffering).Entity;
    }

    public void RemoveRange(ICollection<OrganizationOffering> organizationOfferings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferings.ForEach(organizationOffering => organizationOffering.DeletedAt = now);
        DbContext.OrganizationOffering.UpdateRange(organizationOfferings);
    }

    public OrganizationOffering Undelete(OrganizationOffering organizationOffering)
    {
        organizationOffering.DeletedAt = null;
        return DbContext.OrganizationOffering.Update(organizationOffering).Entity;
    }
}
