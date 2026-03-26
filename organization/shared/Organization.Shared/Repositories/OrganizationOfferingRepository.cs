using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationOfferingRepository : IRepository<OrganizationOffering>
{
    Task<OrganizationOffering?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationOffering>> GetActiveOfferingsAsync(CancellationToken cancellationToken);
    void Add(OrganizationOffering organizationOffering);
    void Remove(OrganizationOffering organizationOffering);
    void Undelete(OrganizationOffering organizationOffering);
    void Update(OrganizationOffering organizationOffering);
}

public class OrganizationOfferingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationOffering>(dbContext, timeProvider), IOrganizationOfferingRepository
{
    public async Task<OrganizationOffering?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationOffering
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationStripeCustomer)
            .Include(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod => !organizationStripePaymentMethod.DeletedAt.HasValue))
            .Include(query => query.OrganizationOfferingActiveMembers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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

    public void Undelete(OrganizationOffering organizationOffering)
    {
        organizationOffering.DeletedAt = null;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }

    public void Update(OrganizationOffering organizationOffering)
    {
        var now = TimeProvider.GetUtcNow();
        organizationOffering.ModifiedAt = now;
        DbContext.OrganizationOffering.Update(organizationOffering);
    }
}
