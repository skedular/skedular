using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationBillingDetailsRepository : IRepository<OrganizationBillingDetails>
{
    Task<OrganizationBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationBillingDetails Add(OrganizationBillingDetails organizationBillingDetails);
    OrganizationBillingDetails Update(OrganizationBillingDetails organizationBillingDetails);
}

public class OrganizationOrganizationBillingDetailsRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationBillingDetails>(dbContext, timeProvider), IOrganizationBillingDetailsRepository
{
    public async Task<OrganizationBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationBillingDetails
            .AsSingleQuery()
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public OrganizationBillingDetails Add(OrganizationBillingDetails organizationBillingDetails)
    {
        var now = TimeProvider.GetUtcNow();
        organizationBillingDetails.CreatedAt = now;
        return DbContext.OrganizationBillingDetails.Add(organizationBillingDetails).Entity;
    }

    public OrganizationBillingDetails Update(OrganizationBillingDetails organizationBillingDetails)
    {
        var now = TimeProvider.GetUtcNow();
        organizationBillingDetails.ModifiedAt = now;
        return DbContext.OrganizationBillingDetails.Update(organizationBillingDetails).Entity;
    }
}
