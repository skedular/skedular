using Billing.Shared.Database;
using Billing.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Billing.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    OrganizationOffering Add(OrganizationOffering organizationOffering);
    OrganizationOffering Update(OrganizationOffering organizationOffering);
    void UpdateRange(ICollection<OrganizationOffering> organizationOfferings);
    void RemoveRange(ICollection<OrganizationOffering> organizationOfferings);
    Task<ICollection<OrganizationOffering>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationOfferingRepository(BillingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BillingDbContext, OrganizationOffering>(dbContext, timeProvider), IOrganizationOfferingRepository
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

    public async Task<ICollection<OrganizationOffering>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Where(query => query.Organization.Id == organizationId)
            .Include(query => query.Organization)
            .ToListAsync(cancellationToken);
}
