using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationXeroConnectionRepository : IRepository<OrganizationXeroConnection>
{
    OrganizationXeroConnection Add(OrganizationXeroConnection organizationXeroConnection);
    OrganizationXeroConnection Update(OrganizationXeroConnection organizationXeroConnection);
    OrganizationXeroConnection Remove(OrganizationXeroConnection organizationXeroConnection);
}

public class OrganizationXeroConnectionRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationXeroConnection>(dbContext, timeProvider), IOrganizationXeroConnectionRepository
{
    public OrganizationXeroConnection Add(OrganizationXeroConnection organizationXeroConnection)
    {
        var now = TimeProvider.GetUtcNow();
        organizationXeroConnection.CreatedAt = now;
        return DbContext.OrganizationXeroConnection.Add(organizationXeroConnection).Entity;
    }

    public OrganizationXeroConnection Update(OrganizationXeroConnection organizationXeroConnection)
    {
        var now = TimeProvider.GetUtcNow();
        organizationXeroConnection.ModifiedAt = now;
        return DbContext.OrganizationXeroConnection.Update(organizationXeroConnection).Entity;
    }

    public OrganizationXeroConnection Remove(OrganizationXeroConnection organizationXeroConnection) =>
        DbContext.OrganizationXeroConnection.Remove(organizationXeroConnection).Entity;
}
