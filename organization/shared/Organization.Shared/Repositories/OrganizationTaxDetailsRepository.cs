using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationTaxDetailsRepository : IRepository<OrganizationTaxDetails>
{
    OrganizationTaxDetails Add(OrganizationTaxDetails address);
    OrganizationTaxDetails Update(OrganizationTaxDetails address);
    OrganizationTaxDetails Remove(OrganizationTaxDetails address);
}

public class OrganizationTaxDetailsRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationTaxDetails>(dbContext, timeProvider), IOrganizationTaxDetailsRepository
{
    public OrganizationTaxDetails Add(OrganizationTaxDetails address)
    {
        var now = TimeProvider.GetUtcNow();
        address.CreatedAt = now;
        return DbContext.OrganizationTaxDetails.Add(address).Entity;
    }

    public OrganizationTaxDetails Update(OrganizationTaxDetails address)
    {
        var now = TimeProvider.GetUtcNow();
        address.ModifiedAt = now;
        return DbContext.OrganizationTaxDetails.Update(address).Entity;
    }

    public OrganizationTaxDetails Remove(OrganizationTaxDetails address) => DbContext.OrganizationTaxDetails.Remove(address).Entity;
}
