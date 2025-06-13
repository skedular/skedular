using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationBillingDetailsRepository : IRepository<OrganizationBillingDetails>
{
    Task<OrganizationBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationBillingDetails Add(OrganizationBillingDetails address);
    OrganizationBillingDetails Update(OrganizationBillingDetails address);
}

public class OrganizationOrganizationBillingDetailsRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationBillingDetails>(dbContext, timeProvider), IOrganizationBillingDetailsRepository
{
    public async Task<OrganizationBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationBillingDetails
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public OrganizationBillingDetails Add(OrganizationBillingDetails address)
    {
        var now = TimeProvider.GetUtcNow();
        address.CreatedAt = now;
        return DbContext.OrganizationBillingDetails.Add(address).Entity;
    }

    public OrganizationBillingDetails Update(OrganizationBillingDetails address)
    {
        var now = TimeProvider.GetUtcNow();
        address.ModifiedAt = now;
        return DbContext.OrganizationBillingDetails.Update(address).Entity;
    }
}
