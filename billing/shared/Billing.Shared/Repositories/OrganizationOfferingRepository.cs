using Billing.Shared.Database;
using Billing.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;

namespace Billing.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    OrganizationOffering Add(OrganizationOffering organizationOffering);
    OrganizationOffering Update(OrganizationOffering organizationOffering);
    void UpdateRange(ICollection<OrganizationOffering> organizationOfferings);
    void RemoveRange(ICollection<OrganizationOffering> organizationOfferings);
}

public class OrganizationOfferingRepository(BillingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BillingDbContext, OrganizationOffering>(dbContext), IOrganizationOfferingRepository
{
    public OrganizationOffering Add(OrganizationOffering organizationOffering)
    {
        var now = timeProvider.GetUtcNow();
        organizationOffering.CreatedAt = now;
        return DbContext.OrganizationOffering.Add(organizationOffering).Entity;
    }

    public OrganizationOffering Update(OrganizationOffering organizationOffering)
    {
        var now = timeProvider.GetUtcNow();
        organizationOffering.ModifiedAt = now;
        return DbContext.OrganizationOffering.Update(organizationOffering).Entity;
    }

    public void UpdateRange(ICollection<OrganizationOffering> organizationOfferings)
    {
        var now = timeProvider.GetUtcNow();
        organizationOfferings.ForEach(organizationOffering => organizationOffering.ModifiedAt = now);
        DbContext.OrganizationOffering.UpdateRange(organizationOfferings);
    }

    public void RemoveRange(ICollection<OrganizationOffering> organizationOfferings)
    {
        var now = timeProvider.GetUtcNow();
        organizationOfferings.ForEach(organizationOffering => organizationOffering.DeletedAt = now);
        DbContext.OrganizationOffering.UpdateRange(organizationOfferings);
    }
}
