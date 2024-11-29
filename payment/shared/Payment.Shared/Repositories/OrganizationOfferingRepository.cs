using Enterprise.Shared;
using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    OrganizationOffering Add(OrganizationOffering organizationOffering);
    OrganizationOffering Update(OrganizationOffering organizationOffering);
    void UpdateRange(ICollection<OrganizationOffering> organizationOfferings);
    void RemoveRange(ICollection<OrganizationOffering> organizationOfferings);
}

public class OrganizationOfferingRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, OrganizationOffering>(dbContext, timeProvider), IOrganizationOfferingRepository
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

    public void RemoveRange(ICollection<OrganizationOffering> organizationOfferings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferings.ForEach(organizationOffering => organizationOffering.DeletedAt = now);
        DbContext.OrganizationOffering.UpdateRange(organizationOfferings);
    }
}
