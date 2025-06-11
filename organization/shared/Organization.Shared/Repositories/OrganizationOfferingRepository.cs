using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    Task<ICollection<OrganizationOffering>> GetActiveOfferingsAsync(CancellationToken cancellationToken);
    void Add(OrganizationOffering organizationOffering);
    void Remove(OrganizationOffering organizationOffering);
    void RemoveRange(ICollection<OrganizationOffering> organizationOfferings);
    void Undelete(OrganizationOffering organizationOffering);
}

public class OrganizationOfferingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationOffering>(dbContext, timeProvider),
        IOrganizationOfferingRepository
{
    public async Task<ICollection<OrganizationOffering>> GetActiveOfferingsAsync(CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Where(query => !query.DeletedAt.HasValue)
            .Include(query => query.Organization)
            .ToListAsync(cancellationToken);

    public void Add(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.CreatedAt = now;
        DbContext.OrganizationOffering.Add(organizationOffering);
    }

    public void Remove(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.DeletedAt = now;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }

    public void RemoveRange(ICollection<OrganizationOffering> organizationOfferings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOfferings.ForEach(organizationOffering => organizationOffering.DeletedAt = now);
        DbContext.OrganizationOffering.UpdateRange(organizationOfferings);
    }

    public void Undelete(OrganizationOffering organizationOffering)
    {
        organizationOffering.DeletedAt = null;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }
}
